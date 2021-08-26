namespace QuickCompareModel.DatabaseSchema
{
    public static class SchemaNameExtensions
    {
        public static string GetSchemaName(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var parts = input.Split('.');
            if (parts.Length == 2)
            {
                return parts[0].StripSquareBrackets();
            }

            return string.Empty;
        }

        public static string GetObjectName(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var parts = input.Split('.');
            if (parts.Length == 2)
            {
                return parts[1].StripSquareBrackets();
            }

            return string.Empty;
        }

        public static string PrependSchemaName(this string objectName, string schemaName) =>
            string.IsNullOrEmpty(schemaName) ? objectName : $"[{schemaName}].[{objectName}]";

        public static string StripSquareBrackets(this string input) =>
            input
                .Replace("[", string.Empty)
                .Replace("]", string.Empty);
    }
}
