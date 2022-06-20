// <copyright file="RelationCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    /// <summary>
    /// Tests for the comparison of relation differences.
    /// </summary>
    public class RelationCompareTests
    {
        private const string RelationName = "FK_Table2_Table1";
        private const string TableName = "[dbo].[Table1]";

        [Fact]
        public void RelationMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables[TableName].Relations.Add(new SqlRelation { RelationName = RelationName });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();

            builder.Differences.TableDifferences[TableName]
                .ToString().Should().Contain($"Relation: {RelationName} does not exist in database 1");
        }

        [Fact]
        public void RelationMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database1.Tables[TableName].Relations.Add(new SqlRelation { RelationName = RelationName });
            builder.Database2.Tables.Add(TableName, new SqlTable());

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();

            builder.Differences.TableDifferences[TableName]
                .ToString().Should().Contain($"Relation: {RelationName} does not exist in database 2");
        }

        [Fact]
        public void RelationsInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database1.Tables[TableName].Relations.Add(new SqlRelation { RelationName = RelationName });
            builder.Database2.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables[TableName].Relations.Add(new SqlRelation { RelationName = RelationName });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }
    }
}
