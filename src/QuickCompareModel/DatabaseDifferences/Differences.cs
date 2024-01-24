// <copyright file="Differences.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences;

using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary> Model to hold lists of various differences between two databases. </summary>
public class Differences
{
    /// <summary> Gets or sets a friendly name for database 1. </summary>
    public string Database1 { get; set; }

    /// <summary> Gets or sets a friendly name for database 2. </summary>
    public string Database2 { get; set; }

    /// <summary> Gets or sets a set of models to represent extended properties and track the differences across two databases. </summary>
    public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent permissions and track the differences across two databases. </summary>
    public Dictionary<string, BaseDifference> PermissionDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent tables and track the differences across two databases. </summary>
    public Dictionary<string, TableDifference> TableDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent user types and track the differences across two databases. </summary>
    public Dictionary<string, ItemDifference> UserTypeDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent functions and track the differences across two databases. </summary>
    public Dictionary<string, DatabaseObjectDifference> FunctionDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent stored procedures and track the differences across two databases. </summary>
    public Dictionary<string, DatabaseObjectDifference> StoredProcedureDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent views and track the differences across two databases. </summary>
    public Dictionary<string, DatabaseObjectDifference> ViewDifferences { get; set; }
        = [];

    /// <summary> Gets or sets a set of models to represent synonyms and track the differences across two databases. </summary>
    public Dictionary<string, DatabaseObjectDifference> SynonymDifferences { get; set; }
        = [];

    /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
    public bool HasDifferences =>
        this.ExtendedPropertyDifferences.Any(x => x.Value.IsDifferent) ||
        this.PermissionDifferences.Any(x => x.Value.IsDifferent) ||
        this.TableDifferences.Any(x => x.Value.IsDifferent) ||
        this.FunctionDifferences.Any(x => x.Value.IsDifferent) ||
        this.StoredProcedureDifferences.Any(x => x.Value.IsDifferent) ||
        this.ViewDifferences.Any(x => x.Value.IsDifferent) ||
        this.SynonymDifferences.Any(x => x.Value.IsDifferent);

    /// <summary> Gets a report of the differences, whether any were detected or not. </summary>
    /// <returns> Difference description. </returns>
    public override string ToString()
    {
        var output = new StringBuilder("QuickCompare schema comparison result\r\n\r\n");
        output.AppendLine($"Database 1: {this.Database1}");
        output.AppendLine($"Database 2: {this.Database2}\r\n");

        if (!this.HasDifferences)
        {
            output.AppendLine("NO DIFFERENCES HAVE BEEN FOUND");
            return output.ToString();
        }

        output.Append(GetSectionDifferenceOutput(this.ExtendedPropertyDifferences, "Extended property"));
        output.Append(GetSectionDifferenceOutput(this.PermissionDifferences, "Permission"));
        output.Append(GetSectionDifferenceOutput(this.TableDifferences, "Table"));
        output.Append(GetSectionDifferenceOutput(this.UserTypeDifferences, "User type"));
        output.Append(GetSectionDifferenceOutput(this.ViewDifferences, "View"));
        output.Append(GetSectionDifferenceOutput(this.FunctionDifferences, "Function"));
        output.Append(GetSectionDifferenceOutput(this.StoredProcedureDifferences, "Stored procedure"));
        output.Append(GetSectionDifferenceOutput(this.SynonymDifferences, "Synonym"));

        return output.ToString();
    }

    private static string GetSectionDifferenceOutput<T>(Dictionary<string, T> source, string name)
        where T : BaseDifference
    {
        var section = new StringBuilder();

        foreach (var prop in source.Where(x => x.Value.IsDifferent))
        {
            section.Append($"{name}: {prop.Key} {prop.Value}");
        }

        if (section.Length > 0)
        {
            section.Insert(0, $"\r\n{name.ToUpperInvariant()} DIFFERENCES\r\n\r\n");
        }

        return section.ToString();
    }
}
