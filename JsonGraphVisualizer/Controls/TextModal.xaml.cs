using System.Windows;
using System.Windows.Controls;

namespace JsonGraphVisualizer.Controls
{
    public partial class TextModal : Window
    {
        public TextModal(string fullText)
        {
            InitializeComponent();
            TextContent.Text = fullText;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextContent.Text))
            {
                Clipboard.SetText(TextContent.Text);
                MessageBox.Show("Text copied to clipboard!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
