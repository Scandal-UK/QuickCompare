// <copyright file="SqlTrigger.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary> Class to represent a trigger in the database. </summary>
public class SqlTrigger
{
    /// <summary> Gets or sets the file group. </summary>
    public string FileGroup { get; set; }

    /// <summary> Gets or sets the trigger name. </summary>
    public string TriggerName { get; set; }

    /// <summary> Gets or sets the trigger owner. </summary>
    public string TriggerOwner { get; set; }

    /// <summary> Gets or sets the table schema. </summary>
    public string TableSchema { get; set; }

    /// <summary> Gets or sets the table name. </summary>
    public string TableName { get; set; }

    /// <summary> Gets or sets a value indicating whether is update. </summary>
    public bool IsUpdate { get; set; }

    /// <summary> Gets or sets a value indicating whether is delete. </summary>
    public bool IsDelete { get; set; }

    /// <summary> Gets or sets a value indicating whether is insert. </summary>
    public bool IsInsert { get; set; }

    /// <summary> Gets or sets a value indicating whether is after. </summary>
    public bool IsAfter { get; set; }

    /// <summary> Gets or sets a value indicating whether is instead of. </summary>
    public bool IsInsteadOf { get; set; }

    /// <summary> Gets or sets a value indicating whether is disabled. </summary>
    public bool IsDisabled { get; set; }

    /// <summary> Gets or sets the trigger content. </summary>
    public string TriggerContent { get; set; }
}
