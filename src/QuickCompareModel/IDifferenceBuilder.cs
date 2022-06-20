// <copyright file="IDifferenceBuilder.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using QuickCompareModel.DatabaseDifferences;
    using QuickCompareModel.DatabaseSchema;

    /// <summary>
    /// Class responsible for building a set of differences between two database instances.
    /// </summary>
    public interface IDifferenceBuilder
    {
        /// <summary> Handler for when the status message changes. </summary>
        event EventHandler<StatusChangedEventArgs> ComparisonStatusChanged;

        /// <summary> Gets or sets the options for the comparison. </summary>
        QuickCompareOptions Options { get; set; }

        /// <summary> Gets or sets the model for database 1. </summary>
        SqlDatabase Database1 { get; set; }

        /// <summary> Gets or sets the model for database 2. </summary>
        SqlDatabase Database2 { get; set; }

        /// <summary> Gets or sets the model representing the differences between two databases. </summary>
        Differences Differences { get; set; }

        /// <summary> Gets or sets the dictionary of definitions that are different. </summary>
        Dictionary<string, (string, string)> DefinitionDifferences { get; set; }

        /// <summary> Inspect two database schemas and build the <see cref="Differences"/> model. </summary>
        /// <returns> A <see cref="Task"/> that represents the asynchronous operation. </returns>
        Task BuildDifferencesAsync();
    }
}