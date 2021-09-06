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
                    this.Properties[keyValue[0]] = keyValue[1];
                }
            }

            this.InitialiseDefaultProperties();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new(filename, FileMode.Create, storage))
            using (StreamWriter writer = new(stream))
            {
                foreach (string key in this.Properties.Keys)
                {
                    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                }
            }
        }

        private void InitialiseDefaultProperties()
        {
            if (!this.Properties.Contains("ConnectionString1"))
            {
                this.Properties["ConnectionString1"] = string.Empty;
            }

            if (!this.Properties.Contains("ConnectionString2"))
            {
                this.Properties["ConnectionString2"] = string.Empty;
            }
        }
    }
}
