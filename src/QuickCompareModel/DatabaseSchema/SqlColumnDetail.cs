// <copyright file="SqlColumnDetail.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary>
/// Class representing properties of a table column.
/// </summary>
public class SqlColumnDetail
{
    /// <summary> Gets or sets the table schema. </summary>
    public string TableSchema { get; set; }

    /// <summary> Gets or sets the table name. </summary>
    public string TableName { get; set; }

    /// <summary> Gets or sets the column name. </summary>
    public string ColumnName { get; set; }

    /// <summary> Gets or sets the ordinal position. </summary>
    public int OrdinalPosition { get; set; }

    /// <summary> Gets or sets the column default. </summary>
    public string ColumnDefault { get; set; }

    /// <summary> Gets or sets a value indicating whether is nullable. </summary>
    public bool IsNullable { get; set; }

    /// <summary> Gets or sets the datatype. </summary>
    public string DataType { get; set; }

    /// <summary> Gets or sets the character maximum length. </summary>
    public int? CharacterMaximumLength { get; set; }

    /// <summary> Gets or sets the character octet length. </summary>
    public int? CharacterOctetLength { get; set; }

    /// <summary> Gets or sets the numeric precision. </summary>
    public int? NumericPrecision { get; set; }

    /// <summary> Gets or sets the numeric precesion radix. </summary>
    public int? NumericPrecisionRadix { get; set; }

    /// <summary> Gets or sets the numeric scale. </summary>
    public int? NumericScale { get; set; }

    /// <summary> Gets or sets the datetime precision. </summary>
    public int? DatetimePrecision { get; set; }

    /// <summary> Gets or sets the character set name. </summary>
    public string CharacterSetName { get; set; }

    /// <summary> Gets or sets the collation name. </summary>
    public string CollationName { get; set; }

    /// <summary> Gets or sets the domain schema. </summary>
    public string DomainSchema { get; set; }

    /// <summary> Gets or sets the domain name. </summary>
    public string DomainName { get; set; }

    /// <summary> Gets or sets a value indicating whether is full-text indexed. </summary>
    public bool IsFullTextIndexed { get; set; }

    /// <summary> Gets or sets a value indicating whether is computed. </summary>
    public bool IsComputed { get; set; }

    /// <summary> Gets or sets a value indicating whether is identity. </summary>
    public bool IsIdentity { get; set; }

    /// <summary> Gets or sets the identity seed. </summary>
    public decimal? IdentitySeed { get; set; }

    /// <summary> Gets or sets the identity increment. </summary>
    public decimal? IdentityIncrement { get; set; }

    /// <summary> Gets or sets a value indicating whether is sparse. </summary>
    public bool IsSparse { get; set; }

    /// <summary> Gets or sets a value indicating whether is column set. </summary>
    public bool IsColumnSet { get; set; }

    /// <summary> Gets or sets a value indicating whether is row guid. </summary>
    public bool IsRowGuid { get; set; }
}
