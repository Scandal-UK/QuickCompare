namespace QuickCompare
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Extensions.Options;
    using QuickCompareModel;

    /// <summary> Interaction logic for the main window. </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ControlContainer.IsEnabled = false;
            this.StatusBarText.Visibility = Visibility.Visible;
            this.ClearOutput();

            try
            {
                DifferenceBuilder builder = new(this.CreateOptionsFromFormValues());
                builder.ComparisonStatusChanged += this.Comparison_StatusChanged;

                await builder.BuildDifferencesAsync();

                this.OutputTextBox.Text = builder.Differences.ToString();
                this.ComboBox1.ItemsSource = builder.DefinitionDifferences;
            }
            finally
            {
                this.ControlContainer.IsEnabled = true;
                this.StatusBarText.Content = string.Empty;
                this.StatusBarText.Visibility = Visibility.Collapsed;
            }
        }

        private void Comparison_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            string message = e.DatabaseInstance switch
            {
                DatabaseInstance.Database1 => $"{e.StatusMessage} for database 1",
                DatabaseInstance.Database2 => $"{e.StatusMessage} for database 2",
                _ => e.StatusMessage,
            };

            this.StatusBarText.Content = message;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var definitionDifference = (KeyValuePair<string, (string, string)>)e.AddedItems[0];
                this.DiffViewer1.OldText = definitionDifference.Value.Item1;
                this.DiffViewer1.NewText = definitionDifference.Value.Item2;
            }
        }

        private void ClearOutput()
        {
            if (this.OutputTextBox.Text != string.Empty)
            {
                this.OutputTextBox.Clear();
                this.ComboBox1.SelectedItem = null;
                this.DiffViewer1.OldText = string.Empty;
                this.DiffViewer1.NewText = string.Empty;
            }
        }

        private IOptions<QuickCompareOptions> CreateOptionsFromFormValues()
        {
            QuickCompareOptions settings = new()
            {
                ConnectionString1 = this.ConnectionString1.Text,
                ConnectionString2 = this.ConnectionString2.Text,
                IgnoreSqlComments = this.IgnoreSqlComments.IsChecked ?? false,
                CompareColumns = this.CompareColumns.IsChecked ?? false,
                CompareCollation = this.CompareCollation.IsChecked ?? false,
                CompareRelations = this.CompareRelations.IsChecked ?? false,
                CompareObjects = this.CompareObjects.IsChecked ?? false,
                CompareIndexes = this.CompareIndexes.IsChecked ?? false,
                ComparePermissions = this.ComparePermissions.IsChecked ?? false,
                CompareProperties = this.CompareProperties.IsChecked ?? false,
                CompareTriggers = this.CompareTriggers.IsChecked ?? false,
                CompareSynonyms = this.CompareSynonyms.IsChecked ?? false,
                CompareUserTypes = this.CompareUserTypes.IsChecked ?? false,
            };

            return Options.Create(settings);
        }
    }
}
