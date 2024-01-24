// <copyright file="SchemaNameExtensions.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary> Extensions to derive the schema and object names from a full object ID. </summary>
public static class SchemaNameExtensions
{
    /// <summary> Returns the schema name from a full object ID. </summary>
    /// <param name="input">Full object ID.</param>
    /// <returns>Schema name or empty string.</returns>
    public static string GetSchemaName(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string[] parts = input.Split('.');
        return parts.Length != 2 ? string.Empty : parts[0].StripSquareBrackets();
    }

    /// <summary> Returns the object name from a full object ID. </summary>
    /// <param name="input">Full object ID.</param>
    /// <returns>Object name or empty string.</returns>
    public static string GetObjectName(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string[] parts = input.Split('.');
        return parts.Length != 2 ? string.Empty : parts[1].StripSquareBrackets();
    }

    /// <summary> Adds the schema name to the beginning of the string. </summary>
    /// <param name="objectName">Source object name.</param>
    /// <param name="schemaName">Schema name to prepend.</param>
    /// <returns>A dot-delimited string containing the schema name and object name.</returns>
    public static string PrependSchemaName(this string objectName, string schemaName) =>
        string.IsNullOrEmpty(schemaName) ? objectName : $"[{schemaName}].[{objectName}]";

    /// <summary>/ Removes instances of square bracket characters from the string. </summary>
    /// <param name="input">Source input.</param>
    /// <returns>The source input without square bracket characters.</returns>
    public static string StripSquareBrackets(this string input) =>
        input
            .Replace("[", string.Empty)
            .Replace("]", string.Empty);
}
