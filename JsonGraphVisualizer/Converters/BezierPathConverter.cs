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

            int startOffset = 2;
            int endOffset = 6;

            Point startPoint = (Point)values[0];
            Point endPoint = (Point)values[1];

            endPoint = new Point(endPoint.X - endOffset / 2, endPoint.Y);

            // محاسبه نقاط کنترل برای منحنی Bezier
            double controlPointOffset = Math.Abs(endPoint.X - startPoint.X) / 2;
            Point postStartPoint = new Point(startPoint.X + startOffset, startPoint.Y);
            Point preEndPoint = new Point(endPoint.X - endOffset, endPoint.Y);

            Point controlPoint1 = new Point(startPoint.X + controlPointOffset, startPoint.Y);
            Point controlPoint2 = new Point(endPoint.X - controlPointOffset, endPoint.Y);

            // ایجاد path geometry
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure { StartPoint = startPoint };

            var startSegment = new LineSegment(postStartPoint, true);
            var bezierSegment = new BezierSegment(controlPoint1, controlPoint2, preEndPoint, true);
            var endSegment = new LineSegment(endPoint, true);

            pathFigure.Segments.Add(startSegment);
            pathFigure.Segments.Add(bezierSegment);
            pathFigure.Segments.Add(endSegment);
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}
