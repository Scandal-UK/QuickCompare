namespace QuickCompare
{
    using System.Collections.Generic;
    using System.Windows;

    public static class QuickCompareContext
    {
        public static string ConnectionString1
        {
            get
            {
                return (string)Application.Current.Properties[nameof(ConnectionString1)];
            }
            set
            {
                Application.Current.Properties[nameof(ConnectionString1)] = value;
            }
        }

        public static string ConnectionString2
        {
            get
            {
                return (string)Application.Current.Properties[nameof(ConnectionString2)];
            }
            set
            {
                Application.Current.Properties[nameof(ConnectionString2)] = value;
            }
        }

        public static string OutputReportText { get; set; }

        public static List<string> ComboDataSource { get; set; }

        public static string ComboSelectedItem { get; set; }

        public static string DefinitionText1 { get; set; }

        public static string DefinitionText2 { get; set; }
    }
}
