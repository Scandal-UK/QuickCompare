﻿// <copyright file="Program.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace ConsoleTestQuickCompare;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using QuickCompareModel;
using QuickCompareModel.Models;

/// <summary> Main program loop. </summary>
internal static class Program
{
    /// <summary> Entrance of program loop. </summary>
    internal static void Main()
    {
        try
        {
            var provider = GetServiceProvider();
            var builder = provider.GetService<IDifferenceBuilder>();

            builder.ComparisonStatusChanged += OnComparisonStatusChanged;
            builder.BuildDifferencesAsync().Wait();

            Console.WriteLine("\r\n--------------------------------");

            string report = builder.Differences.ToString();
            Console.Write(report);
            Trace.Write(report);
        }
        catch (Exception ex)
        {
            Console.Write(ex);
            Trace.Write(ex);
        }
        finally
        {
            Console.ReadKey();
        }
    }

    /// <summary> Gets the <see cref="IServiceProvider"/> DI container configured in <see cref="Startup"/>. </summary>
    /// <returns> An instance of <see cref="IServiceProvider"/>. </returns>
    private static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        new Startup().ConfigureServices(services);
        return services.BuildServiceProvider();
    }

    private static void OnComparisonStatusChanged(object sender, StatusChangedEventArgs e)
    {
        string message = e.DatabaseInstance switch
        {
            DatabaseInstance.Database1 => $"{e.StatusMessage} for database 1",
            DatabaseInstance.Database2 => $"{e.StatusMessage} for database 2",
            _ => e.StatusMessage,
        };

        Console.Write($"\r{message,-70}");
        Trace.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.ff}] {message}");
    }
}
