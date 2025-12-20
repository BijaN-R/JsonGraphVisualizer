using System.ComponentModel;

namespace JsonGraphVisualizer.ViewModels
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        private bool _isSearchMatch = false;

        public string Key { get; set; }
        public object Value { get; set; }

        public bool IsSearchMatch
        {
            get => _isSearchMatch;
            set
            {
                if (_isSearchMatch != value)
                {
                    _isSearchMatch = value;
                    OnPropertyChanged(nameof(IsSearchMatch));
                }
            }
        }

        private bool _isHighlighted = false;
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public PropertyViewModel(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
