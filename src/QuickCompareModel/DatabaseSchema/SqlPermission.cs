// <copyright file="SqlPermission.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using QuickCompareModel.DatabaseSchema.Enums;

/// <summary> Class to represent a permission in the database. </summary>
public class SqlPermission
{
    /// <summary> Gets or sets the role name. </summary>
    public string RoleName { get; set; }

    /// <summary> Gets or sets the user name. </summary>
    public string UserName { get; set; }

    /// <summary> Gets or sets the permission type. </summary>
    public string PermissionType { get; set; }

    /// <summary> Gets or sets the permission state. </summary>
    public string PermissionState { get; set; }

    /// <summary> Gets or sets the object type. </summary>
    public string ObjectType { get; set; }

    /// <summary> Gets or sets the object name. </summary>
    public string ObjectName { get; set; }

    /// <summary> Gets or sets the column name. </summary>
    public string ColumnName { get; set; }

    /// <summary> Gets or sets the object schema. </summary>
    public string ObjectSchema { get; set; }

    /// <summary> Gets the full ID. </summary>
    public string FullId => $"[{this.RoleName}].[{this.UserName}].[{this.PermissionType}].[{this.PermissionState}].[{this.ObjectType}].[{this.ObjectName}].[{this.ColumnName}]";

    /// <summary> Gets the type. </summary>
    public PermissionObjectType Type => this.ObjectType switch
    {
        "SQL_STORED_PROCEDURE" => PermissionObjectType.SqlStoredProcedure,
        "USER_TABLE" => PermissionObjectType.UserTable,
        "SYSTEM_TABLE" => PermissionObjectType.SystemTable,
        "SYNONYM" => PermissionObjectType.Synonym,
        "VIEW" => PermissionObjectType.View,
        "SQL_SCALAR_FUNCTION" => PermissionObjectType.SqlFunction,
        "SQL_TABLE_VALUED_FUNCTION" => PermissionObjectType.SqlFunction,
        "SQL_INLINE_TABLE_VALUED_FUNCTION" => PermissionObjectType.SqlFunction,
        "DATABASE" => PermissionObjectType.Database,
        _ => PermissionObjectType.Unknown,
    };

    private string TargetName => string.IsNullOrEmpty(this.RoleName) ? this.UserName : this.RoleName;

    private string TargetType => string.IsNullOrEmpty(this.RoleName) ? "user" : "role";

    /// <summary> Generates a text description of the difference. </summary>
    /// <returns>Description of the difference.</returns>
    public override string ToString() => this.PermissionType == "REFERENCES"
            ? $"REFERENCES column: [{this.ColumnName}] {(this.PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {this.TargetType}: [{this.TargetName}]"
            : $"[{this.PermissionType}] {(this.PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {this.TargetType}: [{this.TargetName}]";
}
