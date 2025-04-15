// <copyright file="TableCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class TableCompareTests
{
    private const string TableName = "[dbo].[Table1]";
    private const string TabIndent = "     ";

    [Fact]
    public async Task TableMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database2.Tables.Add(TableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Contain("does not exist in database 1");
    }

    [Fact]
    public async Task TableMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        diff.ToString().Should().Contain("does not exist in database 2");
    }

    [Fact]
    public async Task TablesInBothDatabases_AreNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();

        diff.IsDifferent.Should().BeFalse();
        diff.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public async Task TablePropertyMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = TableName.GetObjectName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
            TableName = TableName.GetObjectName(),
            TableSchema = TableName.GetSchemaName(),
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences.Count.Should().Be(1);
        diff.ExtendedPropertyDifferences.ContainsKey("Key1").Should().BeTrue();

        diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase1.Should().BeFalse();

        builder.Differences.TableDifferences[TableName]
            .ToString().Should().Contain("Extended property: Key1 does not exist in database 1");
    }

    [Fact]
    public async Task TablePropertyMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = TableName.GetObjectName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
            TableName = TableName.GetObjectName(),
            TableSchema = TableName.GetSchemaName(),
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();

        diff.ExtendedPropertyDifferences.Count.Should().Be(1);
        diff.ExtendedPropertyDifferences.ContainsKey("Key1").Should().BeTrue();

        diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase2.Should().BeFalse();

        builder.Differences.TableDifferences[TableName]
            .ToString().Should().Contain("Extended property: Key1 does not exist in database 2");
    }

    [Fact]
    public async Task TablePropertyDifference_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = TableName.GetObjectName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
            TableName = TableName.GetObjectName(),
            TableSchema = TableName.GetSchemaName(),
        });

        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = TableName.GetObjectName(),
            PropertyName = "Key1",
            PropertyValue = "Value2",
            TableName = TableName.GetObjectName(),
            TableSchema = TableName.GetSchemaName(),
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences.Count.Should().Be(1);

        diff.ExtendedPropertyDifferences.ContainsKey("Key1").Should().BeTrue();
        diff.ExtendedPropertyDifferences["Key1"].ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        diff.ToString().Trim()
            .Should().Be("Extended property: Key1 value is different; [Value1] in database 1, [Value2] in database 2");
    }

    [Fact]
    public async Task TablePermissionMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database2.Permissions.Add(new SqlPermission
        {
            ObjectSchema = TableName.GetSchemaName(),
            ObjectName = TableName.GetObjectName(),
            ObjectType = "USER_TABLE",
            UserName = "foobar",
            PermissionType = "INSERT",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();

        diff.PermissionDifferences.Count.Should().Be(1);
        diff.PermissionDifferences.ContainsKey("[INSERT] DENIED for user: [foobar]").Should().BeTrue();

        builder.Differences.TableDifferences[TableName]
            .ToString().Should().Contain("Permission: [INSERT] DENIED for user: [foobar] does not exist in database 1");
    }

    [Fact]
    public async Task TablePermissionMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database1.Permissions.Add(new SqlPermission
        {
            ObjectSchema = TableName.GetSchemaName(),
            ObjectName = TableName.GetObjectName(),
            ObjectType = "USER_TABLE",
            UserName = "foobar",
            PermissionType = "INSERT",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();

        diff.PermissionDifferences.Count.Should().Be(1);
        diff.PermissionDifferences.ContainsKey("[INSERT] DENIED for user: [foobar]").Should().BeTrue();

        builder.Differences.TableDifferences[TableName]
            .ToString().Should().Contain("Permission: [INSERT] DENIED for user: [foobar] does not exist in database 2");
    }

    [Fact]
    public async Task TablePermissionInBothDatabases_IsNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable());
        builder.Database2.Tables.Add(TableName, new SqlTable());

        builder.Database1.Permissions.Add(new SqlPermission
        {
            ObjectSchema = TableName.GetSchemaName(),
            ObjectName = TableName.GetObjectName(),
            ObjectType = "USER_TABLE",
            UserName = "foobar",
            PermissionType = "INSERT",
        });

        builder.Database2.Permissions.Add(new SqlPermission
        {
            ObjectSchema = TableName.GetSchemaName(),
            ObjectName = TableName.GetObjectName(),
            ObjectType = "USER_TABLE",
            UserName = "foobar",
            PermissionType = "INSERT",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();

        diff.PermissionDifferences.Count.Should().Be(1);
        diff.PermissionDifferences.ContainsKey("[INSERT] DENIED for user: [foobar]").Should().BeTrue();

        diff.PermissionDifferences["[INSERT] DENIED for user: [foobar]"]
            .ExistsInBothDatabases.Should().BeTrue();

        builder.Differences.TableDifferences[TableName].IsDifferent.Should().BeFalse();
    }

    [Fact]
    public async Task TableColumn_WithSingleDifference_IsSingleLine()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable { ColumnDetails = [new SqlColumnDetail { ColumnName = "foobar" }] });
        builder.Database1.Tables[TableName].ColumnDetails[0].OrdinalPosition = 1;
        builder.Database2.Tables.Add(TableName, new SqlTable { ColumnDetails = [new SqlColumnDetail { ColumnName = "foobar" }] });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.IsDifferent.Should().BeTrue();

        diff.ToString().Should().Be($"\r\n{TabIndent}Column: foobar has different ordinal position - is 1 in database 1 and is 0 in database 2\r\n");
    }

    [Fact]
    public async Task TableColumn_WithMultipleDifferences_IsMultipleLines()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.Tables.Add(TableName, new SqlTable { ColumnDetails = [new SqlColumnDetail { ColumnName = "foobar" }] });
        builder.Database1.Tables[TableName].ColumnDetails[0].OrdinalPosition = 1;
        builder.Database1.Tables[TableName].ColumnDetails[0].NumericPrecision = 1;
        builder.Database2.Tables.Add(TableName, new SqlTable { ColumnDetails = [new SqlColumnDetail { ColumnName = "foobar" }] });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.TableDifferences.Should().ContainKey(TableName);

        var diff = builder.Differences.TableDifferences[TableName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.IsDifferent.Should().BeTrue();

        diff.ToString().Should().StartWith($"\r\n{TabIndent}Column: foobar");
        diff.ToString().Should().Contain(" - has different ordinal position - is 1 in database 1 and is 0 in database 2\r\n");
        diff.ToString().Should().Contain(" - has different numeric precision - is 1 in database 1 and is NULL in database 2\r\n");
    }
}
