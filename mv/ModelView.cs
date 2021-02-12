using System;
using System.Collections.Generic;
using System.Text;

namespace AngelMP3.mv
{
    public class ModelView
    {
        public static List<Song> Songs = new List<Song> {
            new Song("Black in Black", "ACDC", "3:09"),
            new Song("Dungeon", "Gucci", "3:33"),
            new Song("Группа Крови", "Кино", "2:30"),
        };
    }
    public class Song
    {
        private string _name;
        private string _author;
        private string _duration;

        public string Name { get; set; }
        public string Author { get; set; }
        public string Duration { get; set; }

        public Song(string name, string author, string duration)
        {
            Name = name;
            Author = author;
            Duration = duration;
        }
    }
}
