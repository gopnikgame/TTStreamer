using System;
using System.Globalization;
using System.IO;

using Avalonia.Data.Converters;

using TTStreamer.Services;

namespace TTStreamer.Converters
{
    public class GiftIdToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                var giftService = App.GetService<GiftService>();
                var giftData = giftService.Find((int)value);
                if (giftData is null) return null;
                var image  = System.Convert.FromBase64String(giftData.Image);
                return new Avalonia.Media.Imaging.Bitmap(new MemoryStream(image));

            }
            catch (Exception e)
            {
                return value.ToString();
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}