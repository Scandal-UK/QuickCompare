namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary> Model to represent an element that can have extended properties and belongs to a table. </summary>
    public class ItemWithPropertiesDifference
        : ItemDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ItemWithPropertiesDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public ItemWithPropertiesDifference(bool existsInDatabase1, bool existsInDatabase2)
            : base(existsInDatabase1, existsInDatabase2)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ItemWithPropertiesDifference"/> class
        /// with values determining the type and whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        /// <param name="itemType">String describing the element type.</param>
        public ItemWithPropertiesDifference(bool existsInDatabase1, bool existsInDatabase2, string itemType)
            : base(existsInDatabase1, existsInDatabase2) => this.ItemType = itemType;

        /// <summary> Set of models to represent extended properties and track the differences across two databases. </summary>
        public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; }
            = new Dictionary<string, ExtendedPropertyDifference>();

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public override bool IsDifferent => base.IsDifferent || ExtendedPropertyDifferences.Values.Any(x => x.IsDifferent);

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

            var sb = new StringBuilder(base.ToString());
            sb.Append(GetSubSectionDifferenceOutput(ExtendedPropertyDifferences, "Extended property"));

            return sb.ToString();
        }
    }
}
