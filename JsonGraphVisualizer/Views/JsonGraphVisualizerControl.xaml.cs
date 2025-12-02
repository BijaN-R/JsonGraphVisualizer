using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using JsonGraphVisualizer.Controls;
using JsonGraphVisualizer.Models;
using JsonGraphVisualizer.Services;
using JsonGraphVisualizer.ViewModels;
using Newtonsoft.Json.Linq;

namespace JsonGraphVisualizer.Views
{
    public partial class JsonGraphVisualizerControl : UserControl
    {
        #region Dependency Properties

        // 🎨 رنگ‌ها و ظاهر
        public static readonly DependencyProperty ComponentBackgroundProperty =
            DependencyProperty.Register("ComponentBackground", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(30, 30, 30))));

        public static readonly DependencyProperty NodeBackgroundProperty =
            DependencyProperty.Register("NodeBackground", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(45, 45, 48))));

        public static readonly DependencyProperty NodeBorderBrushProperty =
            DependencyProperty.Register("NodeBorderBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 122, 204))));

        public static readonly DependencyProperty EdgeBrushProperty =
            DependencyProperty.Register("EdgeBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(100, 100, 100))));

        public static readonly DependencyProperty ObjectTitleBrushProperty =
            DependencyProperty.Register("ObjectTitleBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(78, 201, 176))));

        public static readonly DependencyProperty ArrayTitleBrushProperty =
            DependencyProperty.Register("ArrayTitleBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 185, 0))));

        public static readonly DependencyProperty KeyTextBrushProperty =
            DependencyProperty.Register("KeyTextBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(94, 185, 235))));

        public static readonly DependencyProperty ValueTextBrushProperty =
            DependencyProperty.Register("ValueTextBrush", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(233, 150, 122))));

        public static readonly DependencyProperty GlobalFontFamilyProperty =
            DependencyProperty.Register("GlobalFontFamily", typeof(FontFamily), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new FontFamily("Consolas")));

        // 📊 داده‌ها
        public static readonly DependencyProperty JsonDataProperty =
            DependencyProperty.Register("JsonData", typeof(string), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(string.Empty, OnJsonDataChanged));

        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof(List<NodeViewModel>), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new List<NodeViewModel>()));

        public static readonly DependencyProperty EdgesProperty =
            DependencyProperty.Register("Edges", typeof(List<EdgeViewModel>), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new List<EdgeViewModel>()));

        // 🔍 Zoom و Pan محدودیت‌ها
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(0.2));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(5.0));

        public static readonly DependencyProperty MaxPanOffsetProperty =
            DependencyProperty.Register("MaxPanOffset", typeof(double), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(150.0));

        #endregion

        #region Property Wrappers

        public Brush ComponentBackground
        {
            get => (Brush)GetValue(ComponentBackgroundProperty);
            set => SetValue(ComponentBackgroundProperty, value);
        }

        public Brush NodeBackground
        {
            get => (Brush)GetValue(NodeBackgroundProperty);
            set => SetValue(NodeBackgroundProperty, value);
        }

        public Brush NodeBorderBrush
        {
            get => (Brush)GetValue(NodeBorderBrushProperty);
            set => SetValue(NodeBorderBrushProperty, value);
        }

        public Brush EdgeBrush
        {
            get => (Brush)GetValue(EdgeBrushProperty);
            set => SetValue(EdgeBrushProperty, value);
        }

        public Brush ObjectTitleBrush
        {
            get => (Brush)GetValue(ObjectTitleBrushProperty);
            set => SetValue(ObjectTitleBrushProperty, value);
        }

        public Brush ArrayTitleBrush
        {
            get => (Brush)GetValue(ArrayTitleBrushProperty);
            set => SetValue(ArrayTitleBrushProperty, value);
        }

        public Brush KeyTextBrush
        {
            get => (Brush)GetValue(KeyTextBrushProperty);
            set => SetValue(KeyTextBrushProperty, value);
        }

        public Brush ValueTextBrush
        {
            get => (Brush)GetValue(ValueTextBrushProperty);
            set => SetValue(ValueTextBrushProperty, value);
        }

        public FontFamily GlobalFontFamily
        {
            get => (FontFamily)GetValue(GlobalFontFamilyProperty);
            set => SetValue(GlobalFontFamilyProperty, value);
        }

        public string JsonData
        {
            get => (string)GetValue(JsonDataProperty);
            set => SetValue(JsonDataProperty, value);
        }

        public List<NodeViewModel> Nodes
        {
            get => (List<NodeViewModel>)GetValue(NodesProperty);
            set => SetValue(NodesProperty, value);
        }

        public List<EdgeViewModel> Edges
        {
            get => (List<EdgeViewModel>)GetValue(EdgesProperty);
            set => SetValue(EdgesProperty, value);
        }

        public double MinZoom
        {
            get => (double)GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        public double MaxZoom
        {
            get => (double)GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        public double MaxPanOffset
        {
            get => (double)GetValue(MaxPanOffsetProperty);
            set => SetValue(MaxPanOffsetProperty, value);
        }

        #endregion

        #region Private Fields

        private readonly JsonParserService _parserService;
        private readonly LayoutService _layoutService;
        private Point _lastMousePosition;
        private bool _isPanning;
        private Point _originalTranslate;

        #endregion

        #region Constructor

        public JsonGraphVisualizerControl()
        {
            InitializeComponent();

            _parserService = new JsonParserService();
            _layoutService = new LayoutService();

            DataContext = this;
        }

        #endregion

        #region JSON Processing

        private static void OnJsonDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JsonGraphVisualizerControl control && e.NewValue is string jsonString)
            {
                control.ProcessJson(jsonString);
            }
        }

        private void ProcessJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Nodes = new List<NodeViewModel>();
                Edges = new List<EdgeViewModel>();
                return;
            }

            try
            {
                // 1️⃣ Parse JSON
                var rootNodes = _parserService.ParseJson(jsonString);

                // 2️⃣ محاسبه Layout
                _layoutService.CalculateLayout(rootNodes);

                // 3️⃣ ایجاد ViewModels
                var nodeViewModels = new List<NodeViewModel>();
                var edgeViewModels = new List<EdgeViewModel>();

                CreateViewModels(rootNodes, nodeViewModels, edgeViewModels);

                Nodes = nodeViewModels;
                Edges = edgeViewModels;

                // 4️⃣ Reset Zoom/Pan
                ResetViewTransform();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateViewModels(
            List<JsonNodeModel> nodes,
            List<NodeViewModel> nodeVMs,
            List<EdgeViewModel> edgeVMs,
            NodeViewModel parent = null)
        {
            foreach (var node in nodes)
            {
                var nodeVM = new NodeViewModel(node);
                nodeVMs.Add(nodeVM);

                // Edge از parent به این node
                if (parent != null)
                {
                    var edge = new EdgeViewModel
                    {
                        Source = parent,
                        Target = nodeVM
                    };
                    edgeVMs.Add(edge);
                    parent.ChildrenViewModels.Add(nodeVM);
                }

                // Recursive برای children
                if (node.Children != null && node.Children.Count > 0)
                {
                    CreateViewModels(node.Children, nodeVMs, edgeVMs, nodeVM);
                }
            }
        }

        #endregion

        #region Pan & Zoom

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;

                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                double newScale = ScaleTransform.ScaleX * zoomFactor;

                // محدود کردن zoom
                newScale = Math.Max(MinZoom, Math.Min(MaxZoom, newScale));

                // نقطه مرکز zoom
                Point mousePos = e.GetPosition(MainCanvas);

                ScaleTransform.ScaleX = newScale;
                ScaleTransform.ScaleY = newScale;

                // تنظیم translate برای zoom از نقطه موس
                double offsetX = mousePos.X * (1 - zoomFactor);
                double offsetY = mousePos.Y * (1 - zoomFactor);

                TranslateTransform.X += offsetX;
                TranslateTransform.Y += offsetY;

                ClampPan();
            }
        }

        private void MainScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _isPanning = true;
                _lastMousePosition = e.GetPosition(MainScrollViewer);
                _originalTranslate = new Point(TranslateTransform.X, TranslateTransform.Y);
                MainScrollViewer.Cursor = Cursors.Hand;
                MainScrollViewer.CaptureMouse();
                e.Handled = true;
            }
        }

        private void MainScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(MainScrollViewer);
                Vector delta = currentPosition - _lastMousePosition;

                TranslateTransform.X = _originalTranslate.X + delta.X;
                TranslateTransform.Y = _originalTranslate.Y + delta.Y;

                ClampPan();
                e.Handled = true;
            }
        }

        private void MainScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                MainScrollViewer.Cursor = Cursors.Arrow;
                MainScrollViewer.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void ClampPan()
        {
            // محدود کردن Pan به 500 پیکسل خارج از محدوده
            double maxOffset = MaxPanOffset;

            TranslateTransform.X = Math.Max(-maxOffset, Math.Min(maxOffset, TranslateTransform.X));
            TranslateTransform.Y = Math.Max(-maxOffset, Math.Min(maxOffset, TranslateTransform.Y));
        }

        private void ResetViewTransform()
        {
            ScaleTransform.ScaleX = 1.0;
            ScaleTransform.ScaleY = 1.0;
            TranslateTransform.X = 0;
            TranslateTransform.Y = 0;
        }

        #endregion

        #region Node Interactions

        private void ExpandCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is NodeViewModel nodeVM)
            {
                e.Handled = true; // جلوگیری از trigger شدن Node_MouseLeftButtonDown

                nodeVM.ToggleExpand();
                UpdateEdgeVisibility();
            }
        }

        private void Title_Click(object sender, MouseButtonEventArgs e)
        {
            // فقط وقتی که Ctrl فشرده نشده (یعنی panning نباشد)
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                if (sender is FrameworkElement fe && fe.DataContext is NodeViewModel nodeVM)
                {
                    e.Handled = true;
                    // نمایش Modal با JSON کامل این node
                    var modal = new JsonModal(nodeVM.Title, nodeVM.Model.RawData);
                    modal.Owner = Window.GetWindow(this);
                    modal.ShowDialog();

                    e.Handled = true;
                }
            }
        }

        private void Value_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                e.Handled = true;

                string raw = null;

                // حالت Property (KV)
                if (fe.DataContext is KeyValuePair<string, object> kvp)
                    raw = kvp.Value?.ToString() ?? "null";

                // حالت Node Primitive
                if (fe.DataContext is NodeViewModel vm)
                    raw = vm.Model.RawData?.ToString() ?? "null";

                if (raw == null) 
                    return;
                
                string normalizedRaw = Regex.Replace(raw, @"\s*\r?\n\s*", " ").Trim();
                Clipboard.SetText(normalizedRaw);
                ShowToast($"'%{Truncate(normalizedRaw)}%' copied to clipboard", ToastType.Success);
            }
        }

        private async void ShowToast(string text, ToastType type = ToastType.Info, int durationMs = 2000)
        {
            ToastText.Inlines.Clear();
            ToastHost.Background = new SolidColorBrush(GetToastColor(type));

            text = Truncate(text, 250);
            // Regex: متن بین دو % را می‌گیرد
            var parts = Regex.Split(text, "(%[^%]+%)");

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                if (part.StartsWith("%") && part.EndsWith("%"))
                {
                    // حذف درصدها
                    string highlighted = part.Substring(1, part.Length - 2);

                    ToastText.Inlines.Add(new Run(highlighted)
                    {
                        Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                        FontWeight = FontWeights.Bold
                    });
                }
                else
                {
                    ToastText.Inlines.Add(new Run(part)
                    {
                        Foreground = Brushes.White
                    });
                }
            }

            ToastHost.Visibility = Visibility.Visible;

            // Fade In
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            ToastHost.BeginAnimation(OpacityProperty, fadeIn);

            // Wait duration
            await Task.Delay(durationMs);

            // Fade Out
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                ToastHost.Visibility = Visibility.Collapsed;
            };
            ToastHost.BeginAnimation(OpacityProperty, fadeOut);
        }

        private Color GetToastColor(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    return Color.FromRgb(0, 110, 0); // سبز

                case ToastType.Info:
                    return Color.FromRgb(0, 90, 160); // آبی

                case ToastType.Warning:
                    return Color.FromRgb(180, 100, 0); // نارنجی

                case ToastType.Error:
                    return Color.FromRgb(160, 30, 30); // قرمز تیره

                case ToastType.Default:
                    return Color.FromRgb(50, 50, 50); // قرمز تیره

                default:
                    return Color.FromRgb(70, 70, 70); // حالت default خاکستری
            }
        }

        private string Truncate(string text, int max = 80)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Length > max
                ? text.Substring(0, max) + "..."
                : text;
        }

        private void UpdateEdgeVisibility()
        {
            foreach (var edge in Edges)
            {
                // Edge نمایش داده شود اگر هر دو node visible باشند
                edge.UpdateVisibility();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// بازنشانی Zoom و Pan به حالت اولیه
        /// </summary>
        public void ResetView()
        {
            ResetViewTransform();
        }

        /// <summary>
        /// Expand کردن تمام node ها
        /// </summary>
        public void ExpandAll()
        {
            foreach (var node in Nodes)
            {
                node.IsExpanded = true;
            }
            UpdateEdgeVisibility();
        }

        /// <summary>
        /// Collapse کردن تمام node ها
        /// </summary>
        public void CollapseAll()
        {
            foreach (var node in Nodes)
            {
                node.IsExpanded = false;
            }
            UpdateEdgeVisibility();
        }

        #endregion
    }
}
