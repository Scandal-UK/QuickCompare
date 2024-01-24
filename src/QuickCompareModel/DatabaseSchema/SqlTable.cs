// <copyright file="SqlTable.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using System.Collections.Generic;

/// <summary>
/// Class to represent a table in the database.
/// </summary>
public class SqlTable
{
    /// <summary> Gets or sets the column details. </summary>
    public List<SqlColumnDetail> ColumnDetails { get; set; } = [];

    /// <summary> Gets or sets the relations. </summary>
    public List<SqlRelation> Relations { get; set; } = [];

    /// <summary> Gets or sets the indexes. </summary>
    public List<SqlIndex> Indexes { get; set; } = [];

    /// <summary> Gets or sets the triggers. </summary>
    public List<SqlTrigger> Triggers { get; set; } = [];

    /// <summary> Gets a value indicating whether column has unique index. </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>True if has a unique index.</returns>
    public bool ColumnHasUniqueIndex(string columnName) =>
        this.Indexes.Exists(x => x.Unique && x.Columns.ContainsKey(columnName) && x.Columns.Count == 1);
}
