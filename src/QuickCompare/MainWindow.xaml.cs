using System.Windows;
using Microsoft.Extensions.Options;
using QuickCompareModel;

namespace QuickCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ConnectionString1.Text = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Database1;Integrated Security=True";
            ConnectionString2.Text = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Database2;Integrated Security=True";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var settings = new QuickCompareOptions
            {
                ConnectionString1 = ConnectionString1.Text,
                ConnectionString2 = ConnectionString2.Text,
            };

            IOptions<QuickCompareOptions> options = Options.Create(settings);

            var builder = new DifferenceBuilder(options);
            builder.BuildDifferences();

            OutputTextBox.AppendText(builder.Differences.ToString());
        }
    }
}
