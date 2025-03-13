using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TTStreamer.Services;

namespace TTStreamer.Models
{
    [ObservableObject]
    public partial class SoundsView
    {
        public ObservableCollection<KeyValuePair<int, string>> KeyList { get; set; } 
        public ObservableCollection<string> AudioList { get; set; } 

        private readonly SoundService soundService;

        public SoundsView(SoundService soundService)
        {
            this.soundService = soundService;
            KeyList = new ObservableCollection<KeyValuePair<int,string>>(soundService.PlayList());
            AudioList = new ObservableCollection<string>(soundService.SoundList());
        }

        public void Update(int id, string sound)
        {
            soundService.Update(id, sound);
        }
    }
}
