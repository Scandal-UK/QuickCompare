﻿// <copyright file="DatabaseObjectDifference.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences;

using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary> Model to represent a complex database element and track the differences across two databases. </summary>
public class DatabaseObjectDifference(bool existsInDatabase1, bool existsInDatabase2) : BaseDifference(existsInDatabase1, existsInDatabase2)
{
    /// <summary> Gets or sets the body text of the object in database 1. </summary>
    public string ObjectDefinition1 { get; set; }

    /// <summary> Gets or sets the body text of the object in database 2. </summary>
    public string ObjectDefinition2 { get; set; }

    /// <summary> Gets or sets a set of models to represent extended properties and track the differences across two databases. </summary>
    public Dictionary<string, ExtendedPropertyDifference> ExtendedPropertyDifferences { get; set; } = [];

    /// <summary> Gets or sets a set of models to represent permissions and track the differences across two databases. </summary>
    public Dictionary<string, BaseDifference> PermissionDifferences { get; set; } = [];

    /// <summary> Gets a value indicating whether the body text is different. </summary>
    public bool DefinitionsAreDifferent => CleanDefinitionText(this.ObjectDefinition1, true) != CleanDefinitionText(this.ObjectDefinition2, true);

    /// <summary> Gets a value indicating whether any differences have been tracked. </summary>
    public override bool IsDifferent =>
        base.IsDifferent ||
        this.DefinitionsAreDifferent ||
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

        var sb = new StringBuilder("\r\n");

        if (this.DefinitionsAreDifferent)
        {
            sb.AppendLine($"{TabIndent}Definitions are different");
        }

        sb.Append(GetSubSectionDifferenceOutput(this.ExtendedPropertyDifferences, "Extended property"));
        sb.Append(GetSubSectionDifferenceOutput(this.PermissionDifferences, "Permission"));

        return sb.ToString();
    }
}
