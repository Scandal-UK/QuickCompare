// <copyright file="ColumnCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    /// <summary>
    /// Tests for the comparison of two sets of column differences.
    /// </summary>
    public class ColumnCompareTests
    {
        /// <summary> Column missing from database 1 is reported. </summary>
        [Fact]
        public void ColumnMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var columnName = "Column1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .ColumnDifferences.Should().ContainKey(columnName);

            var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();

            builder.Differences.TableDifferences[tableName]
                .ToString().Should().Contain($"Column: {columnName} does not exist in database 1");
        }

        /// <summary> Column missing from database 2 is reported. </summary>
        [Fact]
        public void ColumnMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var columnName = "Column1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database1.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });
            builder.Database2.Tables.Add(tableName, new SqlTable());

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .ColumnDifferences.Should().ContainKey(columnName);

            var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();

            builder.Differences.TableDifferences[tableName]
                .ToString().Should().Contain($"Column: {columnName} does not exist in database 2");
        }

        /// <summary> Columns in both databases are not reported. </summary>
        [Fact]
        public void ColumnsInBothDatabases_AreNotReported()
        {
            // Arrange
            var tableName = "Table1";
            var columnName = "Column1";
            var builder = TestHelper.GetBuilderWithSingleTable(tableName, columnName);

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .ColumnDifferences.Should().ContainKey(columnName);

            var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }

        /// <summary> Ordinal position difference is reported. </summary>
        [Fact]
        public void OrdinalPosition_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", OrdinalPosition = 1 }, "ordinal position")
                .Should().BeTrue();

        /// <summary> Column default difference is reported. </summary>
        [Fact]
        public void ColumnDefault_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", ColumnDefault = "foobar" }, "default value")
                .Should().BeTrue();

        /// <summary> Is nullable difference is reported. </summary>
        [Fact]
        public void IsNullable_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsNullable = true }, "allowed null")
                .Should().BeTrue();

        /// <summary> Datatype difference is reported. </summary>
        [Fact]
        public void DataType_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DataType = "foobar" }, "data type")
                .Should().BeTrue();

        /// <summary> Character max length difference is reported. </summary>
        [Fact]
        public void CharacterMaxLength_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterMaximumLength = 10 }, "max length")
                .Should().BeTrue();

        /// <summary> Character octest length difference is reported. </summary>
        [Fact]
        public void CharacterOctetLength_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterOctetLength = 10 }, "character octet length")
                .Should().BeTrue();

        /// <summary> Numeric precision difference is reported. </summary>
        [Fact]
        public void NumericPrecision_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericPrecision = 2 }, "numeric precision")
                .Should().BeTrue();

        /// <summary> Numeric precision radic difference is reported. </summary>
        [Fact]
        public void NumericPrecisionRadix_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericPrecisionRadix = 2 }, "numeric precision radix")
                .Should().BeTrue();

        /// <summary> Numeric scale difference is reported. </summary>
        [Fact]
        public void NumericScale_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericScale = 2 }, "numeric scale")
                .Should().BeTrue();

        /// <summary> Datetime precision difference is reported. </summary>
        [Fact]
        public void DateTimePrecision_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DatetimePrecision = 2 }, "datetime precision")
                .Should().BeTrue();

        /// <summary> Character set name difference is reported. </summary>
        [Fact]
        public void CharacterSetName_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterSetName = "foobar" }, "character set")
                .Should().BeTrue();

        /// <summary> Collation name difference is reported. </summary>
        [Fact]
        public void CollationName_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CollationName = "foobar" }, "collation")
                .Should().BeTrue();

        /// <summary> Domain schema difference is reported. </summary>
        [Fact]
        public void DomainSchema_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DomainSchema = "foobar" }, "custom datatype")
                .Should().BeTrue();

        /// <summary> Domain name difference is reported. </summary>
        [Fact]
        public void DomainName_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DomainName = "foobar" }, "custom datatype")
                .Should().BeTrue();

        /// <summary> Is full text indexed difference reported. </summary>
        [Fact]
        public void IsFullTextIndexed_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsFullTextIndexed = true }, "full-text indexed")
                .Should().BeTrue();

        /// <summary> Is computed difference reported. </summary>
        [Fact]
        public void IsComputed_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsComputed = true }, "computed")
                .Should().BeTrue();

        /// <summary> Is identity difference reported. </summary>
        [Fact]
        public void IsIdentity_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "an identity")
                .Should().BeTrue();

        /// <summary> Identity seed difference is reported. </summary>
        [Fact]
        public void IdentitySeed_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true, IdentitySeed = 1 }, new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "identity seed")
                .Should().BeTrue();

        /// <summary> Identity increment difference is reported. </summary>
        [Fact]
        public void IdentityIncrement_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true, IdentityIncrement = 1 }, new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "identity increment")
                .Should().BeTrue();

        /// <summary> Is sparse difference reported. </summary>
        [Fact]
        public void IsSparse_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsSparse = true }, "sparse")
                .Should().BeTrue();

        /// <summary> Is column-set difference reported. </summary>
        [Fact]
        public void IsColumnSet_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsColumnSet = true }, "column-set")
                .Should().BeTrue();

        /// <summary> Is row-guid difference reported. </summary>
        [Fact]
        public void IsRowGuid_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsRowGuid = true }, "row-guid")
                .Should().BeTrue();

        private static bool ComparisonResultContainsValue(SqlColumnDetail columnDetails, string value) =>
            ComparisonResultContainsValue(columnDetails, new SqlColumnDetail { ColumnName = "Column1" }, value);

        private static bool ComparisonResultContainsValue(SqlColumnDetail column1Details, SqlColumnDetail column2Details, string value)
        {
            // Arrange
            var tableName = "Table1";
            var columnName = "Column1";
            var builder = TestHelper.GetBuilderWithSingleTable(tableName, columnName);

            builder.Database1.Tables[tableName].ColumnDetails[0] = column1Details;
            builder.Database2.Tables[tableName].ColumnDetails[0] = column2Details;

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
            return diff.ToString().Contains(value);
        }
    }
}
