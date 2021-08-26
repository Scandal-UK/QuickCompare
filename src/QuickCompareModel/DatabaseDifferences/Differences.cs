namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
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

            var section = new StringBuilder();

            if (ExtendedPropertyDifferences.Count > 0)
            {
                foreach (var prop in ExtendedPropertyDifferences)
                {
                    if (prop.Value.IsDifferent)
                    {
                        section.Append($"Extended property: [{prop.Key}] - {prop.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nEXTENDED PROPERTY DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (PermissionDifferences.Count > 0)
            {
                foreach (var prop in PermissionDifferences)
                {
                    if (!prop.Value.ExistsInBothDatabases)
                    {
                        section.Append($"Permission: {prop.Key} {prop.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nPERMISSION DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (TableDifferences.Count > 0)
            {
                foreach (var tableDifference in TableDifferences)
                {
                    if (tableDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"Table: {tableDifference.Key} {tableDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nTABLE DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (UserTypeDifferences.Count > 0)
            {
                foreach (var userTypeDifference in UserTypeDifferences)
                {
                    if (userTypeDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"User type: {userTypeDifference.Key} {userTypeDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nUSER TYPE DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (ViewDifferences.Count > 0)
            {
                foreach (var viewDifference in ViewDifferences)
                {
                    if (viewDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"View: {viewDifference.Key} {viewDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nVIEW DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (FunctionDifferences.Count > 0)
            {
                foreach (var functionDifference in FunctionDifferences)
                {
                    if (functionDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"Function: {functionDifference.Key} {functionDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nFUNCTION DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (StoredProcedureDifferences.Count > 0)
            {
                foreach (var procedureDifference in StoredProcedureDifferences)
                {
                    if (procedureDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"Stored procedure: {procedureDifference.Key} {procedureDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nSTORED PROCEDURE DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            if (SynonymDifferences.Count > 0)
            {
                foreach (var synonymDifference in SynonymDifferences)
                {
                    if (synonymDifference.Value.IsDifferent)
                    {
                        section.AppendLine($"Synonym: [{synonymDifference.Key}] {synonymDifference.Value}");
                    }
                }

                if (section.Length > 0)
                {
                    output.Append("\r\nSYNONYM DIFFERENCES\r\n\r\n");
                    output.Append(section);
                    section.Length = 0;
                }
            }

            return output.ToString();
        }
    }
}
