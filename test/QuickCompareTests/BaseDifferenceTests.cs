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
        private const bool StripWhitespace = true;
        private const bool DoNotStripWhitespace = false;

        /// <summary> Test output has been trimmed. </summary>
        [Fact]
        public void CleanDefinitionText_Trims_Input() =>
            BaseDifference.CleanDefinitionText("   SELECT * FROM Table \r\n ", DoNotStripWhitespace)
                .Should().Be("SELECT * FROM Table");

        /// <summary> Test single-line comments are stripped. </summary>
        [Fact]
        public void CleanDefinitionText_Strips_SingleLineComments() =>
            BaseDifference.CleanDefinitionText(
                "-- Line comment\r\n" +
                "SELECT * -- Inline comment\r\n" +
                "FROM Table\r\n" +
                "-- Line comment", DoNotStripWhitespace)
                .Should().Be("SELECT * \r\nFROM Table");

        /// <summary> Test multi-line comments are stripped. </summary>
        [Fact]
        public void CleanDefinitionText_Strips_MultiLineComments() =>
            BaseDifference.CleanDefinitionText(
                "/******************\r\n" +
                " ** Comment body **\r\n" +
                " ******************/ SELECT * FROM Table", DoNotStripWhitespace)
                .Should().Be("SELECT * FROM Table");

        /// <summary> Test enclosed comments are stripped. </summary>
        [Fact]
        public void CleanDefinitionText_Strips_EnclosedComments() =>
            BaseDifference.CleanDefinitionText(
                "SELECT * FROM /* Enclosed comment */Table", DoNotStripWhitespace)
                .Should().Be("SELECT * FROM Table");

        /// <summary> Test comma whitespace is normalised. </summary>
        [Fact]
        public void CleanDefinitionText_Normalises_CommaWhitespace() =>
            BaseDifference.CleanDefinitionText(
                "SELECT ID , Name ,Email,  Phone  , FootSize\r" +
                ",Timestamp FROM Table", StripWhitespace)
                .Should().Be("SELECT ID, Name, Email, Phone, FootSize, Timestamp FROM Table");

        /// <summary> Test whitespace gets condensed to a single space. </summary>
        [Fact]
        public void CleanDefinitionText_Condenses_Whitespace() =>
            BaseDifference.CleanDefinitionText("SELECT   * \r\n FROM\n\tTable \rWHERE  1 =\r1", StripWhitespace)
                .Should().Be("SELECT * FROM Table WHERE 1 = 1");
    }
}
