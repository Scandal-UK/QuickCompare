# QuickCompare
__A simple fast database schema comparison library written in C#__

- [QuickCompareModel](/src/QuickCompareModel) - the core library
- [ConsoleTestQuickCompare](/src/ConsoleTestQuickCompare) - sample console application
- [QuickCompare](/src/QuickCompare) - sample Windows application _(new!)_

## How it works

Using some SQL queries (mainly targeting the INFORMATION_SCHEMA models), the solution uses low-level asynchronous DataReader instances to populate models in the DatabaseSchema namespace.

Next the engine inspects models of both databases, building a set of Difference objects for each database element.

The Difference objects also act as a report generator, with overridden `ToString()` methods a generated report will list all database differences.

Input parameters are accepted via an `IOptions` implementation, `QuickCompareOptions`.

### Why write this??

I know there are many alternatives, but the best ones are not cheap (e.g. RedGate). Many paid solutions do not fit my purposes, mainly because of the amount of false-positive results or even differences that are not detected at all.

Finally, nobody seems to offer a _simple database comparison solution_ that meets my needs for free - even less so if you want to use them programmatically with C#.

So here is a __free alternative__ that will remain open-source and fully unit-tested for us all to enjoy.

Please contribute to the source code if you have time to spare!

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
builder.BuildDifferences();
string outputText = builder.Differences.ToString();
```

_The options are usually injected from the configuration, but are explicitly created in this example for clarity._

### Status event handling

Inspecting two databases for differences is quick, but it is far from instantaneous. You can measure progress by handling status changes.

The `DifferenceBuilder` class raises an event when the status changes - subscribe to `ComparisonStatusChanged` to return an instance of `StatusChangedEventArgs`. This EventArgs instance has a property named `StatusMessage` which could be presented in a UI layer or used to measure timing of steps.

An example of consuming this event can be found in the sample console application.

### Database SQL queries

The SQL queries are located in the folder DatabaseSchema/Queries. They do not require special permission for the master database or anything like that.

---

Consider this a pre-release version until the first release appears on the GitHub project page!

#### To-Do:
- Refactor DifferenceBuilder
- Compare System-Versioned table properties
- Compare Database properties (e.g. compatibility version)
