using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using TagLib;

namespace AngelMP3.Model
{
    class PlayerModel
    { 
        private readonly FsController fsController = new FsController(@"D:\music\test");
        private readonly ObservableCollection<Song> _trackList = new ObservableCollection<Song>();
        public readonly ReadOnlyObservableCollection<Song> TrackList;
        public PlayerModel() {
            TrackList = new ReadOnlyObservableCollection<Song>(_trackList);
            UpdateTrackList();
        }
        public void UpdateTrackList() {
            _trackList.Clear();
            foreach (Song song in fsController.GetSongs()) {
                _trackList.Add(song);
            }
        }
    }
    class FsController
    {
        public DirectoryInfo CurrentDir { get; set; }
        private List<FileInfo> SongFiles { get; set; }
        public FsController() { 
            
        }
        public FsController(string path)
        {
            try
            {
                CurrentDir = new DirectoryInfo(path);
                SongFiles = new List<FileInfo>(CurrentDir.GetFiles("*.mp3", SearchOption.AllDirectories));
            } catch
            {
                SongFiles = new List<FileInfo>();
            }
            
        }
        public void UpdatePath() { 
        
        }
        public List<Song> GetSongs() {
            List<Song> songList = new List<Song>();
            foreach (FileInfo sf in SongFiles) {
                if (!sf.Exists) continue;
                string fullPath = sf.FullName;
                var tfile = TagLib.File.Create(@$"{fullPath}");
                string title = tfile.Tag.Title != null ?
                    tfile.Tag.Title : sf.Name;
                string author = tfile.Tag.FirstPerformer != null ? 
                    tfile.Tag.FirstPerformer : "Unknown";
                CustomTimeSpan duration = new CustomTimeSpan(tfile.Properties.Duration);
                songList.Add(new Song(title, author, duration, fullPath));
            }
            return songList;
        }
        public bool ValidateSongPath(string path) {
            return (new FileInfo(path)).Exists;
        }
    }
    public class CustomTimeSpan {
        public TimeSpan Value { get; set; }
        public CustomTimeSpan(TimeSpan ts) {
            Value = ts;
        }
        public override string ToString()
        {
            int resMinutes = Value.Minutes;
            int numSec = Convert.ToInt32(Value.Seconds);
            string resSeconds = numSec > 9 ? numSec.ToString() : $"0{numSec}";
            if (Value.Hours >= 1) { 
                resMinutes = Value.Minutes + Value.Hours*60;
            }
            return $"{resMinutes}:{resSeconds}";
        }
    }
    public class Song
    {
        public string Name { get; }
        public string Author { get; }
        public CustomTimeSpan Duration { get; }
        public string Path { get; }

        public Song(string name, string author, CustomTimeSpan duration, string path)
        {
            Name = name;
            Author = author;
            Duration = duration;
            Path = path;
        }
        public override string ToString()
        {
            return $"{Name} : {Author} : {Duration} : {Path}";
        }
    }
}
