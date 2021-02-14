using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AngelMP3.Model;

namespace AngelMP3.VM
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        readonly PlayerModel _playerModel = new PlayerModel();
        public ReadOnlyObservableCollection<Song> TrackList => _playerModel.TrackList;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
    
}
