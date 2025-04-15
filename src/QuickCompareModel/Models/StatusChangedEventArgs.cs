// <copyright file="StatusChangedEventArgs.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.Models;

using System;

/// <summary> Custom derivative of <see cref="EventArgs"/> to contain a status message. </summary>
[Serializable]
public class StatusChangedEventArgs(string statusMessage)
    : EventArgs
{
    /// <summary> Initialises a new instance of the <see cref="StatusChangedEventArgs"/> class. </summary>
    /// <param name="statusMessage">Current status message.</param>
    /// <param name="databaseInstance">The database in scope.</param>
    public StatusChangedEventArgs(string statusMessage, DatabaseInstance databaseInstance)
        : this(statusMessage) => this.DatabaseInstance = databaseInstance;

    /// <summary> Gets or sets the current status message. </summary>
    public string StatusMessage { get; set; } = statusMessage;

    /// <summary> Gets or sets the database in scope. </summary>
    /// <remarks> Default value is <see cref="DatabaseInstance.Unknown"/>. </remarks>
    public DatabaseInstance DatabaseInstance { get; set; } = DatabaseInstance.Unknown;
}
