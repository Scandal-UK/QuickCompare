namespace QuickCompareModel.DatabaseDifferences
{
    using System.Text.RegularExpressions;

    /// <summary> Model to represent the most basic element and track the existence across two databases. </summary>
    public class BaseDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="BaseDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public BaseDifference(bool existsInDatabase1, bool existsInDatabase2)
        {
            ExistsInDatabase1 = existsInDatabase1;
            ExistsInDatabase2 = existsInDatabase2;
        }

        /// <summary> Gets or sets a value indicating whether the item exists in database 1. </summary>
        public bool ExistsInDatabase1 { get; set; }

        /// <summary> Gets or sets a value indicating whether the item exists in database 2. </summary>
        public bool ExistsInDatabase2 { get; set; }

        /// <summary> Gets a value indicating whether the item exists in both databases. </summary>
        public bool ExistsInBothDatabases => ExistsInDatabase1 && ExistsInDatabase2;

        /// <summary> Gets a value indicating whether there are any differences. </summary>
        public virtual bool IsDifferent => !ExistsInBothDatabases;

        /// <summary> Gets a text description of the difference or returns an empty string if no difference is detected. </summary>
        public override string ToString() => ExistsInBothDatabases ? string.Empty : $"does not exist in database {(ExistsInDatabase1 ? 2 : 1)}\r\n";

        protected const string TabIndent = "     ";

        /// <summary> A helper method to trim whitespace and comments from text to reduce false-positive results. </summary>
        /// <param name="definition">Text to modify.</param>
        /// <param name="stripWhiteSpace">Value indicating where to reduce all whitespace to a single character.</param>
        /// <returns>Modified text ready for comparison.</returns>
        public static string CleanDefinitionText(string definition, bool stripWhiteSpace)
        {
            if (string.IsNullOrWhiteSpace(definition))
            {
                return string.Empty;
            }

            definition = StripMultiLineComments(definition);
            definition = StripInlineComments(definition);

            if (stripWhiteSpace)
            {
                definition = StripCommaWhitespace(definition);
                definition = NormaliseCommas(definition);
                definition = ReduceWhitespaceToSingleCharacter(definition);
            }

            return definition.Trim();
        }

        private static string StripMultiLineComments(string input) => Regex.Replace(input, @"/\*[^*]*\*+([^/*][^*]*\*+)*/", " ");

        private static string StripInlineComments(string input) => Regex.Replace(input, @"(--(.*|[\r\n]))", string.Empty);

        private static string StripCommaWhitespace(string input) => Regex.Replace(input, @"\s*,\s*", ",");

        private static string NormaliseCommas(string input) => Regex.Replace(input, @"[,]", ", ");

        private static string ReduceWhitespaceToSingleCharacter(string input) => Regex.Replace(input, @"[\s]{2,}", " ");
    }
}
