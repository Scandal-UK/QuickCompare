// <copyright file="SchemaNameExtensionsTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    /// <summary>
    /// Tests the extension class for schema names.
    /// </summary>
    public class SchemaNameExtensionsTests
    {
        private const string ExpectedInput = "[dbo].[Table1]";
        private const string SchemaName = "dbo";
        private const string ObjectName = "Table1";

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void GetSchemaName_GivenExpectedInput_ReturnsExpectedSchema() =>
            ExpectedInput.GetSchemaName()
                .Should().Be(SchemaName);

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void GetSchemaName_GivenUnexpectedInput_ReturnsEmptyString() =>
            ObjectName.GetSchemaName()
                .Should().Be(string.Empty);

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void GetObjectName_GivenExpectedInput_ReturnsExpectedObjectName() =>
            ExpectedInput.GetObjectName()
                .Should().Be(ObjectName);

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void GetObjectName_GivenUnexpectedInput_ReturnsEmptyString() =>
            ObjectName.GetObjectName()
                .Should().Be(string.Empty);

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void PrependSchemaName_GivenExpectedInput_ReturnsExpectedFullName() =>
            ObjectName.PrependSchemaName(SchemaName)
                .Should().Be(ExpectedInput);

        /// <summary> Test output is as expected. </summary>
        [Fact]
        public void PrependSchemaName_GivenEmptySchema_ReturnsOriginalString() =>
            ObjectName.PrependSchemaName(null)
                .Should().Be(ObjectName);
    }
}
