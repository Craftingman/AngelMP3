using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Media;
using System.IO;
using TagLib;

namespace AngelMP3.Model
{
    class PlayerModel
    {
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        private readonly FsController fsController = new FsController(@"D:\Music");
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
            CurrentDir = new DirectoryInfo(path);
            SongFiles = new List<FileInfo>(CurrentDir.GetFiles("*.mp3", SearchOption.AllDirectories));
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
                TimeSpan duration = tfile.Properties.Duration;
                songList.Add(new Song(title, author, duration, fullPath));
            }
            return songList;
        }
    }
    public class Song
    {
        public string Name { get; }
        public string Author { get; }
        public TimeSpan Duration { get; }
        public string Path { get; }

        public Song(string name, string author, TimeSpan duration, string path)
        {
            Name = name;
            Author = author;
            Duration = duration;
            Path = path;
        }
    }
}
