// <copyright file="EmbeddedSqlTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using FluentAssertions;
using QuickCompareModel.DatabaseSchema;
using Xunit;

public class EmbeddedSqlTests
{
    private const string TextStartsWithSelectRegex = "^[SELECT]";

    [Fact]
    public void LoadNonExistingQueryFromResource_ReturnsEmptyString() =>
        SqlDatabase
            .LoadQueryFromResource("Foobar")
            .Should().BeEmpty();

    [Fact]
    public void LoadColumnDetailsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("ColumnDetails")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadExtendedPropertiesQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("ExtendedProperties")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadIncludedColumnsForIndexQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("IncludedColumnsForIndex")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadRelationsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("Relations")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadRolePermissionsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("RolePermissions")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadSynonymsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("Synonyms")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadTableNamesQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("TableNames")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadTriggersQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("Triggers")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadUserPermissionsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("UserPermissions")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadUserRoutineDefinitionsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("UserRoutineDefinitions")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadUserRoutinesQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("UserRoutines")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadUserTypesQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("UserTypes")
            .Should().MatchRegex(TextStartsWithSelectRegex);

    [Fact]
    public void LoadViewsQueryFromResource_ReturnsDatabaseQuery() =>
        SqlDatabase
            .LoadQueryFromResource("Views")
            .Should().MatchRegex(TextStartsWithSelectRegex);
}
