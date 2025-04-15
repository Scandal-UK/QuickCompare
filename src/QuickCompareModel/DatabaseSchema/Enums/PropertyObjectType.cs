// <copyright file="PropertyObjectType.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema.Enums;

/// <summary> Object type that extended property applies to. </summary>
public enum PropertyObjectType
{
    /// <summary> Database. </summary>
    Database,

    /// <summary> Routine. </summary>
    Routine,

    /// <summary> Routine column. </summary>
    RoutineColumn,

    /// <summary> Table. </summary>
    Table,

    /// <summary> Table column. </summary>
    TableColumn,

    /// <summary> Index. </summary>
    Index,
}
