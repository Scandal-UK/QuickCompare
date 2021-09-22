namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary> Model to represent a complex database element and track the differences across two databases. </summary>
    public class DatabaseObjectDifference : BaseDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseObjectDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public DatabaseObjectDifference(bool existsInDatabase1, bool existsInDatabase2)
            : base(existsInDatabase1, existsInDatabase2)
        {
        }

        /// <summary> Gets or sets the body text of the object in database 1. </summary>
        public string ObjectDefinition1 { get; set; }

        /// <summary> Gets or sets the body text of the object in database 2. </summary>
        public string ObjectDefinition2 { get; set; }

        /// <summary> Set of models to represent extended properties and track the differences across two databases. </summary>
        public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; } = new Dictionary<string, ExtendedPropertyDifference>();

        /// <summary> Set of models to represent permissions and track the differences across two databases. </summary>
        public Dictionary<string, BaseDifference> PermissionDifferences { get; set; } = new Dictionary<string, BaseDifference>();

        /// <summary> Gets a value indicating whether the body text is different. </summary>
        public bool DefinitionsAreDifferent => CleanDefinitionText(ObjectDefinition1, true) != CleanDefinitionText(ObjectDefinition2, true);

        /// <summary> Gets a value indicating whether the permission difference set has tracked any differences. </summary>
        public bool HasPermissionDifferences => PermissionDifferences.Values.Any(x => !x.ExistsInBothDatabases);

        /// <summary> Gets a value indicating whether the extended property difference set has tracked any differences. </summary>
        public bool HasExtendedPropertyDifferences => ExtendedPropertyDifferences.Values.Any(x => x.IsDifferent);

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public override bool IsDifferent => !ExistsInBothDatabases || DefinitionsAreDifferent || HasPermissionDifferences || HasExtendedPropertyDifferences;

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

            if (DefinitionsAreDifferent)
            {
                sb.AppendLine($"{TabIndent}Definitions are different");
            }

            if (HasExtendedPropertyDifferences)
            {
                foreach (var diff in ExtendedPropertyDifferences.Where(x => x.Value.IsDifferent))
                {
                    sb.Append($"{TabIndent}Extended property: [{diff.Key}] {diff.Value}");
                }
            }

            if (HasPermissionDifferences)
            {
                foreach (var diff in PermissionDifferences.Where(x => !x.Value.ExistsInBothDatabases))
                {
                    sb.Append($"{TabIndent}Permission: {diff.Key} {diff.Value}");
                }
            }

            return sb.ToString();
        }
    }
}
