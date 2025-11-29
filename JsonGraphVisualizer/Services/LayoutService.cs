// Services/LayoutService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JsonGraphVisualizer.Models;

namespace JsonGraphVisualizer.Services
{
    public class LayoutService
    {
        private const double HorizontalSpacing = 100;
        private const double VerticalSpacing = 25;
        private const double NodePadding = 20;
        private const double PropertyLineHeight = 18;
        private const double HeaderHeight = 40;
        private const double MinNodeWidth = 150;

        private readonly Typeface _typeface;

        public LayoutService()
        {
            // استفاده از فونت Consolas برای محاسبه دقیق
            _typeface = new Typeface(
                new FontFamily("Consolas"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal
            );
        }

        public void CalculateLayout(List<JsonNodeModel> rootNodes)
        {
            if (rootNodes == null || !rootNodes.Any())
                return;

            // ابتدا ابعاد همه نودها رو محاسبه می‌کنیم
            foreach (var node in rootNodes)
            {
                CalculateNodeDimensions(node);
            }

            // سپس موقعیت‌ها رو محاسبه می‌کنیم
            double currentY = 50; // فاصله از بالا
            foreach (var node in rootNodes)
            {
                CalculateNodePositions(node, 50, currentY, 0); // x=50, depth=0
                currentY += GetSubtreeHeight(node) + VerticalSpacing;
            }
        }

        private void CalculateNodeDimensions(JsonNodeModel node)
        {
            if (node == null)
                return;

            // محاسبه عرض بر اساس محتوا
            double maxTextWidth = MeasureString(node.Title, 14, FontWeights.Bold);

            // بررسی عرض Properties
            if (node.Properties != null && node.Properties.Any())
            {
                foreach (var prop in node.Properties)
                {
                    string text = $"{prop.Key}: {FormatValue(prop.Value)}";
                    double textWidth = MeasureString(text, 11, FontWeights.Normal);
                    maxTextWidth = Math.Max(maxTextWidth, textWidth);
                }
            }

            node.Width = Math.Max(MinNodeWidth, maxTextWidth + NodePadding * 2);

            // محاسبه ارتفاع بر اساس تعداد Properties
            int propertyCount = node.Properties?.Count ?? 0;
            node.Height = HeaderHeight + (propertyCount * PropertyLineHeight) + NodePadding;

            // محاسبه بازگشتی برای فرزندان
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    CalculateNodeDimensions(child);
                }
            }
        }

        private void CalculateNodePositions(JsonNodeModel node, double x, double y, int depth)
        {
            if (node == null)
                return;

            node.X = x;
            node.Y = y;

            // محاسبه موقعیت فرزندان
            if (node.Children != null && node.Children.Any())
            {
                double childX = x + node.Width + HorizontalSpacing;
                double childY = y;

                foreach (var child in node.Children)
                {
                    CalculateNodePositions(child, childX, childY, depth + 1);
                    childY += GetSubtreeHeight(child) + VerticalSpacing;
                }

                // تراز مرکزی parent با فرزندان
                double totalChildrenHeight = node.Children.Sum(c => GetSubtreeHeight(c))
                                            + (node.Children.Count - 1) * VerticalSpacing;
                double centerOffset = (totalChildrenHeight - node.Height) / 2;

                if (centerOffset > 0)
                {
                    node.Y = y + centerOffset;
                }
            }
        }

        private double GetSubtreeHeight(JsonNodeModel node)
        {
            if (node == null)
                return 0;

            if (node.Children == null || !node.Children.Any())
                return node.Height;

            double childrenHeight = node.Children.Sum(c => GetSubtreeHeight(c))
                                   + (node.Children.Count - 1) * VerticalSpacing;

            return Math.Max(node.Height, childrenHeight);
        }

        private double MeasureString(string text, double fontSize, FontWeight fontWeight)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // محدود کردن به 70 کاراکتر (برای truncate)
            if (text.Length > 70)
                text = text.Substring(0, 70) + "...";

            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                _typeface,
                fontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Ideal // PixelsPerDip
            );

            return formattedText.Width;
        }

        private string FormatValue(object value)
        {
            if (value == null)
                return "null";

            if (value is string str)
                return $"\"{str}\"";

            return value.ToString();
        }

        // 📍 محاسبه نقاط اتصال برای Edge ها
        public Point GetConnectionPoint(JsonNodeModel node, bool isStart)
        {
            if (node == null)
                return new Point(0, 0);

            if (isStart)
            {
                // نقطه خروج: سمت راست وسط node
                return new Point(node.X + node.Width, node.Y + node.Height / 2);
            }
            else
            {
                // نقطه ورود: سمت چپ وسط node
                return new Point(node.X, node.Y + node.Height / 2);
            }
        }
    }
}
