// Controls/JsonModal.xaml.cs
using System.Windows;
using Newtonsoft.Json;

namespace JsonGraphVisualizer.Controls
{
    public partial class JsonModal : Window
    {
        private string _jsonText;

        public JsonModal(string title, object jsonData)
        {
            InitializeComponent();
            TitleText.Text = $"Node: {title}";

            // فرمت زیبا برای JSON
            _jsonText = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            JsonContent.Text = _jsonText;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_jsonText))
            {
                Clipboard.SetText(_jsonText);
                MessageBox.Show("JSON copied to clipboard!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
