using System;
using System.Globalization;
using System.Windows.Data;

namespace YoutubeVideoDownloadEditor.Converters
{
    public class IsPlayingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPlaying)
            {
                return isPlaying ? "Pause" : "Play";
            }

            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Pause";
        }
    }
}
