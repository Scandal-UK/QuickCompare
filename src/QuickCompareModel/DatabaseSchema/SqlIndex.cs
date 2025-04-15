// <copyright file="SqlIndex.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using System.Collections.Generic;
using System.Text;

/// <summary> Class to represent an index in the database. </summary>
public class SqlIndex
{
    /// <summary> Gets or sets a value indicating whether is primary key. </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary> Gets or sets the table schema. </summary>
    public string TableSchema { get; set; }

    /// <summary> Gets or sets the table name. </summary>
    public string TableName { get; set; }

    /// <summary> Gets or sets the index name. </summary>
    public string IndexName { get; set; }

    /// <summary> Gets or sets a value indicating whether is clustered. </summary>
    public bool Clustered { get; set; }

    /// <summary> Gets or sets a value indicating whether is unique. </summary>
    public bool Unique { get; set; }

    /// <summary> Gets or sets a value indicating whether is unique key. </summary>
    public bool IsUniqueKey { get; set; }

    /// <summary> Gets or sets the index columns. </summary>
    public Dictionary<int, KeyValuePair<string, bool>> IndexColumns { get; set; }

    /// <summary> Gets or sets the file group. </summary>
    public string FileGroup { get; set; }

    /// <summary> Gets or sets the columns. </summary>
    public Dictionary<string, bool> Columns { get; set; } = [];

    /// <summary> Gets or sets the included columns. </summary>
    public Dictionary<string, bool> IncludedColumns { get; set; } = [];

    /// <summary> Gets the full ID. </summary>
    public string FullId => $"[{this.TableSchema}].[{this.TableName}].[{this.IndexName}]";

    /// <summary> Gets the columns as a string. </summary>
    public string ColumnsToString => FlagListToString(this.Columns);

    /// <summary> Gets the included columns as a string. </summary>
    public string IncludedColumnsToString => FlagListToString(this.IncludedColumns);

    /// <summary> Gets the item type. </summary>
    public string ItemType => !this.IsPrimaryKey ? this.IsUniqueKey ? "Unique key" : "Index" : "Primary key";

    /// <summary> Sets the columns value from a given string. </summary>
    /// <param name="value">Comma-separated list of column names.</param>
    public void SetColumnsFromString(string value)
    {
        var columnNames = value.Split(',');
        foreach (var columnName in columnNames)
        {
            if (columnName.Contains("(-)"))
            {
                this.Columns.Add(columnName.Replace("(-)", string.Empty).Trim(), false);
            }
            else
            {
                this.Columns.Add(columnName.Trim(), true);
            }
        }
    }

    private static string FlagListToString(Dictionary<string, bool> flagList)
    {
        if (flagList == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var pair in flagList)
        {
            sb.AppendLine($"{pair.Key}, {pair.Value}");
        }

        return sb.ToString();
    }
}
