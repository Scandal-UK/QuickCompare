// <copyright file="SqlUserType.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary>
/// Class to represent a custom type in the database.
/// </summary>
public class SqlUserType
{
    /// <summary> Gets or sets the custom type name. </summary>
    public string CustomTypeName { get; set; }

    /// <summary> Gets or sets the schema name. </summary>
    public string SchemaName { get; set; }

    /// <summary> Gets or sets the underlying type name. </summary>
    public string UnderlyingTypeName { get; set; }

    /// <summary> Gets or sets the precision. </summary>
    public int? Precision { get; set; }

    /// <summary> Gets or sets the scale. </summary>
    public int? Scale { get; set; }

    /// <summary> Gets or sets the max length. </summary>
    public int? MaxLength { get; set; }

    /// <summary> Gets or sets a value indicating whether is nullable. </summary>
    public bool IsNullable { get; set; }

    /// <summary> Gets or sets the collation name. </summary>
    public string CollationName { get; set; }

    /// <summary> Gets or sets a value indicating whether is assembly type. </summary>
    public bool IsAssemblyType { get; set; }
}
