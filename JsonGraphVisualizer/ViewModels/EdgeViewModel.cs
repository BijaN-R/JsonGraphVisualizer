using System.ComponentModel;
using System.Windows;

namespace JsonGraphVisualizer.ViewModels
{
    public class EdgeViewModel : INotifyPropertyChanged
    {
        private Visibility _visibility = Visibility.Visible;

        public NodeViewModel Source { get; set; }
        public NodeViewModel Target { get; set; }

        public Point StartPoint
        {
            get
            {
                if (Source == null) return new Point();
                // نقطه اتصال از سمت راست node
                return new Point(
                    Source.X + Source.Width,
                    Source.Y + Source.Height / 2
                );
            }
        }

        public Point EndPoint
        {
            get
            {
                if (Target == null) return new Point();
                // نقطه اتصال به سمت چپ node
                return new Point(
                    Target.X,
                    Target.Y + Target.Height / 2
                );
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

        public void UpdateVisibility()
        {
            // Edge visible است اگر هر دو node visible باشند
            Visibility = (Source?.Visibility == Visibility.Visible &&
                         Target?.Visibility == Visibility.Visible)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
