namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary> Model to represent the table element and track the differences across two databases. </summary>
    public class TableDifference : BaseDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TableDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public TableDifference(bool existsInDatabase1, bool existsInDatabase2)
            : base(existsInDatabase1, existsInDatabase2)
        {
        }

        /// <summary> Set of models to represent columns and track the differences across two databases. </summary>
        public Dictionary<string, ItemWithPropertiesDifference> ColumnDifferences { get; set; }
            = new Dictionary<string, ItemWithPropertiesDifference>();

        /// <summary> Set of models to represent columns and track the differences across two databases. </summary>
        public Dictionary<string, ItemDifference> RelationshipDifferences { get; set; }
            = new Dictionary<string, ItemDifference>();

        /// <summary> Set of models to represent indexes and track the differences across two databases. </summary>
        public Dictionary<string, ItemWithPropertiesDifference> IndexDifferences { get; set; }
            = new Dictionary<string, ItemWithPropertiesDifference>();

        /// <summary> Set of models to represent triggers and track the differences across two databases. </summary>
        public Dictionary<string, ItemDifference> TriggerDifferences { get; set; }
            = new Dictionary<string, ItemDifference>();

        /// <summary> Set of models to represent extended properties and track the differences across two databases. </summary>
        public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; }
            = new Dictionary<string, ExtendedPropertyDifference>();

        /// <summary> Set of models to represent permissions and track the differences across two databases. </summary>
        public Dictionary<string, BaseDifference> PermissionDifferences { get; set; }
            = new Dictionary<string, BaseDifference>();

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public override bool IsDifferent =>
            !ExistsInBothDatabases ||
            ColumnDifferences.Values.Any(x => x.IsDifferent) ||
            RelationshipDifferences.Values.Any(x => x.IsDifferent) ||
            IndexDifferences.Values.Any(x => x.IsDifferent) ||
            TriggerDifferences.Values.Any(x => x.IsDifferent) ||
            PermissionDifferences.Values.Any(x => x.IsDifferent) ||
            ExtendedPropertyDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a text description of the difference or returns an empty string if no difference is detected. </summary>
        public override string ToString()
        {
            if (!IsDifferent)
            {
                return string.Empty;
            }

            if (!ExistsInBothDatabases)
            {
                return base.ToString();
            }

            var sb = new StringBuilder("\r\n");

            sb.Append(GetSectionDifferenceOutput(ColumnDifferences, "Column"));
            sb.Append(GetSectionDifferenceOutput(TriggerDifferences, "Trigger"));
            sb.Append(GetSectionDifferenceOutput(RelationshipDifferences, "Relation"));
            sb.Append(GetSectionDifferenceOutput(ExtendedPropertyDifferences, "Extended property"));
            sb.Append(GetSectionDifferenceOutput(PermissionDifferences, "Permission"));

            foreach (var indexDiff in IndexDifferences.Where(x => x.Value.IsDifferent))
            {
                sb.Append($"{TabIndent}{indexDiff.Value.ItemType}: {indexDiff.Key} {indexDiff.Value}");
            }

            return sb.ToString();
        }

        private static string GetSectionDifferenceOutput<T>(Dictionary<string, T> source, string name)
            where T : BaseDifference
        {
            var section = new StringBuilder();

            foreach (var prop in source.Where(x => x.Value.IsDifferent))
            {
                section.Append($"{TabIndent}{name}: {prop.Key} {prop.Value}");
            }

            return section.ToString();
        }
    }
}
