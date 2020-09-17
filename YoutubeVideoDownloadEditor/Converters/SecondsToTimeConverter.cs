using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YoutubeVideoDownloadEditor.Converters
{
    public class SecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double timeSeconds))
            {
                return TimeSpan.FromSeconds(timeSeconds);
            }

            return TimeSpan.FromSeconds(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string timeString)
            {
                if (TimeSpan.TryParse(timeString, out TimeSpan time))
                {
                    return time.TotalSeconds;
                }
            }

            return (double)0;
        }
    }
}
