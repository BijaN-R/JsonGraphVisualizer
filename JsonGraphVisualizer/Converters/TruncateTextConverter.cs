using System;
using System.Globalization;
using System.Windows.Data;

namespace JsonGraphVisualizer.Converters
{
    public class TruncateTextConverter : IValueConverter
    {
        private const int MaxLength = 70;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string text = value.ToString();

            if (text.Length <= MaxLength)
                return text;

            return text.Substring(0, MaxLength) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}
