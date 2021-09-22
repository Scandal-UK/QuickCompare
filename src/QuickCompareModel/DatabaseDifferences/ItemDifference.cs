namespace QuickCompareModel.DatabaseDifferences
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Model to represent an element that belongs to a table.
    /// </summary>
    public class ItemDifference : BaseDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ItemDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public ItemDifference(bool existsInDatabase1, bool existsInDatabase2)
            : base(existsInDatabase1, existsInDatabase2)
        {
        }

        /// <summary> Gets a list of tracked difference strings. </summary>
        public List<string> Differences { get; set; } = new List<string>();

        /// <summary> Gets the item type. </summary>
        public string ItemType { get; set; }

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public override bool IsDifferent => !ExistsInBothDatabases || Differences.Count > 0;

        /// <summary> Gets a text description of the <see cref="Differences"/> or returns an empty string if no difference is detected. </summary>
        public string DifferenceList()
        {
            var sb = new StringBuilder();

            if (Differences.Count == 1)
            {
                sb.AppendLine(Differences.Single());
            }
            else if (Differences.Count > 1)
            {
                sb.Append($"\r\n{TabIndent} - ");
                sb.AppendLine(string.Join($"\r\n{TabIndent} - ", Differences.ToArray()));
            }
            else
            {
                sb.Append("\r\n"); // (only ItemWithPropertiesDifference has output)
            }

            return sb.ToString();
        }

        /// <summary> Gets a text description of the differences or returns an empty string if no difference is detected. </summary>
        public override string ToString() => IsDifferent
            ? !ExistsInBothDatabases ? base.ToString() : DifferenceList()
            : string.Empty;
    }
}
