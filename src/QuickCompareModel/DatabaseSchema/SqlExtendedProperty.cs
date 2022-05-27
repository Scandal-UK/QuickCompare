// <copyright file="SqlExtendedProperty.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema
{
    using QuickCompareModel.DatabaseSchema.Enums;

    /// <summary>
    /// Class representing SQL extended property.
    /// </summary>
    public class SqlExtendedProperty
    {
        public string PropertyType { get; set; }

        public string ObjectName { get; set; }

        public string ObjectSchema { get; set; }

        public string ColumnName { get; set; }

        public string PropertyName { get; set; }

        public string PropertyValue { get; set; }

        public string TableName { get; set; }

        public string IndexName { get; set; }

        public string TableSchema { get; set; }

        public string FullId => !string.IsNullOrEmpty(ObjectName)
                    ? string.IsNullOrEmpty(ColumnName)
                        ? $"[{ObjectName}].[{PropertyName}].[{Type}]"
                        : $"[{ObjectName}].[{PropertyName}].[{ColumnName}].[{Type}]"
                    : PropertyName;

        public PropertyObjectType Type => PropertyType != "INDEX"
                    ? !string.IsNullOrEmpty(TableName)
                        ? string.IsNullOrEmpty(ColumnName) ? PropertyObjectType.Table : PropertyObjectType.TableColumn
                        : PropertyType != "DATABASE"
                            ? string.IsNullOrEmpty(ColumnName) ? PropertyObjectType.Routine : PropertyObjectType.RoutineColumn
                            : PropertyObjectType.Database
                    : PropertyObjectType.Index;
    }
}
