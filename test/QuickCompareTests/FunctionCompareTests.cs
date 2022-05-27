// <copyright file="FunctionCompareTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class FunctionCompareTests
    {
        private const string FunctionName = "[dbo].[Function1]";
        private const string FunctionRoutineType = "FUNCTION";

        [Fact]
        public void FunctionMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database2.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.FunctionDifferences.Should().ContainKey(FunctionName);

            var diff = builder.Differences.FunctionDifferences[FunctionName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Contain("does not exist in database 1");
        }

        [Fact]
        public void FunctionMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.FunctionDifferences.Should().ContainKey(FunctionName);

            var diff = builder.Differences.FunctionDifferences[FunctionName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Contain("does not exist in database 2");
        }

        [Fact]
        public void FunctionsInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType });
            builder.Database2.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.FunctionDifferences.Should().ContainKey(FunctionName);

            var diff = builder.Differences.FunctionDifferences[FunctionName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.IsDifferent.Should().BeFalse();
        }

        [Fact]
        public void FunctionDefinitionDifference_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType, RoutineDefinition = "FooBar" });
            builder.Database2.UserRoutines.Add(FunctionName, new SqlUserRoutine { RoutineType = FunctionRoutineType, RoutineDefinition = "BarFoo" });

            // Act
            builder.BuildDifferencesAsync().Wait();

            // Assert
            builder.Differences.FunctionDifferences.Should().ContainKey(FunctionName);

            var diff = builder.Differences.FunctionDifferences[FunctionName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.IsDifferent.Should().BeTrue();
        }
    }
}
