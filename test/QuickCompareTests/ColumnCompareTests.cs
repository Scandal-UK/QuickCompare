// <copyright file="ColumnCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class ColumnCompareTests
{
    [Fact]
    public async Task ColumnMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var columnName = "Column1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .ColumnDifferences.Should().ContainKey(columnName);

        var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();

        builder.Differences.TableDifferences[tableName]
            .ToString().Should().Contain($"Column: {columnName} does not exist in database 1");
    }

    [Fact]
    public async Task ColumnMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var columnName = "Column1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });
        builder.Database2.Tables.Add(tableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .ColumnDifferences.Should().ContainKey(columnName);

        var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();

        builder.Differences.TableDifferences[tableName]
            .ToString().Should().Contain($"Column: {columnName} does not exist in database 2");
    }

    [Fact]
    public async Task ColumnsInBothDatabases_AreNotReported()
    {
        // Arrange
        var tableName = "Table1";
        var columnName = "Column1";
        var builder = TestHelper.GetBuilderWithSingleTable(tableName, columnName);

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .ColumnDifferences.Should().ContainKey(columnName);

        var diff = builder.Differences.TableDifferences[tableName].ColumnDifferences[columnName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void OrdinalPosition_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", OrdinalPosition = 1 }, "ordinal position")
            .Should().BeTrue();

    [Fact]
    public void ColumnDefault_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", ColumnDefault = "foobar" }, "default value")
            .Should().BeTrue();

    [Fact]
    public void IsNullable_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsNullable = true }, "allowed null")
            .Should().BeTrue();

    [Fact]
    public void DataType_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DataType = "foobar" }, "data type")
            .Should().BeTrue();

    [Fact]
    public void CharacterMaxLength_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterMaximumLength = 10 }, "max length")
            .Should().BeTrue();

    [Fact]
    public void CharacterOctetLength_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterOctetLength = 10 }, "character octet length")
            .Should().BeTrue();

    [Fact]
    public void NumericPrecision_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericPrecision = 2 }, "numeric precision")
            .Should().BeTrue();

    [Fact]
    public void NumericPrecisionRadix_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericPrecisionRadix = 2 }, "numeric precision radix")
            .Should().BeTrue();

    [Fact]
    public void NumericScale_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", NumericScale = 2 }, "numeric scale")
            .Should().BeTrue();

    [Fact]
    public void DateTimePrecision_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DatetimePrecision = 2 }, "datetime precision")
            .Should().BeTrue();

    [Fact]
    public void CharacterSetName_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CharacterSetName = "foobar" }, "character set")
            .Should().BeTrue();

    [Fact]
    public void CollationName_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", CollationName = "foobar" }, "collation")
            .Should().BeTrue();

    [Fact]
    public void DomainSchema_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DomainSchema = "foobar" }, "custom datatype")
            .Should().BeTrue();

    [Fact]
    public void DomainName_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", DomainName = "foobar" }, "custom datatype")
            .Should().BeTrue();

    [Fact]
    public void IsFullTextIndexed_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsFullTextIndexed = true }, "full-text indexed")
            .Should().BeTrue();

    [Fact]
    public void IsComputed_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsComputed = true }, "computed")
            .Should().BeTrue();

    [Fact]
    public void IsIdentity_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "an identity")
            .Should().BeTrue();

    [Fact]
    public void IdentitySeed_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true, IdentitySeed = 1 }, new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "identity seed")
            .Should().BeTrue();

    [Fact]
    public void IdentityIncrement_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true, IdentityIncrement = 1 }, new SqlColumnDetail { ColumnName = "Column1", IsIdentity = true }, "identity increment")
            .Should().BeTrue();

    [Fact]
    public void IsSparse_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsSparse = true }, "sparse")
            .Should().BeTrue();

    [Fact]
    public void IsColumnSet_Difference_IsReported() =>
        ComparisonResultContainsValue(new SqlColumnDetail { ColumnName = "Column1", IsColumnSet = true }, "column-set")
            .Should().BeTrue();

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
