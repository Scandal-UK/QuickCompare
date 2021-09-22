namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary> Model to hold lists of various differences between two databases. </summary>
    public class Differences
    {
        /// <summary> Gets or sets a friendly name for database 1. </summary>
        public string Database1 { get; set; }

        /// <summary> Gets or sets a friendly name for database 2. </summary>
        public string Database2 { get; set; }

        /// <summary> Gets or sets a set of models to represent extended properties and track the differences across two databases. </summary>
        public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; }
            = new Dictionary<string, ExtendedPropertyDifference>();

        /// <summary> Gets or sets a set of models to represent permissions and track the differences across two databases. </summary>
        public Dictionary<string, BaseDifference> PermissionDifferences { get; set; }
            = new Dictionary<string, BaseDifference>();

        /// <summary> Gets or sets a set of models to represent tables and track the differences across two databases. </summary>
        public Dictionary<string, TableDifference> TableDifferences { get; set; }
            = new Dictionary<string, TableDifference>();

        /// <summary> Gets or sets a set of models to represent user types and track the differences across two databases. </summary>
        public Dictionary<string, ItemDifference> UserTypeDifferences { get; set; }
            = new Dictionary<string, ItemDifference>();

        /// <summary> Gets or sets a set of models to represent functions and track the differences across two databases. </summary>
        public Dictionary<string, DatabaseObjectDifference> FunctionDifferences { get; set; }
            = new Dictionary<string, DatabaseObjectDifference>();

        /// <summary> Gets or sets a set of models to represent stored procedures and track the differences across two databases. </summary>
        public Dictionary<string, DatabaseObjectDifference> StoredProcedureDifferences { get; set; }
            = new Dictionary<string, DatabaseObjectDifference>();

        /// <summary> Gets or sets a set of models to represent views and track the differences across two databases. </summary>
        public Dictionary<string, DatabaseObjectDifference> ViewDifferences { get; set; }
            = new Dictionary<string, DatabaseObjectDifference>();

        /// <summary> Gets or sets a set of models to represent synonyms and track the differences across two databases. </summary>
        public Dictionary<string, DatabaseObjectDifference> SynonymDifferences { get; set; }
            = new Dictionary<string, DatabaseObjectDifference>();

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public bool HasDifferences => ExtendedPropertyDifferences.Count + PermissionDifferences.Count + TableDifferences.Count + 
            FunctionDifferences.Count + StoredProcedureDifferences.Count + ViewDifferences.Count + SynonymDifferences.Count > 0;

        /// <summary> Gets a report of the differences, whether any were detected or not. </summary>
        public override string ToString()
        {
            var output = new StringBuilder("QuickCompare schema comparison result\r\n\r\n");
            output.AppendLine($"Database 1: {Database1}");
            output.AppendLine($"Database 2: {Database2}\r\n");

            if (!HasDifferences)
            {
                output.AppendLine("NO DIFFERENCES HAVE BEEN FOUND");
                return output.ToString();
            }

            output.Append(GetSectionDifferenceOutput(ExtendedPropertyDifferences, "Extended property"));
            output.Append(GetSectionDifferenceOutput(PermissionDifferences, "Permission"));
            output.Append(GetSectionDifferenceOutput(TableDifferences, "Table"));
            output.Append(GetSectionDifferenceOutput(UserTypeDifferences, "User type"));
            output.Append(GetSectionDifferenceOutput(ViewDifferences, "View"));
            output.Append(GetSectionDifferenceOutput(FunctionDifferences, "Function"));
            output.Append(GetSectionDifferenceOutput(StoredProcedureDifferences, "Stored procedure"));
            output.Append(GetSectionDifferenceOutput(SynonymDifferences, "Synonym"));

            return output.ToString();
        }

        private static string GetSectionDifferenceOutput<T>(Dictionary<string, T> source, string name) where T : BaseDifference
        {
            var section = new StringBuilder();
            if (source.Count > 0)
            {
                foreach (var prop in source.Where(x => x.Value.IsDifferent))
                {
                    section.Append($"{name}: {prop.Key} {prop.Value}");
                }

                if (section.Length > 0)
                {
                    section.Insert(0, $"\r\n{name.ToUpperInvariant()} DIFFERENCES\r\n\r\n");
                }
            }

            return section.ToString();
        }
    }
}
