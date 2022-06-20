// <copyright file="BaseDifferenceTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseDifferences;
    using Xunit;

    /// <summary>
    /// Tests for the base difference class.
    /// </summary>
    public class BaseDifferenceTests
    {
        private const bool TrimWhitespace = true;
        private const bool DoNotTrimWhitespace = false;

        [Fact]
        public void CleanDefinitionText_Trims_Input() =>
            BaseDifference.CleanDefinitionText("   SELECT * FROM Table \r\n ", DoNotTrimWhitespace)
                .Should().Be("SELECT * FROM Table");

        [Fact]
        public void CleanDefinitionText_Strips_SingleLineComments() =>
            BaseDifference.CleanDefinitionText(
                "-- Line comment\r\n" +
                "SELECT * -- Inline comment\r\n" +
                "FROM Table\r\n" +
                "-- Line comment", DoNotTrimWhitespace)
                .Should().Be("SELECT * \r\nFROM Table");

        [Fact]
        public void CleanDefinitionText_Strips_MultiLineComments() =>
            BaseDifference.CleanDefinitionText(
                "/******************\r\n" +
                " ** Comment body **\r\n" +
                " ******************/ SELECT * FROM Table", DoNotTrimWhitespace)
                .Should().Be("SELECT * FROM Table");

        [Fact]
        public void CleanDefinitionText_Strips_EnclosedComments() =>
            BaseDifference.CleanDefinitionText(
                "SELECT * FROM /* Enclosed comment */Table", DoNotTrimWhitespace)
                .Should().Be("SELECT * FROM Table");

        [Fact]
        public void CleanDefinitionText_Normalises_CommaWhitespace() =>
            BaseDifference.CleanDefinitionText(
                "SELECT ID , Name ,Email,  Phone  , FootSize\r" +
                ",Timestamp FROM Table", TrimWhitespace)
                .Should().Be("SELECT ID, Name, Email, Phone, FootSize, Timestamp FROM Table");

        [Fact]
        public void CleanDefinitionText_Condenses_Whitespace() =>
            BaseDifference.CleanDefinitionText("SELECT   * \r\n FROM\n\tTable \rWHERE  1 =\r1", TrimWhitespace)
                .Should().Be("SELECT * FROM Table WHERE 1 = 1");
    }
}
