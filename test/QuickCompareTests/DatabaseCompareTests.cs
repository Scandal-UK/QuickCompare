// <copyright file="DatabaseCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class DatabaseCompareTests
{
    [Fact]
    public void Database_FriendlyName_ReturnsAsExpected()
    {
        var expectedResult = "[localhost].[Northwind]";

        new SqlDatabase("Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True")
            .FriendlyName.Should().Be(expectedResult);

        new SqlDatabase("Server=localhost;Database=Northwind;Integrated Security=True")
            .FriendlyName.Should().Be(expectedResult);

        new SqlDatabase("Data Source=localhost;Database=Northwind;Integrated Security=True")
            .FriendlyName.Should().Be(expectedResult);

        new SqlDatabase("Server=localhost;Initial Catalog=Northwind;Integrated Security=True")
            .FriendlyName.Should().Be(expectedResult);
    }

    [Fact]
    public async Task TablePropertyMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "DATABASE",
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ExtendedPropertyDifferences.Count.Should().Be(1);
        builder.Differences.ExtendedPropertyDifferences["Key1"].ExistsInDatabase1.Should().BeFalse();
    }

    [Fact]
    public async Task TablePropertyMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "DATABASE",
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ExtendedPropertyDifferences.Count.Should().Be(1);
        builder.Differences.ExtendedPropertyDifferences["Key1"].ExistsInDatabase2.Should().BeFalse();
    }

    [Fact]
    public async Task TablePropertyDifference_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "DATABASE",
            PropertyName = "Key1",
            PropertyValue = "Value1",
        });

        builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
        {
            PropertyType = "DATABASE",
            PropertyName = "Key1",
            PropertyValue = "Value2",
        });

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ExtendedPropertyDifferences.Count.Should().Be(1);
        builder.Differences.ExtendedPropertyDifferences["Key1"].ExistsInBothDatabases.Should().BeTrue();
        builder.Differences.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
    }
}
