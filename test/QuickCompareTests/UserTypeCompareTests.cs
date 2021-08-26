﻿namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class UserTypeCompareTests
    {
        [Fact]
        public void UserTypeMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var userTypeName = "Type1";
            builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

            var diff = builder.Differences.UserTypeDifferences[userTypeName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Contain("does not exist in database 1");
        }

        [Fact]
        public void UserTypeMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var userTypeName = "Type1";
            builder.Database1.UserTypes.Add(userTypeName, new SqlUserType());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

            var diff = builder.Differences.UserTypeDifferences[userTypeName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Contain("does not exist in database 2");
        }

        [Fact]
        public void UserTypeInBothDatabases_IsNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var userTypeName = "Type1";
            builder.Database1.UserTypes.Add(userTypeName, new SqlUserType());
            builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.UserTypeDifferences.Should().ContainKey(userTypeName);

            var diff = builder.Differences.UserTypeDifferences[userTypeName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }

        [Fact]
        public void UnderlyingTypeName_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { UnderlyingTypeName = "char" }, "underlying type")
                .Should().BeTrue();

        [Fact]
        public void Precision_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { Precision = 2 }, "precision")
                .Should().BeTrue();

        [Fact]
        public void Scale_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { Scale = 2 }, "scale")
                .Should().BeTrue();

        [Fact]
        public void MaxLength_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { MaxLength = 2 }, "max length")
                .Should().BeTrue();

        [Fact]
        public void IsNullable_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { IsNullable = true }, "nullable")
                .Should().BeTrue();

        [Fact]
        public void CollationName_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { CollationName = "foobar" }, "collation")
                .Should().BeTrue();

        [Fact]
        public void IsAssemblyType_Difference_IsReported() =>
            ComparisonResultContainsValue(new SqlUserType { IsAssemblyType = true }, "assembly")
                .Should().BeTrue();

        private bool ComparisonResultContainsValue(SqlUserType userType, string value)
        {
            // Arrange
            var userTypeName = "Type1";

            var builder = TestHelper.GetBasicBuilder();
            builder.Database1.UserTypes.Add(userTypeName, userType);
            builder.Database2.UserTypes.Add(userTypeName, new SqlUserType());

            // Act
            builder.BuildDifferences();

            // Assert
            var diff = builder.Differences.UserTypeDifferences[userTypeName];
            return diff.ToString().Contains(value);
        }
    }
}
