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
                this.OnPropertyChanged(nameof(ConnectionString1));
            }
        }

        public string ConnectionString2
        {
            get => (string)Application.Current.Properties[nameof(ConnectionString2)];
            set
            {
                Application.Current.Properties[nameof(ConnectionString2)] = value;
                this.OnPropertyChanged(nameof(ConnectionString2));
            }
        }

        public bool IgnoreSqlComments
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(IgnoreSqlComments)]);
            set
            {
                Application.Current.Properties[nameof(IgnoreSqlComments)] = value.ToString();
                this.OnPropertyChanged(nameof(IgnoreSqlComments));
            }
        }

        public bool CompareColumns
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareColumns)]);
            set
            {
                Application.Current.Properties[nameof(CompareColumns)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareColumns));
            }
        }

        public bool CompareCollation
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareCollation)]);
            set
            {
                Application.Current.Properties[nameof(CompareCollation)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareCollation));
            }
        }

        public bool CompareRelations
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareRelations)]);
            set
            {
                Application.Current.Properties[nameof(CompareRelations)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareRelations));
            }
        }

        public bool CompareObjects
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareObjects)]);
            set
            {
                Application.Current.Properties[nameof(CompareObjects)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareObjects));
            }
        }

        public bool CompareIndexes
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareIndexes)]);
            set
            {
                Application.Current.Properties[nameof(CompareIndexes)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareIndexes));
            }
        }

        public bool ComparePermissions
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(ComparePermissions)]);
            set
            {
                Application.Current.Properties[nameof(ComparePermissions)] = value.ToString();
                this.OnPropertyChanged(nameof(ComparePermissions));
            }
        }

        public bool CompareProperties
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareProperties)]);
            set
            {
                Application.Current.Properties[nameof(CompareProperties)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareProperties));
            }
        }

        public bool CompareTriggers
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareTriggers)]);
            set
            {
                Application.Current.Properties[nameof(CompareTriggers)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareTriggers));
            }
        }

        public bool CompareSynonyms
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareSynonyms)]);
            set
            {
                Application.Current.Properties[nameof(CompareSynonyms)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareSynonyms));
            }
        }

        public bool CompareUserTypes
        {
            get => bool.Parse((string)Application.Current.Properties[nameof(CompareUserTypes)]);
            set
            {
                Application.Current.Properties[nameof(CompareUserTypes)] = value.ToString();
                this.OnPropertyChanged(nameof(CompareUserTypes));
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
