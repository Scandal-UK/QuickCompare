// <copyright file="UserTypeCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class UserTypeCompareTests
{
    [Fact]
    public async Task UserTypeMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var userTypeName = "Type1";
        builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

        var diff = builder.Differences.UserTypeDifferences[userTypeName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Contain("does not exist in database 1");
    }

    [Fact]
    public async Task UserTypeMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var userTypeName = "Type1";
        builder.Database1.UserTypes.Add(userTypeName, new SqlUserType());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

        var diff = builder.Differences.UserTypeDifferences[userTypeName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        diff.ToString().Should().Contain("does not exist in database 2");
    }

    [Fact]
    public async Task UserTypeInBothDatabases_IsNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var userTypeName = "Type1";
        builder.Database1.UserTypes.Add(userTypeName, new SqlUserType());
        builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

        var diff = builder.Differences.UserTypeDifferences[userTypeName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public async Task UnderlyingTypeName_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { UnderlyingTypeName = "char" }, "underlying type");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Precision_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { Precision = 2 }, "precision");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Scale_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { Scale = 2 }, "scale");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MaxLength_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { MaxLength = 2 }, "max length");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsNullable_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { IsNullable = true }, "nullable");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CollationName_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { CollationName = "foobar" }, "collation");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAssemblyType_Difference_IsReported()
    {
        var result = await ComparisonResultContainsValue(new SqlUserType { IsAssemblyType = true }, "assembly");
        result.Should().BeTrue();
    }

    private static async Task<bool> ComparisonResultContainsValue(SqlUserType userType, string value)
    {
        // Arrange
        var userTypeName = "Type1";

        var builder = TestHelper.GetBasicBuilder();
        builder.Database1.UserTypes.Add(userTypeName, userType);
        builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        var diff = builder.Differences.UserTypeDifferences[userTypeName];
        return diff.ToString().Contains(value);
    }
}
