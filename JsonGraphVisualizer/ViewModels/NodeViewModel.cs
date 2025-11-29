using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using JsonGraphVisualizer.Models;
using JsonGraphVisualizer.Controls;

namespace JsonGraphVisualizer.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private bool _isExpanded = true;
        private Visibility _visibility = Visibility.Visible;

        public JsonNodeModel Model { get; }

        public string Title => Model.Title;
        public NodeType Type => Model.Type;
        public Dictionary<string, object> Properties => Model.Properties;
        public double X => Model.X;
        public double Y => Model.Y;
        public double Width => Model.Width;
        public double Height => Model.Height;

        public bool HasChildren => Model.Children != null && Model.Children.Count > 0;

        public string ExpandCollapseIcon => IsExpanded ? "−" : "+";

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(ExpandCollapseIcon));
                    UpdateChildrenVisibility();
                }
            }
        }

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }
        }

        // 🆕 Command برای نمایش متن کامل
        public ICommand ShowFullTextCommand { get; }

        public NodeViewModel(JsonNodeModel model)
        {
            Model = model;
            ShowFullTextCommand = new RelayCommand<object>(ShowFullText);
        }

        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        private void UpdateChildrenVisibility()
        {
            if (Model.Children == null) return;

            foreach (var child in Model.Children)
            {
                UpdateNodeVisibility(child, IsExpanded);
            }
        }

        private void UpdateNodeVisibility(JsonNodeModel node, bool visible)
        {
            // این متد باید از طریق reference به ViewModel واقعی صدا زده شود
            // در عمل این کار در UpdateEdgeVisibility در UserControl انجام می‌شود
        }

        private void ShowFullText(object parameter)
        {
            if (parameter is KeyValuePair<string, object> kvp)
            {
                string fullText = kvp.Value?.ToString() ?? "null";

                if (fullText.Length > 70)
                {
                    var modal = new TextModal(fullText);
                    modal.ShowDialog();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
