// Controls/JsonModal.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace JsonGraphVisualizer.Controls
{
    public partial class JsonModal : Window
    {
        private readonly object _originalData;

        public static readonly DependencyProperty KeyColorProperty =
            DependencyProperty.Register(
                nameof(KeyColor),
                typeof(Brush),
                typeof(JsonModal),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255))));

        public Brush KeyColor
        {
            get => (Brush)GetValue(KeyColorProperty);
            set => SetValue(KeyColorProperty, value);
        }

        public static readonly DependencyProperty ValueColorProperty =
            DependencyProperty.Register(
                nameof(ValueColor),
                typeof(Brush),
                typeof(JsonModal),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)))); 

        public Brush ValueColor
        {
            get => (Brush)GetValue(ValueColorProperty);
            set => SetValue(ValueColorProperty, value);
        }


        public JsonModal(string title, object jsonData)
        {
            InitializeComponent();

            TitleText.Text = $"📄 {title}";
            _originalData = jsonData;

            // 🎯 تبدیل به لیست Key-Value
            var properties = ConvertToKeyValueList(jsonData);
            PropertiesContainer.ItemsSource = properties;
        }

        /// <summary>
        /// تبدیل object به لیست KeyValuePair برای نمایش
        /// </summary>
        private List<KeyValuePair<string, string>> ConvertToKeyValueList(object data)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (data == null)
            {
                result.Add(new KeyValuePair<string, string>("Value", "null"));
                return result;
            }

            // اگه خود data یه Dictionary باشه
            if (data is Dictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    result.Add(new KeyValuePair<string, string>(
                        kvp.Key,
                        FormatValue(kvp.Value)
                    ));
                }
            }
            // اگه یه object معمولی باشه
            else
            {
                result.Add(new KeyValuePair<string, string>(
                    "Value",
                    FormatValue(data)
                ));
            }

            return result;
        }

        /// <summary>
        /// فرمت کردن value برای نمایش بهتر
        /// </summary>
        private string FormatValue(object value)
        {
            if (value == null)
                return "null";

            if (value is string str)
                return $"{str}";

            if (value is bool b)
                return b ? "true" : "false";

            return value.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var properties = ConvertToKeyValueList(_originalData);

                var plainText = string.Join(Environment.NewLine,
                    properties.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

                Clipboard.SetText(plainText);

                MessageBox.Show("✅ Text copied to clipboard!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("❌ Failed to copy!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string GetFullJsonWithChildren()
        {
            return JsonConvert.SerializeObject(_originalData, Formatting.Indented);
        }
    }
}
