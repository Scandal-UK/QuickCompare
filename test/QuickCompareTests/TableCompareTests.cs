namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class TableCompareTests
    {
        private const string TableName = "[dbo].[Table1]";

        [Fact]
        public void TableMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database2.Tables.Add(TableName, new SqlTable());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Be("does not exist in database 1\r\n");
        }

        [Fact]
        public void TableMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.Tables.Add(TableName, new SqlTable());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Be("does not exist in database 2\r\n");
        }

        [Fact]
        public void TablesInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }

        [Fact]
        public void TablePropertyMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());

            builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = TableName.GetObjectName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
                TableName = TableName.GetObjectName(),
                TableSchema = TableName.GetSchemaName(),
            });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase1.Should().BeFalse();
        }

        [Fact]
        public void TablePropertyMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());

            builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = TableName.GetObjectName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
                TableName = TableName.GetObjectName(),
                TableSchema = TableName.GetSchemaName(),
            });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase2.Should().BeFalse();
        }

        [Fact]
        public void TablePropertyDifference_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.Tables.Add(TableName, new SqlTable());
            builder.Database2.Tables.Add(TableName, new SqlTable());

            builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = TableName.GetObjectName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
                TableName = TableName.GetObjectName(),
                TableSchema = TableName.GetSchemaName(),
            });

            builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = TableName.GetObjectName(),
                PropertyName = "Key1",
                PropertyValue = "Value2",
                TableName = TableName.GetObjectName(),
                TableSchema = TableName.GetSchemaName(),
            });

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.TableDifferences.Should().ContainKey(TableName);

            var diff = builder.Differences.TableDifferences[TableName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        }
    }
}
