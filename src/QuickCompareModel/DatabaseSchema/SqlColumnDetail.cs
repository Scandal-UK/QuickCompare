namespace QuickCompareModel.DatabaseSchema
{
    public class SqlColumnDetail
    {
        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int OrdinalPosition { get; set; }

        public string ColumnDefault { get; set; }

        public bool IsNullable { get; set; }

        public string DataType { get; set; }

        public int? CharacterMaximumLength { get; set; }

        public int? CharacterOctetLength { get; set; }

        public int? NumericPrecision { get; set; }

        public int? NumericPrecisionRadix { get; set; }

        public int? NumericScale { get; set; }

        public int? DatetimePrecision { get; set; }

        public string CharacterSetName { get; set; }

        public string CollationName { get; set; }

        public string DomainSchema { get; set; }

        public string DomainName { get; set; }

        public bool IsFullTextIndexed { get; set; }

        public bool IsComputed { get; set; }

        public bool IsIdentity { get; set; }

        public decimal? IdentitySeed { get; set; }

        public decimal? IdentityIncrement { get; set; }

        public bool IsSparse { get; set; }

        public bool IsColumnSet { get; set; }

        public bool IsRowGuid { get; set; }
    }
}
