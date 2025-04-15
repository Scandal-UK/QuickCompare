// <copyright file="IndexCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class IndexCompareTests
{
    [Fact]
    public async Task Index_MissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        builder.Differences.TableDifferences[tableName]
            .ToString().Should().Contain($"Index: {indexName} does not exist in database 1");
    }

    [Fact]
    public async Task Index_MissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });
        builder.Database2.Tables.Add(tableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        builder.Differences.TableDifferences[tableName]
            .ToString().Should().Contain($"Index: {indexName} does not exist in database 2");
    }

    [Fact]
    public async Task Index_InBothDatabases_IsNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.IsDifferent.Should().BeFalse();
    }

    [Fact]
    public async Task IndexType_PrimaryKey_MissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName, IsPrimaryKey = true });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName];
        diff.ToString().Should().Contain($"Primary key: {indexName} does not exist in database 1");
    }

    [Fact]
    public async Task IndexType_PrimaryKey_MissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName, IsPrimaryKey = true });
        builder.Database2.Tables.Add(tableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName];
        diff.ToString().Should().Contain($"Primary key: {indexName} does not exist in database 2");
    }

    [Fact]
    public async Task IndexType_UniqueKey_MissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName, IsUniqueKey = true });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName];
        diff.ToString().Should().Contain($"Unique key: {indexName} does not exist in database 1");
    }

    [Fact]
    public async Task IndexType_UniqueKey_MissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var tableName = "Table1";
        var indexName = "Index1";
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName, IsUniqueKey = true });
        builder.Database2.Tables.Add(tableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences[tableName]
            .IndexDifferences.Should().ContainKey(indexName);

        var diff = builder.Differences.TableDifferences[tableName];
        diff.ToString().Should().Contain($"Unique key: {indexName} does not exist in database 2");
    }
}
