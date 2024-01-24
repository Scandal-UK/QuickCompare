// <copyright file="QuickCompareContext.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompare;

using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.Options;
using QuickCompareModel.Models;

/// <summary>
/// View model class which uses application properties to persist form input between restarts.
/// </summary>
public class QuickCompareContext
{
    /// <summary> Delegate for <see cref="INotifyPropertyChanged"/>. </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary> Gets or sets connection string 1. </summary>
    public string ConnectionString1
    {
        get => GetProperty(nameof(this.ConnectionString1), false);
        set => this.SetProperty(nameof(this.ConnectionString1), value);
    }

    /// <summary> Gets or sets connection string 2. </summary>
    public string ConnectionString2
    {
        get => GetProperty(nameof(this.ConnectionString2), false);
        set => this.SetProperty(nameof(this.ConnectionString2), value);
    }

    /// <summary> Gets or sets a value indicating whether to ignore SQL comments. </summary>
    public bool IgnoreSqlComments
    {
        get => bool.Parse(GetProperty(nameof(this.IgnoreSqlComments)));
        set => this.SetProperty(nameof(this.IgnoreSqlComments), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare table columns. </summary>
    public bool CompareColumns
    {
        get => bool.Parse(GetProperty(nameof(this.CompareColumns)));
        set => this.SetProperty(nameof(this.CompareColumns), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare collation. </summary>
    public bool CompareCollation
    {
        get => bool.Parse(GetProperty(nameof(this.CompareCollation)));
        set => this.SetProperty(nameof(this.CompareCollation), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare relations. </summary>
    public bool CompareRelations
    {
        get => bool.Parse(GetProperty(nameof(this.CompareRelations)));
        set => this.SetProperty(nameof(this.CompareRelations), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare objects. </summary>
    public bool CompareObjects
    {
        get => bool.Parse(GetProperty(nameof(this.CompareObjects)));
        set => this.SetProperty(nameof(this.CompareObjects), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare indexes. </summary>
    public bool CompareIndexes
    {
        get => bool.Parse(GetProperty(nameof(this.CompareIndexes)));
        set => this.SetProperty(nameof(this.CompareIndexes), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare permissions. </summary>
    public bool ComparePermissions
    {
        get => bool.Parse(GetProperty(nameof(this.ComparePermissions)));
        set => this.SetProperty(nameof(this.ComparePermissions), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare extended properties. </summary>
    public bool CompareProperties
    {
        get => bool.Parse(GetProperty(nameof(this.CompareProperties)));
        set => this.SetProperty(nameof(this.CompareProperties), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare triggers. </summary>
    public bool CompareTriggers
    {
        get => bool.Parse(GetProperty(nameof(this.CompareTriggers)));
        set => this.SetProperty(nameof(this.CompareTriggers), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare synonyms. </summary>
    public bool CompareSynonyms
    {
        get => bool.Parse(GetProperty(nameof(this.CompareSynonyms)));
        set => this.SetProperty(nameof(this.CompareSynonyms), value.ToString());
    }

    /// <summary> Gets or sets a value indicating whether to compare user types. </summary>
    public bool CompareUserTypes
    {
        get => bool.Parse(GetProperty(nameof(this.CompareUserTypes)));
        set => this.SetProperty(nameof(this.CompareUserTypes), value.ToString());
    }

    /// <summary> Generates an instance of <see cref="IOptions{QuickCompareOptions}"/>. </summary>
    /// <returns>Instance of <see cref="IOptions{QuickCompareOptions}"/>.</returns>
    public IOptions<QuickCompareOptions> OptionsFromProperties()
    {
        QuickCompareOptions settings = new()
        {
            ConnectionString1 = this.ConnectionString1,
            ConnectionString2 = this.ConnectionString2,
            IgnoreSqlComments = this.IgnoreSqlComments,
            CompareColumns = this.CompareColumns,
            CompareCollation = this.CompareCollation,
            CompareRelations = this.CompareRelations,
            CompareObjects = this.CompareObjects,
            CompareIndexes = this.CompareIndexes,
            ComparePermissions = this.ComparePermissions,
            CompareProperties = this.CompareProperties,
            CompareTriggers = this.CompareTriggers,
            CompareSynonyms = this.CompareSynonyms,
            CompareUserTypes = this.CompareUserTypes,
        };

        return Options.Create(settings);
    }

    /// <summary> Method to raise event when a property is changed. </summary>
    /// <param name="propertyName">Name of property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (this.PropertyChanged != null)
        {
            PropertyChangedEventArgs e = new(propertyName);
            this.PropertyChanged(this, e);
        }
    }

    private static string GetProperty(string name, bool isBool = true) => (string)Application.Current.Properties[name] ?? (isBool ? "True" : string.Empty);

    private void SetProperty(string name, string value)
    {
        Application.Current.Properties[name] = value;
        this.OnPropertyChanged(name);
    }
}
