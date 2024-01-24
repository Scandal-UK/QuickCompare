// <copyright file="PermissionObjectType.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema.Enums;

/// <summary>
/// Object type that permission applies to.
/// </summary>
public enum PermissionObjectType
{
    /// <summary> Unknown. </summary>
    Unknown,

    /// <summary> Database. </summary>
    Database,

    /// <summary> SQL stored procedure. </summary>
    SqlStoredProcedure,

    /// <summary> SQL function. </summary>
    SqlFunction,

    /// <summary> SQL synonym. </summary>
    Synonym,

    /// <summary> SQL user table. </summary>
    UserTable,

    /// <summary> SQL system table. </summary>
    SystemTable,

    /// <summary> SQL view. </summary>
    View,
}
