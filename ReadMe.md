# QuickCompare
__A simple fast database schema comparison library written in C#__

## How it works

Using some SQL queries (mainly targeting the INFORMATION_SCHEMA models), the solution uses low-level asynchronous DataReader instances to populate models in the DatabaseSchema namespace.

Next the engine inspects models of both databases, building a set of Difference objects for each database element.

The Difference objects also act as a report generator, with overridden `ToString()` methods a generated report will list all database differences.

Input parameters are accepted via an `IOptions` implementation, `QuickCompareOptions`.

### Example usage

The `DifferenceBuilder` class generates the report from the `GetDifferenceReport()` method as shown in this example.

```C#
var settings = new QuickCompareOptions
{
    ConnectionString1 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind1;Integrated Security=True",
    ConnectionString2 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Northwind2;Integrated Security=True",
};

IOptions<QuickCompareOptions> options = Options.Create(settings);

var builder = new DifferenceBuilder(options);
builder.BuildDifferences();
string outputText = builder.Differences.ToString();
```

_The options are usually injected from the configuration, but are explicitly created in this example for clarity._

### Status event handling

Inspecting two databases for differences is quick, but it is far from instantaneous. You can measure progress by handling status changes.

The `DifferenceBuilder` class raises an event when the status changes - subscribe to `ComparisonStatusChanged` to return an instance of `StatusChangedEventArgs`. This EventArgs instance has a property named `StatusMessage` which could be presented in a UI layer or used to measure timing of steps.

An example of consuming this event can be found in the sample application (see below).

### Database SQL queries

The SQL queries are located in the folder DatabaseSchema/Queries. They do not require special permission for the master database or anything like that.

### Sample console application

A very basic sample application is in the `ConsoleTestQuickCompare` project. Modify the connection strings in `appsettings.json` to run the application for local testing.

[View sample application](/src/ConsoleTestQuickCompare/ReadMe.md)

### To-Do:

- Complete unit tests
- Refactor DifferenceBuilder
- Compare System-Versioned table properties
- Compare Database properties (e.g. compatibility version)
- Add a forms UI with a comparison view of differences
