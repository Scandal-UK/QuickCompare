namespace QuickCompare
{
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string filename = "QuickCompare.Settings.txt";

        private void App_Startup(object sender, StartupEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new(filename, FileMode.OpenOrCreate, storage))
            using (StreamReader reader = new(stream))
            {
                while (!reader.EndOfStream)
                {
                    string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                    Properties[keyValue[0]] = keyValue[1];
                }
            }

            InitialiseDefaultProperties();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception just occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void App_Exit(object sender, ExitEventArgs e)
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
            if (!Properties.Contains("ConnectionString1"))
            {
                Properties["ConnectionString1"] = string.Empty;
            }

            if (!Properties.Contains("ConnectionString2"))
            {
                Properties["ConnectionString2"] = string.Empty;
            }
        }
    }
}
