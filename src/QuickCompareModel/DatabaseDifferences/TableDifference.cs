﻿// <copyright file="TableDifference.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences;

using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary> Model to represent the table element and track the differences across two databases. </summary>
public class TableDifference(bool existsInDatabase1, bool existsInDatabase2) : BaseDifference(existsInDatabase1, existsInDatabase2)
{
    /// <summary> Gets or sets a set of models to represent columns and track the differences across two databases. </summary>
    public Dictionary<string, ItemWithPropertiesDifference> ColumnDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent columns and track the differences across two databases. </summary>
    public Dictionary<string, ItemDifference> RelationshipDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent indexes and track the differences across two databases. </summary>
    public Dictionary<string, ItemWithPropertiesDifference> IndexDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent triggers and track the differences across two databases. </summary>
    public Dictionary<string, ItemDifference> TriggerDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent extended properties and track the differences across two databases. </summary>
    public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent permissions and track the differences across two databases. </summary>
    public Dictionary<string, BaseDifference> PermissionDifferences { get; set; } = [];

    /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
    public override bool IsDifferent =>
        base.IsDifferent ||
        this.ColumnDifferences.Values.Any(x => x.IsDifferent) ||
        this.RelationshipDifferences.Values.Any(x => x.IsDifferent) ||
        this.IndexDifferences.Values.Any(x => x.IsDifferent) ||
        this.TriggerDifferences.Values.Any(x => x.IsDifferent) ||
        this.PermissionDifferences.Values.Any(x => x.IsDifferent) ||
        this.ExtendedPropertyDifferences.Values.Any(x => x.IsDifferent);

    /// <summary> Gets a text description of the difference or returns an empty string if no difference is detected. </summary>
    /// <returns> Difference description. </returns>
    public override string ToString()
    {
        if (!this.IsDifferent)
        {
            return string.Empty;
        }

        if (base.IsDifferent)
        {
            return base.ToString();
        }

        var section = new StringBuilder("\r\n");

        section.Append(GetSubSectionDifferenceOutput(this.ColumnDifferences, "Column"));
        section.Append(GetSubSectionDifferenceOutput(this.TriggerDifferences, "Trigger"));
        section.Append(GetSubSectionDifferenceOutput(this.RelationshipDifferences, "Relation"));
        section.Append(GetSubSectionDifferenceOutput(this.ExtendedPropertyDifferences, "Extended property"));
        section.Append(GetSubSectionDifferenceOutput(this.PermissionDifferences, "Permission"));

        foreach (var indexDiff in this.IndexDifferences.Where(x => x.Value.IsDifferent))
        {
            section.Append($"{TabIndent}{indexDiff.Value.ItemType}: {indexDiff.Key} {indexDiff.Value}");
        }

        return section.ToString();
    }
}
