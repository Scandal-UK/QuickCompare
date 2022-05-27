namespace QuickCompare
{
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.Extensions.Options;
    using QuickCompareModel;

    /// <summary>
    /// View model class which uses application properties to persist form input between restarts.
    /// </summary>
    public class QuickCompareContext
    {
        public string ConnectionString1
        {
            get => GetProperty(nameof(ConnectionString1), false);
            set => SetProperty(nameof(ConnectionString1), value);
        }

        public string ConnectionString2
        {
            get => GetProperty(nameof(ConnectionString2), false);
            set => SetProperty(nameof(ConnectionString2), value);
        }

        public bool IgnoreSqlComments
        {
            get => bool.Parse(GetProperty(nameof(IgnoreSqlComments)));
            set => SetProperty(nameof(IgnoreSqlComments), value.ToString());
        }

        public bool CompareColumns
        {
            get => bool.Parse(GetProperty(nameof(CompareColumns)));
            set => SetProperty(nameof(CompareColumns), value.ToString());
        }

        public bool CompareCollation
        {
            get => bool.Parse(GetProperty(nameof(CompareCollation)));
            set => SetProperty(nameof(CompareCollation), value.ToString());
        }

        public bool CompareRelations
        {
            get => bool.Parse(GetProperty(nameof(CompareRelations)));
            set => SetProperty(nameof(CompareRelations), value.ToString());
        }

        public bool CompareObjects
        {
            get => bool.Parse(GetProperty(nameof(CompareObjects)));
            set => SetProperty(nameof(CompareObjects), value.ToString());
        }

        public bool CompareIndexes
        {
            get => bool.Parse(GetProperty(nameof(CompareIndexes)));
            set => SetProperty(nameof(CompareIndexes), value.ToString());
        }

        public bool ComparePermissions
        {
            get => bool.Parse(GetProperty(nameof(ComparePermissions)));
            set => SetProperty(nameof(ComparePermissions), value.ToString());
        }

        public bool CompareProperties
        {
            get => bool.Parse(GetProperty(nameof(CompareProperties)));
            set => SetProperty(nameof(CompareProperties), value.ToString());
        }

        public bool CompareTriggers
        {
            get => bool.Parse(GetProperty(nameof(CompareTriggers)));
            set => SetProperty(nameof(CompareTriggers), value.ToString());
        }

        public bool CompareSynonyms
        {
            get => bool.Parse(GetProperty(nameof(CompareSynonyms)));
            set => SetProperty(nameof(CompareSynonyms), value.ToString());
        }

        public bool CompareUserTypes
        {
            get => bool.Parse(GetProperty(nameof(CompareUserTypes)));
            set => SetProperty(nameof(CompareUserTypes), value.ToString());
        }

        public IOptions<QuickCompareOptions> OptionsFromProperties()
        {
            QuickCompareOptions settings = new()
            {
                ConnectionString1 = this.ConnectionString1,
                ConnectionString2 = this.ConnectionString2,
                IgnoreSqlComments = this.IgnoreSqlComments,
                CompareColumns = this.CompareColumns,
                CompareCollation = this.CompareCollation,
                CompareRelations = this.CompareRelations,
                CompareObjects = this.CompareObjects,
                CompareIndexes = this.CompareIndexes,
                ComparePermissions = this.ComparePermissions,
                CompareProperties = this.CompareProperties,
                CompareTriggers = this.CompareTriggers,
                CompareSynonyms = this.CompareSynonyms,
                CompareUserTypes = this.CompareUserTypes,
            };

            return Options.Create(settings);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        private static string GetProperty(string name, bool isBool = true) => (string)Application.Current.Properties[name] ?? (isBool ? "True" : string.Empty);

        private void SetProperty(string name, string value)
        {
            Application.Current.Properties[name] = value;
            this.OnPropertyChanged(name);
        }
    }
}
