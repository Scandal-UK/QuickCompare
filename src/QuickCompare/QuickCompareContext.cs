namespace QuickCompare
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// View model class which uses application properties to persist form input between restarts.
    /// </summary>
    public class QuickCompareContext
    {
        public string ConnectionString1
        {
            get => (string)Application.Current.Properties[nameof(ConnectionString1)];
            set
            {
                Application.Current.Properties[nameof(ConnectionString1)] = value;
                OnPropertyChanged(nameof(ConnectionString1));
            }
        }

        public string ConnectionString2
        {
            get => (string)Application.Current.Properties[nameof(ConnectionString2)];
            set
            {
                Application.Current.Properties[nameof(ConnectionString2)] = value;
                OnPropertyChanged(nameof(ConnectionString2));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs e = new(propertyName);
                handler(this, e);
            }
        }
    }
}
