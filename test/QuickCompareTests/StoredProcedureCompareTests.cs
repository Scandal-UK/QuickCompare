// <copyright file="StoredProcedureCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class StoredProcedureCompareTests
{
    private const string StoredProcedureName = "[dbo].[StoredProcedure1]";
    private const string StoredProcedureRoutineType = "PROCEDURE";

    [Fact]
    public async Task StoredProcedureMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Contain("does not exist in database 1");
    }

    [Fact]
    public async Task StoredProcedureMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        diff.ToString().Should().Contain("does not exist in database 2");
    }

    [Fact]
    public async Task StoredProceduresInBothDatabases_AreNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.IsDifferent.Should().BeFalse();
    }

    [Fact]
    public async Task StoredProcedureDefinitionDifference_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType, RoutineDefinition = "foo" });
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType, RoutineDefinition = "bar" });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.IsDifferent.Should().BeTrue();
    }

    [Fact]
    public async Task StoredProcedurePropertyMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = StoredProcedureName.GetObjectName(),
            ObjectSchema = StoredProcedureName.GetSchemaName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences.Count.Should().Be(1);
        diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase1.Should().BeFalse();
    }

    [Fact]
    public async Task StoredProcedurePropertyMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = StoredProcedureName.GetObjectName(),
            ObjectSchema = StoredProcedureName.GetSchemaName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences.Count.Should().Be(1);
        diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase2.Should().BeFalse();
    }

    [Fact]
    public async Task StoredProcedurePropertyDifference_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
        builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = StoredProcedureName.GetObjectName(),
            ObjectSchema = StoredProcedureName.GetSchemaName(),
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "OBJECT_OR_COLUMN",
            ObjectName = StoredProcedureName.GetObjectName(),
            ObjectSchema = StoredProcedureName.GetSchemaName(),
            PropertyName = "Key1",
            PropertyValue = "Value2",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

        var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences.Count.Should().Be(1);
        diff.ExtendedPropertyDifferences["Key1"].ExistsInBothDatabases.Should().BeTrue();
        diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        diff.ToString().Trim()
            .Should().Be("Extended property: Key1 value is different; [Value1] in database 1, [Value2] in database 2");
    }
}
