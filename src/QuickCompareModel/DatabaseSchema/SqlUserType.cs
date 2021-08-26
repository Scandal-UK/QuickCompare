namespace QuickCompareModel.DatabaseSchema
{
    public class SqlUserType
    {
        public string CustomTypeName { get; set; }

        public string SchemaName { get; set; }

        public string UnderlyingTypeName { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public int? MaxLength { get; set; }

        public bool IsNullable { get; set; }

        public string CollationName { get; set; }

        public bool IsAssemblyType { get; set; }
    }
}
