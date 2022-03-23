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
            ReadApplicationPropertiesFromFile();
            InitialiseDefaultProperties();

            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            WriteApplicationPropertiesToFile();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception just occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            MessageBox.Show($"An unhandled exception just occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void ReadApplicationPropertiesFromFile()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = new(filename, FileMode.OpenOrCreate, storage);
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                Properties[keyValue[0]] = keyValue[1];
            }
        }

        private void WriteApplicationPropertiesToFile()
        {
            var storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = new(filename, FileMode.Create, storage);
            using StreamWriter writer = new(stream);
            foreach (string key in this.Properties.Keys)
            {
                writer.WriteLine("{0},{1}", key, this.Properties[key]);
            }
        }

        private void InitialiseDefaultProperties()
        {
            if (!Properties.Contains(nameof(QuickCompareContext.ConnectionString1)))
            {
                Properties[nameof(QuickCompareContext.ConnectionString1)] = string.Empty;
            }

            if (!Properties.Contains(nameof(QuickCompareContext.ConnectionString2)))
            {
                Properties[nameof(QuickCompareContext.ConnectionString2)] = string.Empty;
            }

            if (!Properties.Contains(nameof(QuickCompareContext.IgnoreSqlComments)))
            {
                Properties[nameof(QuickCompareContext.IgnoreSqlComments)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareColumns)))
            {
                Properties[nameof(QuickCompareContext.CompareColumns)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareRelations)))
            {
                Properties[nameof(QuickCompareContext.CompareRelations)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareObjects)))
            {
                Properties[nameof(QuickCompareContext.CompareObjects)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.ComparePermissions)))
            {
                Properties[nameof(QuickCompareContext.ComparePermissions)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareProperties)))
            {
                Properties[nameof(QuickCompareContext.CompareProperties)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareTriggers)))
            {
                Properties[nameof(QuickCompareContext.CompareTriggers)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareSynonyms)))
            {
                Properties[nameof(QuickCompareContext.CompareSynonyms)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareUserTypes)))
            {
                Properties[nameof(QuickCompareContext.CompareUserTypes)] = true.ToString();
            }

            if (!Properties.Contains(nameof(QuickCompareContext.CompareIndexes)))
            {
                Properties[nameof(QuickCompareContext.CompareIndexes)] = true.ToString();
            }
        }
    }
}
