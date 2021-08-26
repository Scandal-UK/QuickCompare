namespace QuickCompareTests
{
    using FluentAssertions;
    using Xunit;

    public class ViewCompareTests
    {
        [Fact]
        public void ViewMissingFromDatabase1_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var viewName = "View1";
            builder.Database2.Views.Add(viewName, "foobar");

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.ViewDifferences.Should().ContainKey(viewName);

            var diff = builder.Differences.ViewDifferences[viewName];
            diff.ExistsInDatabase1.Should().BeFalse();
            diff.ExistsInDatabase2.Should().BeTrue();
            diff.ToString().Should().Be("does not exist in database 1\r\n");
        }

        [Fact]
        public void ViewMissingFromDatabase2_IsReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var viewName = "View1";
            builder.Database1.Views.Add(viewName, "foobar");

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.ViewDifferences.Should().ContainKey(viewName);

            var diff = builder.Differences.ViewDifferences[viewName];
            diff.ExistsInDatabase1.Should().BeTrue();
            diff.ExistsInDatabase2.Should().BeFalse();
            diff.ToString().Should().Be("does not exist in database 2\r\n");
        }

        [Fact]
        public void ViewsInBothDatabases_AreNotReported()
        {
            // Arrange
            var builder = TestHelper.GetBasicBuilder();

            var viewName = "View1";
            builder.Database1.Views.Add(viewName, "foobar");
            builder.Database2.Views.Add(viewName, "foobar");

            // Act
            builder.BuildDifferences();

            // Assert
            builder.Differences.ViewDifferences.Should().ContainKey(viewName);

            var diff = builder.Differences.ViewDifferences[viewName];
            diff.ExistsInBothDatabases.Should().BeTrue();
            diff.ToString().Should().Be(string.Empty);
        }
    }
}
