// <copyright file="ViewCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class ViewCompareTests
{
    [Fact]
    public async Task ViewMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var viewName = "View1";
        builder.Database2.Views.Add(viewName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ViewDifferences.Should().ContainKey(viewName);

        var diff = builder.Differences.ViewDifferences[viewName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Be("does not exist in database 1\r\n");
    }

    [Fact]
    public async Task ViewMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var viewName = "View1";
        builder.Database1.Views.Add(viewName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ViewDifferences.Should().ContainKey(viewName);

        var diff = builder.Differences.ViewDifferences[viewName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        diff.ToString().Should().Be("does not exist in database 2\r\n");
    }

    [Fact]
    public async Task ViewsInBothDatabases_AreNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var viewName = "View1";
        builder.Database1.Views.Add(viewName, "foobar");
        builder.Database2.Views.Add(viewName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.ViewDifferences.Should().ContainKey(viewName);

        var diff = builder.Differences.ViewDifferences[viewName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ToString().Should().Be(string.Empty);
    }
}
