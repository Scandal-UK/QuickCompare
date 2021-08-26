namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel;
    using Xunit;

    public class EmbeddedSqlTests
    {
        private const string TextStartsWithSelectRegex = "^[SELECT]";

        [Fact]
        public void LoadNonExistingQueryFromResource_ReturnsEmptyString() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("Foobar")
                .Should().BeEmpty();

        [Fact]
        public void LoadColumnDetailsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("ColumnDetails")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadExtendedPropertiesQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("ExtendedProperties")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadIncludedColumnsForIndexQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("IncludedColumnsForIndex")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadRelationsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("Relations")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadRolePermissionsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("RolePermissions")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadSynonymsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("Synonyms")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadTableNamesQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("TableNames")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadTriggersQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("Triggers")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadUserPermissionsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("UserPermissions")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadUserRoutineDefinitionsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("UserRoutineDefinitions")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadUserRoutinesQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("UserRoutines")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadUserTypesQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("UserTypes")
                .Should().MatchRegex(TextStartsWithSelectRegex);

        [Fact]
        public void LoadViewsQueryFromResource_ReturnsDatabaseQuery() =>
            new SqlDatabase(string.Empty)
                .LoadQueryFromResource("Views")
                .Should().MatchRegex(TextStartsWithSelectRegex);
    }
}
