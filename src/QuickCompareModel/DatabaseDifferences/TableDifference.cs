namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
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
        public bool HasColumnDifferences
        {
            get
            {
                foreach (var column in ColumnDifferences.Values)
                {
                    if (column.IsDifferent)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether the relation difference set has tracked any differences. </summary>
        public bool HasRelationshipDifferences
        {
            get
            {
                foreach (var relation in RelationshipDifferences.Values)
                {
                    if (relation.IsDifferent)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether the index difference set has tracked any differences. </summary>
        public bool HasIndexDifferences
        {
            get
            {
                foreach (var index in IndexDifferences.Values)
                {
                    if (index.IsDifferent)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether the trigger difference set has tracked any differences. </summary>
        public bool HasTriggerDifferences
        {
            get
            {
                foreach (var trigger in TriggerDifferences.Values)
                {
                    if (trigger.IsDifferent)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether the permission difference set has tracked any differences. </summary>
        public bool HasPermissionDifferences
        {
            get
            {
                foreach (var permission in PermissionDifferences.Values)
                {
                    if (!permission.ExistsInBothDatabases)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether the extended property difference set has tracked any differences. </summary>
        public bool HasExtendedPropertyDifferences
        {
            get
            {
                foreach (var prop in ExtendedPropertyDifferences.Values)
                {
                    if (prop.IsDifferent)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public bool IsDifferent
        {
            get
            {
                return !ExistsInBothDatabases || HasColumnDifferences ||
                    HasRelationshipDifferences || HasIndexDifferences || HasTriggerDifferences ||
                    HasExtendedPropertyDifferences || HasPermissionDifferences;
            }
        }

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
                foreach (var colDiff in ColumnDifferences)
                {
                    if (colDiff.Value.IsDifferent)
                    {
                        sb.Append($"{TabIndent}{colDiff.Key} {colDiff.Value}");
                    }
                }
            }

            if (HasTriggerDifferences)
            {
                foreach (var triggerDiff in TriggerDifferences)
                {
                    if (triggerDiff.Value.IsDifferent)
                    {
                        sb.Append($"{TabIndent}Trigger: {triggerDiff.Key} {triggerDiff.Value}");
                    }
                }
            }

            if (HasIndexDifferences)
            {
                foreach (var indexDiff in IndexDifferences)
                {
                    if (indexDiff.Value.IsDifferent)
                    {
                        sb.Append($"{TabIndent}{indexDiff.Value.ItemType}: {indexDiff.Key} {indexDiff.Value}");
                    }
                }
            }

            if (HasRelationshipDifferences)
            {
                foreach (var relationDiff in RelationshipDifferences)
                {
                    if (relationDiff.Value.IsDifferent)
                    {
                        sb.Append($"{TabIndent}Relation: {relationDiff.Key} {relationDiff.Value}");
                    }
                }
            }

            if (HasExtendedPropertyDifferences)
            {
                foreach (var propDiff in ExtendedPropertyDifferences)
                {
                    if (propDiff.Value.IsDifferent)
                    {
                        sb.Append($"{TabIndent}Extended property: {propDiff.Key} {propDiff.Value}");
                    }
                }
            }

            if (HasPermissionDifferences)
            {
                foreach (var permissionDiff in PermissionDifferences)
                {
                    if (!permissionDiff.Value.ExistsInBothDatabases)
                    {
                        sb.Append($"{TabIndent}Permission: {permissionDiff.Key} {permissionDiff.Value}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
