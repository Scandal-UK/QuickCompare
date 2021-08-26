namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class RelationCompareTests
    {
        private const string RelationName = "FK_Table2_Table1";
        private const string TableName = "[dbo].[Table1]";
        private const string SecondTableName = "[dbo].[Table2]";

        [Fact]
        public void RelationMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables[TableName].Relations.Add(new SqlRelation { RelationName = RelationName });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Be("does not exist in database 1\r\n");
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
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Be("does not exist in database 2\r\n");
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
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[TableName]
                .RelationshipDifferences.Should().ContainKey(RelationName);

            var diff = builder.Differences.TableDifferences[TableName].RelationshipDifferences[RelationName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }

        private SqlRelation GetTestRelationship()
        {
            // todo: add unit tests for the following differences
            return new SqlRelation
            {
                RelationName = RelationName,
                ChildSchema = SecondTableName.GetSchemaName(),
                ChildTable = SecondTableName.GetObjectName(),
                ChildColumns = "RelatedColumn",
                UniqueConstraintName = "PK_Table1",
                ParentSchema = TableName.GetSchemaName(),
                ParentTable = TableName.GetObjectName(),
                ParentColumns = "Column1",
                UpdateRule = "NO ACTION",
                DeleteRule = "NO ACTION",
            };
        }
    }
}
