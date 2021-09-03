using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Options;
using QuickCompareModel;

namespace QuickCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, (string, string)> definitionDifferences;

        public MainWindow()
        {
            InitializeComponent();

            ConnectionString1.Text = (string)Application.Current.Properties[nameof(ConnectionString1)];
            ConnectionString2.Text = (string)Application.Current.Properties[nameof(ConnectionString2)];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                ClearOutput();
            }

            SetApplicationState();

            var builder = new DifferenceBuilder(GetOptions());
            builder.BuildDifferences();

            definitionDifferences = builder.DefinitionDifferences;

            OutputTextBox.AppendText(builder.Differences.ToString());
            ComboBox1.ItemsSource = definitionDifferences.Keys;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var definitionDifference = definitionDifferences[(string)e.AddedItems[0]];
                DiffViewer1.OldText = definitionDifference.Item1;
                DiffViewer1.NewText = definitionDifference.Item2;
            }
        }

        private IOptions<QuickCompareOptions> GetOptions()
        {
            var settings = new QuickCompareOptions
            {
                ConnectionString1 = ConnectionString1.Text,
                ConnectionString2 = ConnectionString2.Text,
            };

            return Options.Create(settings);
        }

        private void SetApplicationState()
        {
            Application.Current.Properties[nameof(ConnectionString1)] = ConnectionString1.Text;
            Application.Current.Properties[nameof(ConnectionString2)] = ConnectionString2.Text;
        }

        private void ClearOutput()
        {
            OutputTextBox.Clear();

            ComboBox1.SelectedItem = null;
            DiffViewer1.OldText = string.Empty;
            DiffViewer1.NewText = string.Empty;
        }
    }
}
