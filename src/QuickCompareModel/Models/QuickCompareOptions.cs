// <copyright file="QuickCompareOptions.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.Models;

/// <summary> Settings for the database comparison. </summary>
public class QuickCompareOptions
{
    /// <summary> Gets or sets the connection string for the first database. </summary>
    public string ConnectionString1 { get; set; }

    /// <summary> Gets or sets the connection string for the second database. </summary>
    public string ConnectionString2 { get; set; }

    /// <summary> Gets or sets a value indicating whether to ignore remarks and comments. </summary>
    public bool IgnoreSqlComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare table columns. </summary>
    public bool CompareColumns { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare text collation. </summary>
    public bool CompareCollation { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare table relations. </summary>
    public bool CompareRelations { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare views/functions/procedures. </summary>
    public bool CompareObjects { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare indexes. </summary>
    public bool CompareIndexes { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare permissions. </summary>
    public bool ComparePermissions { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare extended properties. </summary>
    public bool CompareProperties { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare triggers. </summary>
    public bool CompareTriggers { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare synonyms. </summary>
    public bool CompareSynonyms { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare ordinal position of columns. </summary>
    public bool CompareOrdinalPositions { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to compare user types. </summary>
    public bool CompareUserTypes { get; set; } = true;
}
