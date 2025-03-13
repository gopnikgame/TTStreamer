using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TTStreamer.Models
{
    public class GiftView : INotifyPropertyChanged
    {
        private bool _repeatEnded;
        private int _repeatCount = 1;
        public byte[] Image { get; set; }
        public string Sender { get; set; }
        public string Text => $"{Sender} прислал {RepeatCount} {GiftName}";
        public int GiftId { get; set; }

        public string GiftName { get; set; }
        public bool RepeatEnded
        {
            get => _repeatEnded;
            set
            {
                SetField(ref _repeatEnded, value);
                OnPropertyChanged(nameof(Text));
            }
        }

        public int RepeatCount
        {
            get => _repeatCount;
            set
            {
                SetField(ref _repeatCount, value);
                OnPropertyChanged(nameof(Text));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}