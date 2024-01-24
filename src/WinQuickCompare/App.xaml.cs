// <copyright file="App.xaml.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompare;

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Threading;

/// <summary> Global events for the application. </summary>
public partial class App : Application
{
    private const string Filename = "QuickCompare.Settings.txt";

    private void App_Startup(object sender, StartupEventArgs e)
    {
        this.ReadApplicationPropertiesFromFile();
        AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;
    }

    private void App_Exit(object sender, ExitEventArgs e)
    {
        this.WriteApplicationPropertiesToFile();
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _ = MessageBox.Show(this.MainWindow, $"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        e.Handled = true;
    }

    private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _ = MessageBox.Show(this.MainWindow, $"An unhandled exception occurred: {((Exception)e.ExceptionObject).Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    private void ReadApplicationPropertiesFromFile()
    {
        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
        using IsolatedStorageFileStream stream = new(Filename, FileMode.OpenOrCreate, storage);
        using StreamReader reader = new(stream);
        while (!reader.EndOfStream)
        {
            this.SetPropertyFromLineOfText(reader.ReadLine());
        }
    }

    private void SetPropertyFromLineOfText(string line)
    {
        string key = line[..line.IndexOf(',')];
        string value = line[(line.IndexOf(',') + 1)..];
        this.Properties[key] = value;
    }

    private void WriteApplicationPropertiesToFile()
    {
        using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
        using IsolatedStorageFileStream stream = new(Filename, FileMode.Create, storage);
        using StreamWriter writer = new(stream);
        foreach (string key in this.Properties.Keys)
        {
            writer.WriteLine("{0},{1}", key, this.Properties[key]);
        }
    }
}
