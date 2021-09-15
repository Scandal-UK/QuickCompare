﻿namespace QuickCompareModel.DatabaseDifferences
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

        /// <summary> Gets a value indicating whether the column difference set has tracked any differences. </summary>
        public bool HasColumnDifferences => ColumnDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether the relation difference set has tracked any differences. </summary>
        public bool HasRelationshipDifferences => RelationshipDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether the index difference set has tracked any differences. </summary>
        public bool HasIndexDifferences => IndexDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether the trigger difference set has tracked any differences. </summary>
        public bool HasTriggerDifferences => TriggerDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether the permission difference set has tracked any differences. </summary>
        public bool HasPermissionDifferences => PermissionDifferences.Values.Any(x => !x.ExistsInBothDatabases);

        /// <summary> Gets a value indicating whether the extended property difference set has tracked any differences. </summary>
        public bool HasExtendedPropertyDifferences => ExtendedPropertyDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public bool IsDifferent => !ExistsInBothDatabases || HasColumnDifferences ||
                    HasRelationshipDifferences || HasIndexDifferences || HasTriggerDifferences ||
                    HasExtendedPropertyDifferences || HasPermissionDifferences;

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

            if (HasColumnDifferences)
            {
                foreach (var colDiff in ColumnDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}[{colDiff.Key}] {colDiff.Value}");
                }
            }

            if (HasTriggerDifferences)
            {
                foreach (var triggerDiff in TriggerDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}Trigger: {triggerDiff.Key} {triggerDiff.Value}");
                }
            }

            if (HasIndexDifferences)
            {
                foreach (var indexDiff in IndexDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}{indexDiff.Value.ItemType}: {indexDiff.Key} {indexDiff.Value}");
                }
            }

            if (HasRelationshipDifferences)
            {
                foreach (var relationDiff in RelationshipDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}Relation: {relationDiff.Key} {relationDiff.Value}");
                }
            }

            if (HasExtendedPropertyDifferences)
            {
                foreach (var propDiff in ExtendedPropertyDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}Extended property: {propDiff.Key} {propDiff.Value}");
                }
            }

            if (HasPermissionDifferences)
            {
                foreach (var permissionDiff in PermissionDifferences.Where(x => !x.Value.ExistsInBothDatabases))
                {
                    sb.Append($"{TabIndent}Permission: {permissionDiff.Key} {permissionDiff.Value}");
                }
            }

            return sb.ToString();
        }
    }
}
