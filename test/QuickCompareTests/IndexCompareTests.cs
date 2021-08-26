namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class IndexCompareTests
    {
        [Fact]
        public void IndexMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var indexName = "Index1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .IndexDifferences.Should().ContainKey(indexName);

            var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Be("does not exist in database 1\r\n");
        }

        [Fact]
        public void IndexMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var indexName = "Index1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });
            builder.Database2.Tables.Add(tableName, new SqlTable());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .IndexDifferences.Should().ContainKey(indexName);

            var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Be("does not exist in database 2\r\n");
        }

        [Fact]
        public void IndexInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var indexName = "Index1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database1.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });
            builder.Database2.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables[tableName].Indexes.Add(new SqlIndex { IndexName = indexName });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .IndexDifferences.Should().ContainKey(indexName);

            var diff = builder.Differences.TableDifferences[tableName].IndexDifferences[indexName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.IsDifferent.Should().BeFalse();
        }
    }
}
