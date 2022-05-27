namespace QuickCompare
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using QuickCompareModel;

    /// <summary> Interaction logic for the main window. </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void ButtonRunComparison_Click(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
            this.ToggleForm(false);

            try
            {
                var context = (QuickCompareContext)this.FindResource(nameof(QuickCompareContext));
                DifferenceBuilder builder = new(context.OptionsFromProperties());
                builder.ComparisonStatusChanged += this.Comparison_StatusChanged;

                await builder.BuildDifferencesAsync();

                this.OutputTextBox.Text = builder.Differences.ToString();
                this.ComboBoxDefinitions.ItemsSource = builder.DefinitionDifferences;
            }
            finally
            {
                this.ToggleForm(true);
            }
        }

        private void Comparison_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.DatabaseInstance)
            {
                case DatabaseInstance.Database1:
                    this.StatusBarDatabase1.Content = $"Database 1: {e.StatusMessage}";
                    break;
                case DatabaseInstance.Database2:
                    this.StatusBarDatabase2.Content = $"Database 2: {e.StatusMessage}";
                    break;
                default:
                    this.StatusBarDatabase1.Content = e.StatusMessage;
                    this.StatusBarDatabase2.Content = string.Empty;
                    break;
            }
        }

        private void ComboBoxDefinitions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var definitionDifference = (KeyValuePair<string, (string, string)>)e.AddedItems[0];
                this.DiffViewer.OldText = definitionDifference.Value.Item1;
                this.DiffViewer.NewText = definitionDifference.Value.Item2;
            }
        }

        private void ClearOutput()
        {
            if (this.OutputTextBox.Text != string.Empty)
            {
                this.OutputTextBox.Clear();
                this.ComboBoxDefinitions.SelectedItem = null;

                this.DiffViewer.OldText = string.Empty;
                this.DiffViewer.NewText = string.Empty;

                this.StatusBarDatabase1.Content = string.Empty;
                this.StatusBarDatabase2.Content = string.Empty;
            }

            // Temporary workaround for caret getting stuck on Enter & to trigger property-changed event on Enter:
            this.ButtonRunComparison.Focus();
        }

        private void ToggleForm(bool enabled)
        {
            this.StatusBar.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
            this.ControlContainer.IsEnabled = enabled;
        }
    }
}
