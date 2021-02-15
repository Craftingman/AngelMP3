using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Data;
using AngelMP3.Model;

namespace AngelMP3.VM
{
    public class ViewModel : INotifyPropertyChanged
    {
        readonly PlayerModel _playerModel = new PlayerModel();
        public ReadOnlyObservableCollection<Song> TrackList { get; set; }
        public ICollectionView TrackListView { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            TrackList = _playerModel.TrackList;
            TrackListView = CollectionViewSource.GetDefaultView(TrackList);
            TrackListView.Filter = SongFilter;
        }
        private string _filterString = "";
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged("FilterString");
                TrackListView.Refresh();
            }
        }
        private bool SongFilter(object item)
        {
            Song song = item as Song;
            return song.Name.ToLower().Contains(FilterString.ToLower()) ||
                   song.Author.ToLower().Contains(FilterString.ToLower());
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private RelayCommand filterTrackListCommand;
        public RelayCommand FilterTrackListCommand
        {
            get
            {
                return filterTrackListCommand ??
                    (filterTrackListCommand = new RelayCommand(obj => {
                        string newFilterString = obj as string;
                        FilterString = newFilterString;
                    }));
            }
        }
    }

    class MediaController {
        private readonly MediaPlayer mediaPlayer;
        private Song nowPlaying;

        public MediaController() {
            mediaPlayer = new MediaPlayer();
        }
        public bool PlaySong(Song newSong)
        {
            try
            {
                if (nowPlaying != null)
                {
                    if (newSong.Path == nowPlaying.Path && mediaPlayer.HasAudio)
                    {
                        mediaPlayer.Play();
                        return true;
                    }
                }
                mediaPlayer.Open(new Uri(newSong.Path));
                mediaPlayer.Play();
                nowPlaying = newSong;
            } catch
            {
                return false;
            }
            return true;
        }
        public bool PauseSong(Song song)
        {
            if (nowPlaying != null && mediaPlayer.HasAudio)
            {
                mediaPlayer.Pause();
                return true;
            }
            return false;
        }
        public void ChangeVolume(float newVolume)
        {
            if (newVolume > 1)
            {
                mediaPlayer.Volume = 1;
                return;
            }
            if (newVolume < 0) 
            {
                mediaPlayer.Volume = 0;
                return;
            }
            mediaPlayer.Volume = newVolume;
        }
        public void ChangePosition() 
        { 
        
        }
    }
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
