namespace QuickCompare
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Extensions.Options;
    using QuickCompareModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                ClearOutput();
            }

            DifferenceBuilder builder = new(CreateOptionsFromFormValues());
            builder.BuildDifferences();

            OutputTextBox.Text = builder.Differences.ToString();
            ComboBox1.ItemsSource = builder.DefinitionDifferences;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var definitionDifference = (KeyValuePair<string, (string, string)>)e.AddedItems[0];
                DiffViewer1.OldText = definitionDifference.Value.Item1;
                DiffViewer1.NewText = definitionDifference.Value.Item2;
            }
        }

        private void ClearOutput()
        {
            OutputTextBox.Clear();
            ComboBox1.SelectedItem = null;
            DiffViewer1.OldText = string.Empty;
            DiffViewer1.NewText = string.Empty;
        }

        private IOptions<QuickCompareOptions> CreateOptionsFromFormValues()
        {
            QuickCompareOptions settings = new()
            {
                ConnectionString1 = ConnectionString1.Text,
                ConnectionString2 = ConnectionString2.Text,
                IgnoreSqlComments = IgnoreSqlComments.IsChecked ?? false,
                CompareColumns = CompareColumns.IsChecked ?? false,
                CompareRelations = CompareRelations.IsChecked ?? false,
                CompareObjects = CompareObjects.IsChecked ?? false,
                CompareIndexes = CompareIndexes.IsChecked ?? false,
                ComparePermissions = ComparePermissions.IsChecked ?? false,
                CompareProperties = CompareProperties.IsChecked ?? false,
                CompareTriggers = CompareTriggers.IsChecked ?? false,
                CompareSynonyms = CompareSynonyms.IsChecked ?? false,
                CompareUserTypes = CompareUserTypes.IsChecked ?? false,
            };

            return Options.Create(settings);
        }
    }
}
