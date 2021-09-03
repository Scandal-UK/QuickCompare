using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace QuickCompare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string filename = "QuickCompare.Settings.txt";

        private void App_Startup(object sender, StartupEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, storage))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                    this.Properties[keyValue[0]] = keyValue[1];
                }
            }

            this.SetDefaultProperties();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Create, storage))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (string key in this.Properties.Keys)
                {
                    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                }
            }
        }

        private void SetDefaultProperties()
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
