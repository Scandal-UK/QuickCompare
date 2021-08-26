﻿namespace QuickCompareModel
{
    using System;
    using QuickCompareModel.DatabaseDifferences;

    /// <summary>
    /// Class responsible for building a set of differences between two database instances.
    /// </summary>
    public interface IDifferenceBuilder
    {
        /// <summary> Gets or sets the options for the comparison. </summary>
        QuickCompareOptions Options { get; set; }

        /// <summary> Gets or sets the model for database 1. </summary>
        SqlDatabase Database1 { get; set; }

        /// <summary> Gets or sets the model for database 2. </summary>
        SqlDatabase Database2 { get; set; }

        /// <summary> Model representing the differences between two databases. </summary>
        Differences Differences { get; set; }

        /// <summary> Handler for when the status message changes. </summary>
        event EventHandler<StatusChangedEventArgs> ComparisonStatusChanged;

        /// <summary> Inspect two database schemas and build the <see cref="Differences"/> model. </summary>
        void BuildDifferences();
    }
}