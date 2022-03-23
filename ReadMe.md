# QuickCompare

[![Build Status](https://github.com/Scandal-UK/QuickCompare/workflows/Build%20and%20Test/badge.svg)](https://github.com/Scandal-UK/QuickCompare/actions?query=workflow%3A%22Build%20and%20Test%22)
[![CodeQL](https://github.com/Scandal-UK/QuickCompare/workflows/CodeQL/badge.svg)](https://github.com/Scandal-UK/QuickCompare/actions?query=workflow%3ACodeQL)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Scandal-UK_QuickCompare&metric=alert_status)](https://sonarcloud.io/dashboard?id=Scandal-UK_QuickCompare)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Scandal-UK_QuickCompare&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=Scandal-UK_QuickCompare)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Scandal-UK_QuickCompare&metric=security_rating)](https://sonarcloud.io/dashboard?id=Scandal-UK_QuickCompare)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Scandal-UK_QuickCompare&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Scandal-UK_QuickCompare)

> __A simple, fast, free SQL database schema comparison library/app written in C#__

- [QuickCompareModel](/src/QuickCompareModel) - the core library and NuGet package source code
- [ConsoleTestQuickCompare](/src/ConsoleTestQuickCompare) - sample .NET 6 console application
- [WinQuickCompare](/src/WinQuickCompare) - sample .NET 6 Windows application

This package interrogates the schema of two Microsoft SQL Server databases and reports on the differences between them. There is a front-end for Windows users and a NuGet package for .NET developers and DevOps engineers.

It is free to use _without any restrictions_ (and always will be), but I do encourage everyone to contribute improvements to the project and raise issues as appropriate. If you find this project useful, considering clicking the star to add to your favourites!

## How it works

Using some SQL queries (mainly targeting the INFORMATION_SCHEMA views), the solution uses low-level asynchronous DataReader instances to populate models in the DatabaseSchema namespace.

Next the engine inspects models of both databases, building a set of Difference objects for each database element.

The Difference objects also act as a report generator, with overridden `ToString()` methods a generated report will list all database differences.

Input parameters are accepted via the `IOptions` implementation; `QuickCompareOptions`. This aids automation because it allows the application to be easily run by anything that can provide application settings (such as Azure Pipelines).

## Purpose

I know there are many alternatives, but the best ones are not cheap (e.g. RedGate). Many paid solutions do not fit my purposes, mainly because of the amount of false-positive results or even differences that are not detected at all.

Finally, nobody seems to offer a _simple database comparison solution_ that meets my needs for free - even less so if you want to use them programmatically with C#.

So here is a __free alternative__ that will remain open-source and fully unit-tested for us all to enjoy.

It is lean enough to be used in CI/CD pipelines and as it targets information schema views, it does not negatively impact the performance of the databases under interrogation.

Please contribute to the source code if you have the time/knowledge to spare!

### Example usage

The `DifferenceBuilder` class builds the report from the `Differences` collection as shown in this example.

```C#
var settings = new QuickCompareOptions
{
    ConnectionString1 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind1;Integrated Security=True",
    ConnectionString2 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind2;Integrated Security=True",
};

IOptions<QuickCompareOptions> options = Options.Create(settings);

var builder = new DifferenceBuilder(options);
await builder.BuildDifferencesAsync();
string outputText = builder.Differences.ToString();
```

_The options are usually injected from the configuration, but are explicitly created in this example for clarity._

### Status event handling

Inspecting two databases for differences is quick, but it is far from instantaneous. You can measure progress by handling status changes.

The `DifferenceBuilder` class raises an event when the status changes - subscribe to `ComparisonStatusChanged` to return an instance of `StatusChangedEventArgs`. This EventArgs instance has a property named `StatusMessage` which could be presented in a UI layer or used to measure timing of steps.

An example of consuming this event can be found in the sample console application.

### Database SQL queries

The SQL queries are located in the folder DatabaseSchema/Queries. These have been written for backwards-compatibility with SQL Server 2000.

The queries do not require special permission for the master database and have minimal impact on database performance, even during busy periods.

---

_Consider this a pre-release version until the first release appears on the GitHub project page!_

#### To-Do:
- Display status updates in Windows app UI
- Compare System-Versioned table properties
- Compare Database properties (e.g. compatibility version)
- Add "upload report" feature for DevOps (XML/JSON?)
- Change Windows UI to display a tree view or similar(?)
