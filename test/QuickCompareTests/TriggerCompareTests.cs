// <copyright file="TriggerCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class TriggerCompareTests
    {
        [Fact]
        public void IndexMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var triggerName = "Trigger1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables[tableName].Triggers.Add(new SqlTrigger { TriggerName = triggerName });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .TriggerDifferences.Should().ContainKey(triggerName);

            var diff = builder.Differences.TableDifferences[tableName].TriggerDifferences[triggerName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();

            builder.Differences.TableDifferences[tableName]
                .ToString().Should().Contain($"Trigger: {triggerName} does not exist in database 1");
        }

        [Fact]
        public void IndexMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var triggerName = "Trigger1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database1.Tables[tableName].Triggers.Add(new SqlTrigger { TriggerName = triggerName });
            builder.Database2.Tables.Add(tableName, new SqlTable());

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .TriggerDifferences.Should().ContainKey(triggerName);

            var diff = builder.Differences.TableDifferences[tableName].TriggerDifferences[triggerName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();

            builder.Differences.TableDifferences[tableName]
                .ToString().Should().Contain($"Trigger: {triggerName} does not exist in database 2");
        }

        [Fact]
        public void IndexInBothDatabases_IsNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var tableName = "Table1";
            var triggerName = "Trigger1";
            builder.Database1.Tables.Add(tableName, new SqlTable());
            builder.Database1.Tables[tableName].Triggers.Add(new SqlTrigger { TriggerName = triggerName, TableName = tableName });
            builder.Database2.Tables.Add(tableName, new SqlTable());
            builder.Database2.Tables[tableName].Triggers.Add(new SqlTrigger { TriggerName = triggerName, TableName = tableName });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.TableDifferences[tableName]
                .TriggerDifferences.Should().ContainKey(triggerName);

            var diff = builder.Differences.TableDifferences[tableName].TriggerDifferences[triggerName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }
    }
}
