using System.Windows;
using JsonGraphVisualizer.Views;

namespace JsonGraphVisualizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 📝 JSON نمونه برای تست
            JsonInput.Text = @"{
                    ""GeneralCargoList"": [
                      {
                        ""HsCode"": ""84334000"",
                        ""Description"": ""علوفه"",
                      },
                      {
                        ""HsCode"": ""62335000"",
                        ""Description"": ""چای"",
                      },
                      {
                        ""HsCode"": ""35624000"",
                        ""Description"": ""قهوه"",
                      }
                    ],
                    ""BulkList"": [1,2,3,4,5],
                    ""ContainerList"": [],

                    ""WorkflowRemark"": """",
                    ""Owner"": {
                      ""Name"": ""علي مراد زاده"",
                      ""PostalCode"": ""5713878848"",
                      ""Address"": ""بندرعباس"",
                      ""IsCompany"": ""False""
                    }
                  }";
                }

        private void LoadJson_Click(object sender, RoutedEventArgs e)
        {
            RootControl.JsonData = null;
            RootControl.JsonData = JsonInput.Text;
        }
    }
}
