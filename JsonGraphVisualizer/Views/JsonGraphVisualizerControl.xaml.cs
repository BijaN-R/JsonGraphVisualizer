using System;
using System.Collections.Generic;
using System.Linq;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonGraphVisualizer.Views
{
    public partial class JsonGraphVisualizerControl : UserControl
    {
        #region Dependency Properties

        // 🎨 Color & Appearance
        public static readonly DependencyProperty ComponentBackgroundProperty =
            DependencyProperty.Register("ComponentBackground", typeof(Brush), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(243, 243, 243))));

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

        // 📊 Data
        public static readonly DependencyProperty JsonDataProperty =
            DependencyProperty.Register("JsonData", typeof(string), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(string.Empty, OnJsonDataChanged));

        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof(List<NodeViewModel>), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new List<NodeViewModel>()));

        public static readonly DependencyProperty EdgesProperty =
            DependencyProperty.Register("Edges", typeof(List<EdgeViewModel>), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(new List<EdgeViewModel>()));

        // 🔍 Zoom & Pan limitations
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(0.4));

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

        internal string JsonData
        {
            get => (string)GetValue(JsonDataProperty);
            set
            {
                SetValue(JsonDataProperty, value);
            }
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

        public bool IsSearchVisible
        {
            get => (bool)GetValue(IsSearchVisibleProperty);
            set => SetValue(IsSearchVisibleProperty, value);
        }

        public static readonly DependencyProperty IsSearchVisibleProperty =
            DependencyProperty.Register(
                nameof(IsSearchVisible),
                typeof(bool),
                typeof(JsonGraphVisualizerControl),
                new PropertyMetadata(false));

        #endregion

        #region Private Fields

        private readonly JsonParserService _parserService;
        private readonly LayoutService _layoutService;
        private Point _lastMousePosition;
        private bool _isPanning;
        private Point _originalTranslate;
        private string _originalJsonData;
        private bool _hasDragged;
        private const double DragThreshold = 5.0;

        private List<SearchMatch> _searchResults = new List<SearchMatch>();
        private int _currentSearchIndex = -1;

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

        public string SetJsonData(string jsonData)
        {
            string normalizedJson = JsonParserService.FixNestedJson(jsonData);
            _originalJsonData = normalizedJson;
            JsonData = "";
            JsonData = normalizedJson;
            return normalizedJson;
        }

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
                var rootNodes = _parserService.ParseJson(jsonString);

                _layoutService.CalculateLayout(rootNodes);

                var nodeViewModels = new List<NodeViewModel>();
                var edgeViewModels = new List<EdgeViewModel>();

                CreateViewModels(rootNodes, nodeViewModels, edgeViewModels);

                Nodes = nodeViewModels;
                Edges = edgeViewModels;

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
                nodeVM.IsSearchMatch = false;
                nodeVMs.Add(nodeVM);

                // Edge from parent to this node
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

                // Recursive for children
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
                e.Handled = true;

                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;

                if (ScaleTransform.ScaleX * zoomFactor > MaxZoom ||
                    ScaleTransform.ScaleX * zoomFactor < MinZoom)
                {
                    return;
                }


                // 1. Mouse position in CANVAS
                Point mousePos = e.GetPosition(MainCanvas);

                // 2. Old scale
                Point before = e.GetPosition(MainCanvas);

                ScaleTransform.CenterX = mousePos.X;
                ScaleTransform.CenterY = mousePos.Y;
                // 3. New scale 
                ScaleTransform.ScaleX *= zoomFactor;
                ScaleTransform.ScaleY *= zoomFactor;

                Point after = e.GetPosition(MainCanvas);

                TranslateTransform.X += (after.X - before.X);
                TranslateTransform.Y += (after.Y - before.Y);


                ClampPan();
        }

        private void MainScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // اگر کلیک از دلِ یک Button (یا هر عضوی از آن) آمده، از پَنینگ صرف‌نظر کن
            if (e.OriginalSource is DependencyObject dep &&
                FindVisualParent<Button>(dep) != null)
            {
                // اجازه بده Button خودش رویداد را دریافت کند
                return;
            }

            // یا اگر روی ToastHost کلیک شده
            if (e.OriginalSource is Border b && b.Name == "ToastHost")
                return;

            this.Focus();

            _isPanning = true;
            _hasDragged = false;
            _lastMousePosition = e.GetPosition(MainScrollViewer);
            _originalTranslate = new Point(TranslateTransform.X, TranslateTransform.Y);
            MainScrollViewer.Cursor = Cursors.Hand;
            MainScrollViewer.CaptureMouse();
            e.Handled = true;
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private void MainScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(MainScrollViewer);
                Vector delta = currentPosition - _lastMousePosition;

                if (Math.Abs(delta.X) > DragThreshold || Math.Abs(delta.Y) > DragThreshold)
                {
                    _hasDragged = true;
                }

                TranslateTransform.X = _originalTranslate.X + delta.X;
                TranslateTransform.Y = _originalTranslate.Y + delta.Y;

                //ClampPan();
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
            }
        }

        private void ClampPan()
        {
            // Limit Panning 
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

        private void SetAsRoot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                try
                {
                    // 🔍 Finding NodeViewModel from ContextMenu
                    var contextMenu = (ContextMenu)menuItem.Parent;
                    var placementTarget = contextMenu.PlacementTarget;

                    NodeViewModel nodeVM = null;

                    // Because it could be TextBlock or Border
                    if (placementTarget is FrameworkElement fe)
                    {
                        // Finding NodeViewModel from Visual Tree
                        nodeVM = FindAncestorNodeViewModel(fe);
                    }

                    if (nodeVM != null)
                    {
                        string fullJson = JsonConvert.SerializeObject(
                            nodeVM.Model.RawData,
                            Formatting.Indented);

                        JsonData = fullJson;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Error: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CopyFromHere_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                try
                {
                    // 🔍 Finding NodeViewModel from ContextMenu
                    var contextMenu = (ContextMenu)menuItem.Parent;
                    var placementTarget = contextMenu.PlacementTarget;

                    NodeViewModel nodeVM = null;

                    // Because it could be TextBlock or Border
                    if (placementTarget is FrameworkElement fe)
                    {
                        // Finding NodeViewModel from Visual Tree
                        nodeVM = FindAncestorNodeViewModel(fe);
                    }

                    if (nodeVM != null)
                    {
                        string fullJson = JsonConvert.SerializeObject(
                            nodeVM.Model.RawData,
                            Formatting.Indented);

                        Clipboard.SetText(fullJson);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Error: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private NodeViewModel FindAncestorNodeViewModel(DependencyObject element)
        {
            while (element != null)
            {
                if (element is FrameworkElement fe && fe.DataContext is NodeViewModel nodeVM)
                {
                    return nodeVM;
                }
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private void Node_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_hasDragged && Keyboard.Modifiers != ModifierKeys.Control)
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    if (sender is Border border && border.DataContext is NodeViewModel nodeVM)
                    {
                        e.Handled = true;

                        // 🎯 Filter: Only no child properties
                        var filteredData = FilterPrimitiveProperties(nodeVM.Model);

                        var modal = new JsonModal(nodeVM.Title, filteredData)
                        {
                            KeyColor = KeyTextBrush,
                            ValueColor = ValueTextBrush,
                            Owner = Window.GetWindow(this)
                        };
                        modal.Owner = Window.GetWindow(this);
                        modal.ShowDialog();

                        //e.Handled = true;
                    }
                } 
            }
        }

        private object FilterPrimitiveProperties(JsonNodeModel node)
        {
            // If the node type was Primitive return all RawData 
            if (node.Type == NodeType.Primitive)
            {
                return node.RawData;
            }

            // If not, only return properties with primitive value
            var result = new Dictionary<string, object>();

            foreach (var prop in node.Properties)
            {
                var value = prop.Value;

                // Check to be sure the value is not a complex object/array
                if (value != null &&
                    !(value is JObject) &&
                    !(value is JArray) &&
                    !IsComplexType(value))
                {
                    result[prop.Key] = value;
                }
            }

            return result;
        }

        private bool IsComplexType(object value)
        {
            if (value == null)
                return false;

            var type = value.GetType();

            // If it be string، number، bool، null → Primitive
            return !(type.IsPrimitive ||
                     type == typeof(string) ||
                     type == typeof(decimal) ||
                     type == typeof(DateTime));
        }

        private void Value_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is PropertyViewModel prop)
            {
                var contextMenu = (ContextMenu)menuItem.Parent;
                var textBlock = (TextBlock)contextMenu.PlacementTarget;

                string valueToCopy = prop.Value?.ToString() ?? "null";

                // 🧹 Remove extra quotations
                valueToCopy = valueToCopy.Trim('"');

                string normalizedRaw = Regex.Replace(valueToCopy, @"\s*\r?\n\s*", " ").Trim();
                Clipboard.SetText(normalizedRaw);
                ShowToast($"'%{Truncate(normalizedRaw)}%' copied to clipboard", ToastType.Success);
            }
        }

        private void ResetViewButton_Click(object sender, RoutedEventArgs e)
        {
            JsonData = _originalJsonData;

            ScaleTransform.ScaleX = 1.0;
            ScaleTransform.ScaleY = 1.0;

            var rootNode = Nodes?.FirstOrDefault();

            if (rootNode != null)
            {
                double viewportCenterY = MainScrollViewer.ActualHeight / 2.0;

                TranslateTransform.X = 0;
                TranslateTransform.Y = 20 - rootNode.Y;
            }
            else
            {
                TranslateTransform.X = 0;
                TranslateTransform.Y = 0;
            }
        }

        private async void ShowToast(string text, ToastType type = ToastType.Info, int durationMs = 2000)
        {
            ToastText.Inlines.Clear();
            ToastHost.Background = new SolidColorBrush(GetToastColor(type));

            text = Truncate(text, 250);
            // Regex: It takes strings between two %
            var parts = Regex.Split(text, "(%[^%]+%)");

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                if (part.StartsWith("%") && part.EndsWith("%"))
                {
                    // remove %
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
                    return Color.FromRgb(0, 110, 0); // Green

                case ToastType.Info:
                    return Color.FromRgb(0, 90, 160); // Blue

                case ToastType.Warning:
                    return Color.FromRgb(180, 100, 0); // Orange

                case ToastType.Error:
                    return Color.FromRgb(160, 30, 30); // Dark-Red

                case ToastType.Default:
                    return Color.FromRgb(50, 50, 50); // Gray

                default:
                    return Color.FromRgb(70, 70, 70); // Default color (Gray)
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
                // Show the edge only if both of it's nodes are visible 
                edge.UpdateVisibility();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset Zoom & Pan to the default settings
        /// </summary>
        public void ResetView()
        {
            ResetViewTransform();
        }

        /// <summary>
        /// Expand all nodes
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
        /// Collapse all nodes
        /// </summary>
        public void CollapseAll()
        {
            foreach (var node in Nodes)
            {
                node.IsExpanded = false;
            }
            UpdateEdgeVisibility();
        }

        #region Search_Box
        // 🔎 Search in nodes
        private List<SearchMatch> FindNodesWithValue(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || Nodes == null)
                return new List<SearchMatch>();

            searchTerm = searchTerm.ToLower();
            var candidates = Nodes.ToList();
            var results = new List<SearchMatch>();

            ClearSearchMatches();

            foreach (var node in candidates)
            {
                bool titleMatched = node.Title?.ToLower().Contains(searchTerm) == true;
                if (titleMatched)
                {
                    node.IsSearchMatch = true;
                    results.Add(new SearchMatch { Node = node, Prop = null });
                }

                foreach (var prop in node.Properties)
                {
                    prop.IsSearchMatch = true;
                    var key = (prop.Key ?? "").ToLower();
                    var val = (prop.Value?.ToString() ?? "").ToLower();
                    if (key.Contains(searchTerm) || val.Contains(searchTerm))
                    {
                        results.Add(new SearchMatch { Node = node, Prop = prop });
                    }
                }
            }

            var distinct = results
                .GroupBy(m => new { m.Node, m.Prop })  
                .Select(g => g.First())
                .ToList();

            return distinct;
        }

        private void CenterOnNode(NodeViewModel node)
        {
            double nodeCenterX = node.X + node.Width / 2;
            double nodeCenterY = node.Y + node.Height / 2;
            double targetX = (MainScrollViewer.ActualWidth / 2) - (nodeCenterX * ScaleTransform.ScaleX);
            double targetY = (MainScrollViewer.ActualHeight / 2) - (nodeCenterY * ScaleTransform.ScaleY);

            TranslateTransform.X = targetX;
            TranslateTransform.Y = targetY;
        }

        private void ClearSearchMatches()
        {
            if (Nodes == null) return;
            foreach (var node in Nodes)
            {
                node.IsSearchMatch = false;
                foreach (var prop in node.Properties)
                    prop.IsSearchMatch = false;
            }
        }

        private void ClearAllHighlights()
        {
            if (Nodes == null) return;
            foreach (var node in Nodes)
            {
                node.IsHighlighted = false;
                foreach (var prop in node.Properties)
                    prop.IsHighlighted = false;
            }
        }

        // Highlight all current results
        private void HighlightAllMatches()
        {
            // First remove all highlights
            ClearAllHighlights();

            //Then highlight corresponding nodes and properties to every result
            foreach (var match in _searchResults)
            {
                if (match.Node.IsSearchMatch)
                    match.Node.IsHighlighted = true;
                if (match.Prop != null)
                    match.Prop.IsHighlighted = true;
            }
        }

        #region Search_Events
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _searchResults.Clear();
                _currentSearchIndex = -1;
                MatchCounterText.Text = "";
                ClearAllHighlights();
                return;
            }

            _searchResults = FindNodesWithValue(searchTerm);
            HighlightAllMatches();

            // Set current index and centeralize on the first node
            if (_searchResults.Count > 0)
            {
                _currentSearchIndex = 0;
                MatchCounterText.Text = $"{_currentSearchIndex + 1}/{_searchResults.Count}";
                CenterOnNode(_searchResults[0].Node);
            }
            else
            {
                _currentSearchIndex = -1;
                MatchCounterText.Text = "0/0";
            }
        }

        private void NextMatch_Click(object sender, RoutedEventArgs e)
        {
            if (_searchResults.Count == 0) return;

            _currentSearchIndex = (_currentSearchIndex + 1) % _searchResults.Count;
            MatchCounterText.Text = $"{_currentSearchIndex + 1}/{_searchResults.Count}";
            CenterOnNode(_searchResults[_currentSearchIndex].Node);
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            _searchResults.Clear();
            _currentSearchIndex = -1;
            MatchCounterText.Text = "";
            IsSearchVisible = false;
            ClearSearchMatches();
            ClearAllHighlights() ;
        }

        // ⌨️ Keyboard
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                IsSearchVisible = !IsSearchVisible;
                if (IsSearchVisible)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SearchTextBox.Focus();
                        SearchTextBox.SelectAll();
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && IsSearchVisible)
            {
                ClearSearch_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && IsSearchVisible)
            {
                NextMatch_Click(null, null);
                e.Handled = true;
            }
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClearSearch_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                NextMatch_Click(null, null);
                e.Handled = true;
            }
        }

        #endregion

        #endregion
        #endregion
    }
}
