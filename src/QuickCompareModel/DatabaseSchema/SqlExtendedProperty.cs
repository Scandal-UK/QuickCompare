// <copyright file="SqlExtendedProperty.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using QuickCompareModel.DatabaseSchema.Enums;

/// <summary>
/// Class representing SQL extended property.
/// </summary>
public class SqlExtendedProperty
{
    /// <summary> Gets or sets the property type. </summary>
    public string PropertyType { get; set; }

    /// <summary> Gets or sets the object name. </summary>
    public string ObjectName { get; set; }

    /// <summary> Gets or sets the object schema. </summary>
    public string ObjectSchema { get; set; }

    /// <summary> Gets or sets the column name. </summary>
    public string ColumnName { get; set; }

    /// <summary> Gets or sets the property name. </summary>
    public string PropertyName { get; set; }

    /// <summary> Gets or sets the property value. </summary>
    public string PropertyValue { get; set; }

    /// <summary> Gets or sets the table name. </summary>
    public string TableName { get; set; }

    /// <summary> Gets or sets the index name. </summary>
    public string IndexName { get; set; }

    /// <summary> Gets or sets the table schema. </summary>
    public string TableSchema { get; set; }

    /// <summary> Gets the full ID. </summary>
    public string FullId => !string.IsNullOrEmpty(this.ObjectName)
                ? string.IsNullOrEmpty(this.ColumnName)
                    ? $"[{this.ObjectName}].[{this.PropertyName}].[{this.Type}]"
                    : $"[{this.ObjectName}].[{this.PropertyName}].[{this.ColumnName}].[{this.Type}]"
                : this.PropertyName;

    /// <summary> Gets the target <see cref="PropertyObjectType"/>. </summary>
    public PropertyObjectType Type => this.PropertyType != "INDEX"
                ? !string.IsNullOrEmpty(this.TableName)
                    ? string.IsNullOrEmpty(this.ColumnName) ? PropertyObjectType.Table : PropertyObjectType.TableColumn
                    : this.PropertyType != "DATABASE"
                        ? string.IsNullOrEmpty(this.ColumnName) ? PropertyObjectType.Routine : PropertyObjectType.RoutineColumn
                        : PropertyObjectType.Database
                : PropertyObjectType.Index;
}
