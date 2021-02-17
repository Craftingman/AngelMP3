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
        private string playButtonImg = "img/play_icon.png";
        public string PlayButtonImg
        {
            get
            {
                return playButtonImg;
            }
            set
            {
                playButtonImg = value;
                OnPropertyChanged("PlayButtonImg");
            }
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

        public ViewModel()
        {
            TrackList = _playerModel.TrackList;
            UpdateFilteredTrackList();
            MediaControl.PropertyChanged += (object obj, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "IsPlaying")
                {
                    ChangePlayButtonImg(MediaControl.IsPlaying);
                }
            };
        }
        private void ChangePlayButtonImg(bool playing)
        {
            if (playing)
            {
                PlayButtonImg = "img/pause_icon.png";
                return;
            }
            PlayButtonImg = "img/play_icon.png";
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
        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return playPauseCommand ??
                    (playPauseCommand = new RelayCommand(obj => {
                        MediaControl.PausePlaySong();
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
                    if(PlaySong(value))
                    {
                        _nowPlaying = value;
                        OnPropertyChanged("NowPlaying");
                    }
                }
            }
        }
        private bool isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            private set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    OnPropertyChanged("IsPlaying");
                }
            }
        }
        public event EventHandler SongEnded;

        public MediaController() {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += (Object s, EventArgs e) => SongEnded?.Invoke(this, e);
        }
        public bool PausePlaySong()
        {
            if (IsPlaying)
            {
                return PauseSong();
            }
            return PlaySong(NowPlaying);
        }
        public bool PlaySong(Song newSong)
        {
            if (newSong != null)
            {
                if (NowPlaying != null)
                {
                    if (newSong.Path == NowPlaying.Path && mediaPlayer.HasAudio)
                    {
                        mediaPlayer.Play();
                        IsPlaying = true;
                        return true;
                    }
                }
                mediaPlayer.Open(new Uri(newSong.Path));
                mediaPlayer.Play();
                IsPlaying = true;
                return true;
            }
            return false;
        }
        public bool PauseSong()
        {
            if (NowPlaying != null && mediaPlayer.HasAudio)
            {
                mediaPlayer.Pause();
                IsPlaying = false;
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
