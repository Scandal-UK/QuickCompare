// <copyright file="ItemDifference.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences;

using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary> Model to represent an element that belongs to a table. </summary>
public class ItemDifference(bool existsInDatabase1, bool existsInDatabase2) : BaseDifference(existsInDatabase1, existsInDatabase2)
{
    /// <summary> Gets or sets a list of tracked difference strings. </summary>
    public List<string> Differences { get; set; } = [];

    /// <summary> Gets or sets the item type. </summary>
    public string ItemType { get; set; }

    /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
    public override bool IsDifferent => base.IsDifferent || this.Differences.Count > 0;

    /// <summary> Gets a text description of the list of differences or returns an empty string if no difference is detected. </summary>
    /// <returns> Ttext description of the list of differences. </returns>
    public string DifferenceList()
    {
        var sb = new StringBuilder();

        if (this.Differences.Count == 1)
        {
            sb.AppendLine(this.Differences.Single());
        }
        else if (this.Differences.Count > 1)
        {
            sb.Append($"\r\n{TabIndent} - ");
            sb.AppendLine(string.Join($"\r\n{TabIndent} - ", this.Differences.ToArray()));
        }
        else
        {
            sb.Append("\r\n"); // (only ItemWithPropertiesDifference has output)
        }

        return sb.ToString();
    }

    /// <summary> Gets a text description of the differences or returns an empty string if no difference is detected. </summary>
    /// <returns> Difference description. </returns>
    public override string ToString() => this.IsDifferent
        ? base.IsDifferent ? base.ToString() : this.DifferenceList()
        : string.Empty;
}
