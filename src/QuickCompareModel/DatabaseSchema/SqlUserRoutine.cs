// <copyright file="SqlUserRoutine.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

/// <summary>
/// Class to represent a user function or procedure in the database.
/// </summary>
public class SqlUserRoutine
{
    /// <summary> Gets or sets the routine type. </summary>
    public string RoutineType { get; set; }

    /// <summary> Gets or sets the routine definition. </summary>
    public string RoutineDefinition { get; set; }
}
