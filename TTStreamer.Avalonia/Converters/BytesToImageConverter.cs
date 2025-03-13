using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace TTStreamer.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var bytes = (byte[])value;
            return new Avalonia.Media.Imaging.Bitmap(new MemoryStream(bytes));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}