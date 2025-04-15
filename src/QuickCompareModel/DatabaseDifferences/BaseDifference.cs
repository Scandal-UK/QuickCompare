// <copyright file="BaseDifference.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseDifferences;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary> Model to represent the most basic element and track the existence across two databases. </summary>
public partial class BaseDifference(bool existsInDatabase1, bool existsInDatabase2)
{
    /// <summary> Whitespace indentation used for output text. </summary>
    protected const string TabIndent = "     ";

    /// <summary> Gets or sets a value indicating whether the item exists in database 1. </summary>
    public bool ExistsInDatabase1 { get; set; } = existsInDatabase1;

    /// <summary> Gets or sets a value indicating whether the item exists in database 2. </summary>
    public bool ExistsInDatabase2 { get; set; } = existsInDatabase2;

    /// <summary> Gets a value indicating whether the item exists in both databases. </summary>
    public bool ExistsInBothDatabases => this.ExistsInDatabase1 && this.ExistsInDatabase2;

    /// <summary> Gets a value indicating whether there are any differences. </summary>
    public virtual bool IsDifferent => !this.ExistsInBothDatabases;

    /// <summary> A helper method to trim whitespace and comments from text to reduce false-positive results. </summary>
    /// <param name="definition">Text to modify.</param>
    /// <param name="stripWhiteSpace">Value indicating where to reduce all whitespace to a single character.</param>
    /// <returns>Modified text ready for comparison.</returns>
    public static string CleanDefinitionText(string definition, bool stripWhiteSpace)
    {
        if (string.IsNullOrWhiteSpace(definition))
        {
            return string.Empty;
        }

        definition = StripMultiLineComments(definition);
        definition = StripSingleLineComments(definition);

        if (stripWhiteSpace)
        {
            definition = StripCommaWhitespace(definition);
            definition = NormaliseCommas(definition);
            definition = ReduceWhitespaceToSingleCharacter(definition);
        }

        return definition.Trim();
    }

    /// <summary> Gets a text description of the difference or returns an empty string if no difference is detected. </summary>
    /// <returns> Difference description. </returns>
    public override string ToString() => this.ExistsInBothDatabases ? string.Empty : $"does not exist in database {(this.ExistsInDatabase1 ? 2 : 1)}\r\n";

    /// <summary> A helper method to list difference values for subsections. </summary>
    /// <typeparam name="T">Implementation of <see cref="BaseDifference"/>.</typeparam>
    /// <param name="source">Difference collection.</param>
    /// <param name="name">Name of subsection.</param>
    /// <returns>Text output for subsection.</returns>
    protected static string GetSubSectionDifferenceOutput<T>(Dictionary<string, T> source, string name)
        where T : BaseDifference
    {
        var section = new StringBuilder();

        foreach (var prop in source.Where(x => x.Value.IsDifferent))
        {
            section.Append($"{TabIndent}{name}: {prop.Key} {prop.Value}");
        }

        return section.ToString();
    }

    private static string StripMultiLineComments(string input) => MultilineCommentRegex().Replace(input, string.Empty);

    private static string StripSingleLineComments(string input) => SingleLineCommentRegex().Replace(input, string.Empty);

    private static string StripCommaWhitespace(string input) => CommaWitespaceRegex().Replace(input, ",");

    private static string NormaliseCommas(string input) => CommaRegex().Replace(input, ", ");

    private static string ReduceWhitespaceToSingleCharacter(string input) => WhitespaceRegex().Replace(input, " ");

    [GeneratedRegex(@"/\*[^*]*\*+([^/*][^*]*\*+)*/")] private static partial Regex MultilineCommentRegex();

    [GeneratedRegex(@"(--)([^\r\n]+)")] private static partial Regex SingleLineCommentRegex();

    [GeneratedRegex(@"\s*,\s*")] private static partial Regex CommaWitespaceRegex();

    [GeneratedRegex(@"[,]")] private static partial Regex CommaRegex();

    [GeneratedRegex(@"[\s]+")] private static partial Regex WhitespaceRegex();
}
