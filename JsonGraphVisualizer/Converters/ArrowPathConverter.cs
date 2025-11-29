using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonGraphVisualizer.Converters
{
    public class ArrowPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Point endPoint))
                return null;

            // ایجاد فلش کوچک در انتهای خط
            double arrowSize = 8;

            PathGeometry arrowGeometry = new PathGeometry();
            PathFigure arrowFigure = new PathFigure
            {
                StartPoint = new Point(endPoint.X - arrowSize, endPoint.Y - arrowSize / 2)
            };

            arrowFigure.Segments.Add(new LineSegment(endPoint, true));
            arrowFigure.Segments.Add(new LineSegment(
                new Point(endPoint.X - arrowSize, endPoint.Y + arrowSize / 2), true));
            arrowFigure.IsClosed = true;

            arrowGeometry.Figures.Add(arrowFigure);

            return arrowGeometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}