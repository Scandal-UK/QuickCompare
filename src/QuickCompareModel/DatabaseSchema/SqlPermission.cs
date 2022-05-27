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

        public string FullId => $"[{RoleName}].[{UserName}].[{PermissionType}].[{PermissionState}].[{ObjectType}].[{ObjectName}].[{ColumnName}]";

        public PermissionObjectType Type => ObjectType switch
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

        public override string ToString() => PermissionType == "REFERENCES"
                ? $"REFERENCES column: [{ColumnName}] {(PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {TargetType}: [{TargetName}]"
                : $"[{PermissionType}] {(PermissionState == "GRANT" ? string.Empty : "DENIED ")}for {TargetType}: [{TargetName}]";

        private string TargetName => string.IsNullOrEmpty(RoleName) ? UserName : RoleName;

        private string TargetType => string.IsNullOrEmpty(RoleName) ? "user" : "role";
    }
}
