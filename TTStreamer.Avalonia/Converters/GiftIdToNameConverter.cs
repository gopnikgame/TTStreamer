using System;
using System.Globalization;
using Avalonia.Data.Converters;
using TTStreamer.Services;

namespace TTStreamer.Converters
{
    public class GiftIdToNameConverter:IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                var giftService = App.GetService<GiftService>();
                var gift = giftService.Find((int)value);
                return gift?.Name;
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