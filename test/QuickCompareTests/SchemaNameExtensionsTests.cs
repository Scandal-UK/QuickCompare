// <copyright file="SchemaNameExtensionsTests.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests
{
    using FluentAssertions;
    using QuickCompareModel.DatabaseSchema;
    using Xunit;

    public class SchemaNameExtensionsTests
    {
        private const string ExpectedInput = "[dbo].[Table1]";
        private const string SchemaName = "dbo";
        private const string ObjectName = "Table1";

        [Fact]
        public void GetSchemaName_GivenExpectedInput_ReturnsExpectedSchema() =>
            ExpectedInput.GetSchemaName()
                .Should().Be(SchemaName);

        [Fact]
        public void GetSchemaName_GivenUnexpectedInput_ReturnsEmptyString() =>
            ObjectName.GetSchemaName()
                .Should().Be(string.Empty);

        [Fact]
        public void GetObjectName_GivenExpectedInput_ReturnsExpectedObjectName() =>
            ExpectedInput.GetObjectName()
                .Should().Be(ObjectName);

        [Fact]
        public void GetObjectName_GivenUnexpectedInput_ReturnsEmptyString() =>
            ObjectName.GetObjectName()
                .Should().Be(string.Empty);

        [Fact]
        public void PrependSchemaName_GivenExpectedInput_ReturnsExpectedFullName() =>
            ObjectName.PrependSchemaName(SchemaName)
                .Should().Be(ExpectedInput);

        [Fact]
        public void PrependSchemaName_GivenEmptySchema_ReturnsOriginalString() =>
            ObjectName.PrependSchemaName(null)
                .Should().Be(ObjectName);
    }
}
