// <copyright file="SqlPermission.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema
{
    using QuickCompareModel.DatabaseSchema.Enums;

    public class SqlPermission
    {
        public string RoleName { get; set; }

        public string UserName { get; set; }

        public string PermissionType { get; set; }

        public string PermissionState { get; set; }

        public string ObjectType { get; set; }

        public string ObjectName { get; set; }

        public string ColumnName { get; set; }

        public string ObjectSchema { get; set; }

        public string FullId => $"[{this.RoleName}].[{this.UserName}].[{this.PermissionType}].[{this.PermissionState}].[{this.ObjectType}].[{this.ObjectName}].[{this.ColumnName}]";

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

        public override string ToString() => this.PermissionType == "REFERENCES"
                ? $"REFERENCES column: [{this.ColumnName}] {(this.PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {this.TargetType}: [{this.TargetName}]"
                : $"[{this.PermissionType}] {(this.PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {this.TargetType}: [{this.TargetName}]";

        private string TargetName => string.IsNullOrEmpty(this.RoleName) ? this.UserName : this.RoleName;

        private string TargetType => string.IsNullOrEmpty(this.RoleName) ? "user" : "role";
    }
}
