// <copyright file="DatabaseInstance.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel
{
    /// <summary> Enumeration to define which database is in scope. </summary>
    public enum DatabaseInstance
    {
        /// <summary>Value is not defined.</summary>
        Unknown,

        /// <summary>The first database in the comparison.</summary>
        Database1,

        /// <summary>The second database in the comparison.</summary>
        Database2,
    }
}
