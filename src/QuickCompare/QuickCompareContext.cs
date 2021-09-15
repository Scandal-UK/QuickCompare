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

        public bool IgnoreSqlComments
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(IgnoreSqlComments)]);
            set
            {
                Application.Current.Properties[nameof(IgnoreSqlComments)] = value.ToString();
                OnPropertyChanged(nameof(IgnoreSqlComments));
            }
        }

        public bool CompareColumns
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareColumns)]);
            set
            {
                Application.Current.Properties[nameof(CompareColumns)] = value.ToString();
                OnPropertyChanged(nameof(CompareColumns));
            }
        }

        public bool CompareRelations
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareRelations)]);
            set
            {
                Application.Current.Properties[nameof(CompareRelations)] = value.ToString();
                OnPropertyChanged(nameof(CompareRelations));
            }
        }

        public bool CompareObjects
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareObjects)]);
            set
            {
                Application.Current.Properties[nameof(CompareObjects)] = value.ToString();
                OnPropertyChanged(nameof(CompareObjects));
            }
        }

        public bool CompareIndexes
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareIndexes)]);
            set
            {
                Application.Current.Properties[nameof(CompareIndexes)] = value.ToString();
                OnPropertyChanged(nameof(CompareIndexes));
            }
        }

        public bool ComparePermissions
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(ComparePermissions)]);
            set
            {
                Application.Current.Properties[nameof(ComparePermissions)] = value.ToString();
                OnPropertyChanged(nameof(ComparePermissions));
            }
        }

        public bool CompareProperties
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareProperties)]);
            set
            {
                Application.Current.Properties[nameof(CompareProperties)] = value.ToString();
                OnPropertyChanged(nameof(CompareProperties));
            }
        }

        public bool CompareTriggers
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareTriggers)]);
            set
            {
                Application.Current.Properties[nameof(CompareTriggers)] = value.ToString();
                OnPropertyChanged(nameof(CompareTriggers));
            }
        }

        public bool CompareSynonyms
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareSynonyms)]);
            set
            {
                Application.Current.Properties[nameof(CompareSynonyms)] = value.ToString();
                OnPropertyChanged(nameof(CompareSynonyms));
            }
        }

        public bool CompareUserTypes
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareUserTypes)]);
            set
            {
                Application.Current.Properties[nameof(CompareUserTypes)] = value.ToString();
                OnPropertyChanged(nameof(CompareUserTypes));
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
