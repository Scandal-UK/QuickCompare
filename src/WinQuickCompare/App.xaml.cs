namespace QuickCompare
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string filename = "QuickCompare.Settings.txt";

        private void App_Startup(object sender, StartupEventArgs e)
        {
            this.ReadApplicationPropertiesFromFile();
            this.InitialiseDefaultProperties();

            AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            this.WriteApplicationPropertiesToFile();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show($"An unhandled exception just occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show($"An unhandled exception just occurred: {((Exception)e.ExceptionObject).Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void ReadApplicationPropertiesFromFile()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = new(filename, FileMode.OpenOrCreate, storage);
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                this.Properties[keyValue[0]] = keyValue[1];
            }
        }

        private void WriteApplicationPropertiesToFile()
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = new(filename, FileMode.Create, storage);
            using StreamWriter writer = new(stream);
            foreach (string key in this.Properties.Keys)
            {
                writer.WriteLine("{0},{1}", key, this.Properties[key]);
            }
        }

        private void InitialiseDefaultProperties()
        {
            if (!this.Properties.Contains(nameof(QuickCompareContext.ConnectionString1)))
            {
                this.Properties[nameof(QuickCompareContext.ConnectionString1)] = string.Empty;
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.ConnectionString2)))
            {
                this.Properties[nameof(QuickCompareContext.ConnectionString2)] = string.Empty;
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.IgnoreSqlComments)))
            {
                this.Properties[nameof(QuickCompareContext.IgnoreSqlComments)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareColumns)))
            {
                this.Properties[nameof(QuickCompareContext.CompareColumns)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareRelations)))
            {
                this.Properties[nameof(QuickCompareContext.CompareRelations)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareObjects)))
            {
                this.Properties[nameof(QuickCompareContext.CompareObjects)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.ComparePermissions)))
            {
                this.Properties[nameof(QuickCompareContext.ComparePermissions)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareProperties)))
            {
                this.Properties[nameof(QuickCompareContext.CompareProperties)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareTriggers)))
            {
                this.Properties[nameof(QuickCompareContext.CompareTriggers)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareSynonyms)))
            {
                this.Properties[nameof(QuickCompareContext.CompareSynonyms)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareUserTypes)))
            {
                this.Properties[nameof(QuickCompareContext.CompareUserTypes)] = true.ToString();
            }

            if (!this.Properties.Contains(nameof(QuickCompareContext.CompareIndexes)))
            {
                this.Properties[nameof(QuickCompareContext.CompareIndexes)] = true.ToString();
            }
        }
    }
}
