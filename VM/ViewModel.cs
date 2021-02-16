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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AngelMP3.Model;

namespace AngelMP3.VM
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly PlayerModel _playerModel = new PlayerModel();
        private readonly MediaController _mediaController = new MediaController();
        public MediaController MediaControl {
            get
            {
                return _mediaController;
            }
        }
        public ReadOnlyObservableCollection<Song> TrackList { get; set; }
        private List<Song> filteredTrackList;
        public List<Song> FilteredTrackList { 
            get { return filteredTrackList; } 
            set
            {
                filteredTrackList = value;
                OnPropertyChanged("FilteredTrackList");
            }
        }
        public void UpdateFilteredTrackList()
        {
            List<Song> templist = new List<Song>();
            foreach (Song song in TrackList)
            {
                if (SongFilter(song))
                {
                    templist.Add(song);
                }
            }
            FilteredTrackList = templist;
        }
        public ViewModel()
        {
            TrackList = _playerModel.TrackList;
            UpdateFilteredTrackList();
        }
        private string _filterString = "";
        public string FilterString
        {
            get { return _filterString; }
            set
             {
                _filterString = value;
                OnPropertyChanged("FilterString");
                UpdateFilteredTrackList();
            }
        }
        private bool SongFilter(object item)
        {
            Song song = item as Song;
            return song.Name.ToLower().Contains(FilterString.ToLower()) ||
                   song.Author.ToLower().Contains(FilterString.ToLower());
        }
        public void NextSong()
        {
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
        private RelayCommand playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ??
                    (playCommand = new RelayCommand(obj => {
                        
                    }));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MediaController : INotifyPropertyChanged
    {
        private readonly MediaPlayer mediaPlayer;
        private Song _nowPlaying;
        public Song NowPlaying
        {
            get {
                return _nowPlaying;
            }
            set {
                if (_nowPlaying != value)
                {
                    _nowPlaying = value;
                    OnPropertyChanged("NowPlaying");
                }
            }
        }
        public event EventHandler SongEnded;

        public MediaController() {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += (Object s, EventArgs e) => SongEnded?.Invoke(this, e);
        }
        public bool PlaySong(Song newSong)
        {
            try
            {
                if (NowPlaying != null)
                {
                    if (newSong.Path == NowPlaying.Path && mediaPlayer.HasAudio)
                    {
                        mediaPlayer.Play();
                        return true;
                    }
                }
                mediaPlayer.Open(new Uri(newSong.Path));
                mediaPlayer.Play();
                NowPlaying = newSong;
            } catch
            {
                return false;
            }
            return true;
        }
        public bool PauseSong(Song song)
        {
            if (NowPlaying != null && mediaPlayer.HasAudio)
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
