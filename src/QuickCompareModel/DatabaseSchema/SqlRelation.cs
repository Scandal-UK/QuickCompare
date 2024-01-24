// <copyright file="SqlRelation.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary>
/// Class to represent a relationship in the database.
/// </summary>
public class SqlRelation
{
    /// <summary> Gets or sets the relation nName. </summary>
    public string RelationName { get; set; }

    /// <summary> Gets or sets the child schema. </summary>
    public string ChildSchema { get; set; }

    /// <summary> Gets or sets the child table. </summary>
    public string ChildTable { get; set; }

    /// <summary> Gets or sets the child columns. </summary>
    public string ChildColumns { get; set; }

    /// <summary> Gets or sets the unique-constraint name. </summary>
    public string UniqueConstraintName { get; set; }

    /// <summary> Gets or sets the parent schema. </summary>
    public string ParentSchema { get; set; }

    /// <summary> Gets or sets the parent table. </summary>
    public string ParentTable { get; set; }

    /// <summary> Gets or sets the parent columns. </summary>
    public string ParentColumns { get; set; }

    /// <summary> Gets or sets the update rule. </summary>
    public string UpdateRule { get; set; }

    /// <summary> Gets or sets the delete rule. </summary>
    public string DeleteRule { get; set; }
}
