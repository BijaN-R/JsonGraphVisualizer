using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JsonGraphVisualizer.ViewModels
{
    public class JsonGraphViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NodeViewModel> Nodes { get; set; }
        public ObservableCollection<EdgeViewModel> Edges { get; set; }

        public JsonGraphViewModel()
        {
            Nodes = new ObservableCollection<NodeViewModel>();
            Edges = new ObservableCollection<EdgeViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    } 
}
