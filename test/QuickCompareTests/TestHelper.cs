// <copyright file="TestHelper.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareTests;

using Microsoft.Extensions.Options;
using QuickCompareModel;
using QuickCompareModel.DatabaseSchema;
using QuickCompareModel.Models;

/// <summary>
/// Class to provide common setup methods for tests.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Gets a <see cref="DifferenceBuilder"/> with the table name and column name both specified.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>Instance of <see cref="DifferenceBuilder"/>.</returns>
    public static DifferenceBuilder GetBuilderWithSingleTable(string tableName, string columnName)
    {
        var builder = GetBasicBuilder();
        builder.Database1.Tables.Add(tableName, new SqlTable());
        builder.Database1.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });
        builder.Database2.Tables.Add(tableName, new SqlTable());
        builder.Database2.Tables[tableName].ColumnDetails.Add(new SqlColumnDetail { ColumnName = columnName });

        return builder;
    }

    /// <summary>
    /// Gets a <see cref="DifferenceBuilder"/>.
    /// </summary>
    /// <returns>Instance of <see cref="DifferenceBuilder"/>.</returns>
    public static DifferenceBuilder GetBasicBuilder()
    {
        var options = GetDefaultOptions();

        var database1 = new SqlDatabase(options.Value.ConnectionString1);
        var database2 = new SqlDatabase(options.Value.ConnectionString2);

        return new DifferenceBuilder(options, database1, database2);
    }

    private static IOptions<QuickCompareOptions> GetDefaultOptions()
    {
        var settings = new QuickCompareOptions
        {
            ConnectionString1 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind1;Integrated Security=True",
            ConnectionString2 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind2;Integrated Security=True",
        };

        return Options.Create(settings);
    }
}
