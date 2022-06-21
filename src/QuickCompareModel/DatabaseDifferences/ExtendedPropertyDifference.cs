// <copyright file="ExtendedPropertyDifference.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences
{
    /// <summary> Model to represent an extended property and track the differences across two databases. </summary>
    public class ExtendedPropertyDifference : BaseDifference
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ExtendedPropertyDifference"/> class
        /// with values determining whether the item exists in each database.
        /// </summary>
        /// <param name="existsInDatabase1">Value indicating whether the item exists in database 1.</param>
        /// <param name="existsInDatabase2">Value indicating whether the item exists in database 2.</param>
        public ExtendedPropertyDifference(bool existsInDatabase1, bool existsInDatabase2)
            : base(existsInDatabase1, existsInDatabase2)
        {
        }

        /// <summary> Gets or sets the property value for database 1. </summary>
        public string Value1 { get; set; }

        /// <summary> Gets or sets the property value for database 2. </summary>
        public string Value2 { get; set; }

        /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
        public override bool IsDifferent => base.IsDifferent || this.Value1 != this.Value2;

        /// <summary> Gets a text description of the difference or returns an empty string if no difference is detected. </summary>
        /// <returns> Difference description. </returns>
        public override string ToString() => this.IsDifferent
                ? base.IsDifferent ? base.ToString() : $"value is different; [{this.Value1}] in database 1, [{this.Value2}] in database 2\r\n"
                : string.Empty;
    }
}
