namespace QuickCompareModel.DatabaseSchema
{
    using System.Collections.Generic;
    using System.Text;

    public class SqlIndex
    {
        public bool IsPrimaryKey { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string IndexName { get; set; }

        public bool Clustered { get; set; }

        public bool Unique { get; set; }

        public bool IsUniqueKey { get; set; }

        public Dictionary<int, KeyValuePair<string, bool>> IndexColumns { get; set; }

        public string FileGroup { get; set; }

        public Dictionary<string, bool> Columns { get; set; } = new Dictionary<string, bool>();

        public Dictionary<string, bool> IncludedColumns { get; set; } = new Dictionary<string, bool>();

        public string FullId => $"[{TableSchema}].[{TableName}].[{IndexName}]";

        public string ColumnsToString => FlagListToString(Columns);

        public string IncludedColumnsToString => FlagListToString(IncludedColumns);

        public string ItemType => !IsPrimaryKey ? IsUniqueKey ? "Unique key" : "Index" : "Primary key";

        public void SetColumnsFromString(string value)
        {
            var columnNames = value.Split(',');
            foreach (var columnName in columnNames)
            {
                if (columnName.IndexOf("(-)") > 0)
                {
                    Columns.Add(columnName.Replace("(-)", "").Trim(), false);
                }
                else
                {
                    Columns.Add(columnName.Trim(), true);
                }
            }
        }

        private static string FlagListToString(Dictionary<string, bool> flagList)
        {
            if (flagList == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var pair in flagList)
            {
                sb.AppendLine($"{pair.Key}, {pair.Value}");
            }

            return sb.ToString();
        }
    }
}
