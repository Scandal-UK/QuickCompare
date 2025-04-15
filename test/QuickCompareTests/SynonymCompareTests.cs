// <copyright file="SynonymCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class SynonymCompareTests
{
    [Fact]
    public async Task SynonymMissingFromDatabase1_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var synonymName = "Synonym1";
        builder.Database2.Synonyms.Add(synonymName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.SynonymDifferences.Should().ContainKey(synonymName);

        var diff = builder.Differences.SynonymDifferences[synonymName];
        diff.ExistsInDatabase1.Should().BeFalse();
        diff.ExistsInDatabase2.Should().BeTrue();
        diff.ToString().Should().Be("does not exist in database 1\r\n");
    }

    [Fact]
    public async Task SynonymMissingFromDatabase2_IsReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var synonymName = "Synonym1";
        builder.Database1.Synonyms.Add(synonymName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.SynonymDifferences.Should().ContainKey(synonymName);

        var diff = builder.Differences.SynonymDifferences[synonymName];
        diff.ExistsInDatabase1.Should().BeTrue();
        diff.ExistsInDatabase2.Should().BeFalse();
        diff.ToString().Should().Be("does not exist in database 2\r\n");
    }

    [Fact]
    public async Task SynonymsInBothDatabases_AreNotReported()
    {
        // Arrange
        var builder = TestHelper.GetBasicBuilder();

        var synonymName = "Synonym1";
        builder.Database1.Synonyms.Add(synonymName, "foobar");
        builder.Database2.Synonyms.Add(synonymName, "foobar");

        // Act
        await builder.BuildDifferencesAsync();

        // Assert
        builder.Differences.SynonymDifferences.Should().ContainKey(synonymName);

        var diff = builder.Differences.SynonymDifferences[synonymName];
        diff.ExistsInBothDatabases.Should().BeTrue();
        diff.ToString().Should().Be(string.Empty);
    }
}
