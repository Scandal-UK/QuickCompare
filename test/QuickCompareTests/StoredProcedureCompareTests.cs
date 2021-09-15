﻿namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class StoredProcedureCompareTests
    {
        private const string StoredProcedureName = "[dbo].[StoredProcedure1]";
        private const string StoredProcedureRoutineType = "PROCEDURE";

        [Fact]
        public void StoredProcedureMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Contain("does not exist in database 1");
        }

        [Fact]
        public void StoredProcedureMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Contain("does not exist in database 2");
        }

        [Fact]
        public void StoredProceduresInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.IsDifferent.Should().BeFalse();
        }

        [Fact]
        public void StoredProcedureDefinitionDifference_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType, RoutineDefinition = "foo" });
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType, RoutineDefinition = "bar" });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.IsDifferent.Should().BeTrue();
        }

        [Fact]
        public void StoredProcedurePropertyMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = StoredProcedureName.GetObjectName(),
                ObjectSchema = StoredProcedureName.GetSchemaName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
            });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase1.Should().BeFalse();
        }

        [Fact]
        public void StoredProcedurePropertyMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = StoredProcedureName.GetObjectName(),
                ObjectSchema = StoredProcedureName.GetSchemaName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
            });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInDatabase2.Should().BeFalse();
        }

        [Fact]
        public void StoredProcedurePropertyDifference_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });
            builder.Database2.UserRoutines.Add(StoredProcedureName, new SqlUserRoutine { RoutineType = StoredProcedureRoutineType });

            builder.Database1.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = StoredProcedureName.GetObjectName(),
                ObjectSchema = StoredProcedureName.GetSchemaName(),
                PropertyName = "Key1",
                PropertyValue = "Value1",
            });

            builder.Database2.ExtendedProperties.Add(new SqlExtendedProperty
            {
                PropertyType = "OBJECT_OR_COLUMN",
                ObjectName = StoredProcedureName.GetObjectName(),
                ObjectSchema = StoredProcedureName.GetSchemaName(),
                PropertyName = "Key1",
                PropertyValue = "Value2",
            });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.StoredProcedureDifferences.Should().ContainKey(StoredProcedureName);

            var diff = builder.Differences.StoredProcedureDifferences[StoredProcedureName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences.Count.Should().Be(1);
            diff.ExtendedPropertyDifferences["Key1"].ExistsInBothDatabases.Should().BeTrue();
            diff.ExtendedPropertyDifferences["Key1"].IsDifferent.Should().BeTrue();
        }
    }
}
