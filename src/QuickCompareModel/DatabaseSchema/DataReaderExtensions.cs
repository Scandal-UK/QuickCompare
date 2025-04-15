// <copyright file="DataReaderExtensions.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using Microsoft.Data.SqlClient;

/// <summary> Set of extensions to improve cognitive complexity when dealing with DBNull to null conversions. </summary>
public static class DataReaderExtensions
{
    /// <summary>Gets the value of the specified column as a string or a null.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public static string GetNullableString(this SqlDataReader dr, int index) => dr.IsDBNull(index) ? null : dr.GetString(index);

    /// <summary>Gets the value of the specified column as a byte or a null.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public static int? GetNullableByte(this SqlDataReader dr, int index) => dr.IsDBNull(index) ? null : (int?)dr.GetByte(index);

    /// <summary>Gets the value of the specified column as a decimal or a null.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public static decimal? GetNullableDecimal(this SqlDataReader dr, int index) => dr.IsDBNull(index) ? null : (decimal?)dr.GetDecimal(index);

    /// <summary>Gets the value of the specified column as an Int16 or a null.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public static int? GetNullableInt16(this SqlDataReader dr, int index) => dr.IsDBNull(index) ? null : (int?)dr.GetInt16(index);

    /// <summary>Gets the value of the specified column as an Int32 or a null.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public static int? GetNullableInt32(this SqlDataReader dr, int index) => dr.IsDBNull(index) ? null : (int?)dr.GetInt32(index);

    /// <summary>Gets the value of the specified column as an Int32 and returns a Boolean.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The interpreted value of the specified column.</returns>
    public static bool GetNullableInt32AsBoolean(this SqlDataReader dr, int index) => !dr.IsDBNull(index) && dr.GetInt32(index) == 1;

    /// <summary>Gets the value of the specified column as an Int32 and returns a Boolean.</summary>
    /// <param name="dr">The current instance of <see cref="SqlDataReader"/>.</param>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The interpreted value of the specified column.</returns>
    public static bool GetInt32AsBoolean(this SqlDataReader dr, int index) => dr.GetInt32(index) == 1;
}
