using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonGraphVisualizer.Converters
{
    public class BezierPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            Point startPoint = (Point)values[0];
            Point endPoint = (Point)values[1];

            // محاسبه نقاط کنترل برای منحنی Bezier
            double controlPointOffset = Math.Abs(endPoint.X - startPoint.X) / 2;

            Point controlPoint1 = new Point(startPoint.X + controlPointOffset, startPoint.Y);
            Point controlPoint2 = new Point(endPoint.X - controlPointOffset, endPoint.Y);

            // ایجاد path geometry
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure { StartPoint = startPoint };

            BezierSegment bezierSegment = new BezierSegment(
                controlPoint1,
                controlPoint2,
                endPoint,
                true
            );

            pathFigure.Segments.Add(bezierSegment);
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}
