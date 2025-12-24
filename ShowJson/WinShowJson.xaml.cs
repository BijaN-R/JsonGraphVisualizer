using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JsonGraphVisualizer.Views;
using JsonVisualizer.Helpers;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace ShowJson
{
    public partial class WinShowJson : Window
    {
        private bool _isInternalUpdate;

        public WinShowJson()
        {
            InitializeComponent();
            Loaded += WinShowJson_Loaded;
        }

        private void WinShowJson_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyTheme();
        }

        private string GetTextFromRichTextBox()
        {
            var textRange = new TextRange(
                txtJsonInput.Document.ContentStart,
                txtJsonInput.Document.ContentEnd);

            return textRange.Text.TrimEnd(); // TrimEnd برای حذف newline اضافی آخر
        }
        private void SetTextToRichTextBox(string text)
        {
            txtJsonInput.Document.Blocks.Clear();
            txtJsonInput.Document.Blocks.Add(new Paragraph(new Run(text)));

            // 🎨 اعمال Highlighting
            _isInternalUpdate = true;
            JsonSyntaxHighlighter.ApplyHighlighting(txtJsonInput);
            _isInternalUpdate = false;
        }

        private void BtnLoadJson_Click(object sender, RoutedEventArgs e)
        {
            string jsonText = GetTextFromRichTextBox();

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                MessageBox.Show("Please enter JSON text.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SetTextToRichTextBox(graphControl.SetJsonData(jsonText));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading JSON: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChbDarkMode_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (btnLoadJson == null) return;
            bool isDark = chbDarkMode.IsChecked == true;

            if (isDark)
            {
                ApplyDarkTheme();
            }
            else
            {
                ApplyLightTheme();
            }
        }

        private void ApplyDarkTheme()
        {
            // 🌙 Dark Theme Colors
            var bgDark = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            var bgInput = new SolidColorBrush(Color.FromRgb(44, 44, 44));
            var fgLight = new SolidColorBrush(Colors.LightGray);
            var splitterBg = new SolidColorBrush(Color.FromRgb(64, 64, 64));

            // Window & Main Grid
            MainGrid.Background = bgDark;

            // TextBox
            txtJsonInput.Background = bgInput;
            txtJsonInput.Foreground = fgLight;
            txtJsonInput.BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60));

            // 🎨 ScrollBar Colors - کلید اصلی!
            Resources["ScrollBarTrackBrush"] = new SolidColorBrush(Color.FromRgb(58, 58, 58));
            Resources["ScrollBarThumbBrush"] = new SolidColorBrush(Color.FromRgb(104, 104, 104));
            Resources["ScrollBarThumbHoverBrush"] = new SolidColorBrush(Color.FromRgb(158, 158, 158));

            // CheckBox
            chbDarkMode.Foreground = fgLight;
            chbGrid.Background = bgInput;

            // Button
            btnLoadJson.Background = new SolidColorBrush(Color.FromRgb(10, 70, 123));
            btnLoadJson.Foreground = fgLight;
            btnLoadJson.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 120, 180));

            // GridSplitter
            gridSplitter.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));

            // Graph Control
            graphControl.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
            graphControl.NodeBackground = new SolidColorBrush(Color.FromRgb(30, 35, 40));
            graphControl.NodeBorderBrush = new SolidColorBrush(Color.FromRgb(60, 65, 70));
        }

        private void ApplyLightTheme()
        {
            // ☀️ Light Theme Colors
            var bgLight = new SolidColorBrush(Colors.White);
            var bgInput = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            var fgDark = new SolidColorBrush(Colors.Black);
            var borderLight = new SolidColorBrush(Color.FromRgb(200, 200, 200));

            // Window & Main Grid
            MainGrid.Background = bgLight;

            // TextBox
            txtJsonInput.Background = bgInput;
            txtJsonInput.Foreground = fgDark;
            txtJsonInput.BorderBrush = borderLight;

            // 🎨 ScrollBar Colors - رنگ‌های روشن
            Resources["ScrollBarTrackBrush"] = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            Resources["ScrollBarThumbBrush"] = new SolidColorBrush(Color.FromRgb(160, 160, 160));
            Resources["ScrollBarThumbHoverBrush"] = new SolidColorBrush(Color.FromRgb(120, 120, 120));

            // CheckBox
            chbDarkMode.Foreground = fgDark;
            chbGrid.Background = bgInput;

            // Button
            btnLoadJson.Background = new SolidColorBrush(Color.FromRgb(190, 190, 190));
            btnLoadJson.Foreground = fgDark;
            btnLoadJson.BorderBrush = borderLight;

            // GridSplitter
            gridSplitter.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));

            // Graph Control - Light theme
            graphControl.Background = new SolidColorBrush(Color.FromRgb(210, 210, 210));
            graphControl.NodeBackground = new SolidColorBrush(Color.FromRgb(30, 35, 40));
            graphControl.NodeBorderBrush = new SolidColorBrush(Color.FromRgb(0, 122, 204));
        }


        private void TxtInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateDocumentPageWidth();
        }

        private void UpdateDocumentPageWidth()
        {
            if (txtJsonInput?.Document == null || txtJsonInput.ActualWidth <= 0)
                return;

            // محاسبه عرض واقعی قابل استفاده
            double padding = txtJsonInput.Padding.Left + txtJsonInput.Padding.Right;
            double border = txtJsonInput.BorderThickness.Left + txtJsonInput.BorderThickness.Right;
            double scrollBarWidth = SystemParameters.VerticalScrollBarWidth;

            double availableWidth = txtJsonInput.ActualWidth - padding - border - scrollBarWidth - 5;

            if (availableWidth > 50) // حداقل عرض منطقی
            {
                txtJsonInput.Document.PageWidth = availableWidth;
            }
        }


        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInternalUpdate)
                return;

            _isInternalUpdate = true;

            JsonSyntaxHighlighter.ApplyHighlighting(txtJsonInput);

            _isInternalUpdate = false;
        }
    }
}
