using System.Windows.Forms.Integration;
using JsonGraphVisualizer.ViewModels;
using JsonGraphVisualizer.Views;
using System.Windows.Media;

namespace ShowJson
{
    public partial class Form1 : Form
    {
        private JsonGraphVisualizerControl graphControl;
        public Form1()
        {
            InitializeComponent();
            InitializeWpfGraph();
        }
        private void InitializeWpfGraph()
        {
            ElementHost host = new ElementHost();
            host.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            host.Location = new Point(0, 80);
            host.Height = this.Height - 119;
            host.Width = this.Width;

            try
            {
                graphControl = new JsonGraphVisualizerControl();

                host.Child = graphControl;

                this.Controls.Add(host);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in loading WPF: {ex.Message}\n" +
                                "Does App.xaml in WPF project have any specific style?");
            }
        }

        private void btnLoadJson_Click(object sender, EventArgs e)
        {
            string jsonText = txtJsonInput.Text;
            graphControl.JsonData = "";
            graphControl.JsonData = jsonText;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //graphControl.ComponentBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 243, 243));
        }
    }
}
