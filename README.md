# Egonsoft.HU Serilog Logging Extensions

[![GitHub](https://img.shields.io/github/license/gcsizmadia/EgonsoftHU.Extensions.Logging.Serilog?label=License)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/EgonsoftHU.Extensions.Logging.Serilog?label=NuGet)](https://www.nuget.org/packages/EgonsoftHU.Extensions.Logging.Serilog)
[![Nuget](https://img.shields.io/nuget/dt/EgonsoftHU.Extensions.Logging.Serilog?label=Downloads)](https://www.nuget.org/packages/EgonsoftHU.Extensions.Logging.Serilog)

`PropertyBagEnricher` and C# extension methods for `Serilog.ILogger`.

## Introduction

The motivation behind this project is as follows:

- Enrich log events with the name of the source member (method, property, etc.) in which the log event occurs.
  - `ILogger.Here()` extension method.
  - `PropertyBagEnricher.CreateForSourceMember()` static factory method.
  - `PropertyBagEnricher.Here()` instance method.
- Enrich log events with multiple properties at once using `PropertyBagEnricher`.
- Populate `Exception.Data` dictionary with the log event properties.
  - `Exception.Populate(PropertyBagEnricher)` extension method.

## Releases

You can download the package from **nuget.org**: [EgonsoftHU.Extensions.Logging.Serilog](https://www.nuget.org/packages/EgonsoftHU.Extensions.Logging.Serilog)

You can find the release notes [here](https://github.com/gcsizmadia/logging/releases).

[Releases](RELEASES.md) under active support or development:

|Version|Release Date|Latest Patch Version|End of Support|Targets|
|-|-|-|-|-|
|10.0|2026-10-20|10.0.0|2028-11-14|`net8.0`, `net9.0`, `net10.0`<br/>`net472`<br/>`netstandard2.0`, `netstandard2.1`|

### Breaking change

The project namespace has changed to avoid namespace collision with `Serilog`.  
**Please note:** The package id will remain unchanged.

|Version|Namespace|
|-|-|
|`>= 1.0.0 && < 2.0.0`|`EgonsoftHU.Extensions.Logging.Serilog`|
|`>= 2.0.0`|`EgonsoftHU.Extensions.Logging`|

## Summary

### SourceMember property

When you create a logger by calling `Log.Logger.ForContext<T>()` then the log events will contain a
`SourceContext` property with the type name where the log event occurs.

This project helps you add a `SourceMember` property with the member name of the type where the log event occurs.

The `SourceMember` property name can be customized. See examples below.

### PropertyBagEnricher : Serilog.Core.ILogEventEnricher

Each time you add a log event property (e.g. `logger.ForContext("SomeProperty", 42)`) a new logger instance will be created.

This enricher helps you reducing creating logger instances by adding multiple log event properties at once.

### Exception.Data population

`Exception.Data` dictionary can contain additional user-defined information about the exception.

The log event properties the enricher contains can be populated into the `Exception.Data` dictionary using the `Exception.Populate(PropertyBagEnricher)` extension method. See example below.

## Examples

### ILogger.Here() - single use per method

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

namespace SomeCompany.Services;

public class SomeService
{
    private readonly ILogger logger;

    // To inject a contextual logger using Autofac:
    // https://github.com/gcsizmadia/EgonsoftHU.Extensions.Logging.Serilog.Autofac
    public SomeService(ILogger logger)
    {
        this.logger = logger;
        logger.Here().Verbose("SomeService has been created");
    }

    public void DoSomething()
    {
        logger.Here().Information("We are doing something here");
    }
}
```

The log events and their properties will be:

```json
{"SourceContext":"SomeCompany.Services.SomeService","SourceMember":".ctor","@l":"Verbose","@mt":"SomeService has been created","@t":"2022-05-08T12:34:56.1111111Z"}
{"SourceContext":"SomeCompany.Services.SomeService","SourceMember":"DoSomething","@l":"Information","@mt":"We are doing something here","@t":"2022-05-08T12:34:56.2222222Z"}
```

### ILogger.Here() - multiple use per method

To reduce creating logger instances you can store the logger instance created by calling the `Here()` method.

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

private void DoComplexThings()
{
    ILogger logger = this.logger.Here();

    // All the log events below will contain this log event property:
    // "SourceMember": "DoComplexThings"

    logger.Verbose("Preparing to do complex things");
    logger.Warning("Configuration not found, using default settings");
    logger.Verbose("Prepared to do complex things");
    logger.Information("Complex things done");
}
```

### PropertyBagEnricher

To further reduce creating logger instances you can use the `PropertyBagEnricher` to add multiple log event properties at once.

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

private void CalculateRectangleArea(int a, int b)
{
    logger
        .Here()
        .ForContext(
            PropertyBagEnricher
                .Create()
                .Add("SideA", a)
                .Add("SideB", b)
        )
        .Information("Rectangle area: {RectangleArea}", a * b);
}
```

You can also combine `Here()` with `PropertyBagEnricher` by using the `CreateForSourceMember()` factory method:

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

private void CalculateRectangleArea(int a, int b)
{
    logger
        .ForContext(
            PropertyBagEnricher
                .CreateForSourceMember() // this will add the SourceMember property.
                .Add("SideA", a)
                .Add("SideB", b)
        )
        .Information("Rectangle area: {RectangleArea}", a * b);
}
```

Of course, you can store the enricher in a variable if you want to add other properties later:

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

private void CalculateRectangleArea(int a, int b)
{
    var enricher =
        PropertyBagEnricher
            .CreateForSourceMember() // this will add the SourceMember property.
            .Add("SideA", a)
            .Add("SideB", b);

    ILogger logger = this.logger.ForContext(enricher);

    logger.Information("Rectangle area: {RectangleArea}", a * b);
    
    enricher.Add("RectangleArea", a * b);
    
    // this log event will also contain the RectangleArea property.
    logger.Verbose("Rectangle area calculated");
}
```

You can also add multiple properties at once:

```csharp
using EgonsoftHU.Extensions.Logging;

var propertiesFromOtherSource = new Dictionary<string, object?>()
{
    ["SideA"] = 5,
    ["SideB"] = 10
};

var enricher =
    PropertyBagEnricher
        .CreateForSourceMember()
        .AddRange(propertiesFromOtherSource);
```

**Please note:**
- `AddRange<TValue>()` method expects a single parameter.
- The parameter type is `IEnumerable<KeyValuePair<string, TValue>>`.
- `TValue` generic type parameter has no generic constraint, hence, both nullable and non-nullable types can be used as the type of the values. E.g.:
  - `Dictionary<string, object>`
  - `Dictionary<string, object?>`

### Exception.Data - populate log event properties

Instead of manually providing the additional information:

```csharp
var ex = new ArithmeticException("Cannot calculate area.");
ex.Data["SideA"] = a;
ex.Data["SideB"] = b;

throw ex;
```

you can use your existing `PropertyBagEnricher` instance:

```csharp
using EgonsoftHU.Extensions.Logging;

var enricher =
    PropertyBagEnricher
        .Create()
        .Add("SideA", a)
        .Add("SideB", b);

// Exception.Data dictionary will contain all the properties above.
// The extension method returns the exception itself,
// hence you can use it directly in a `throw` statement or expression.
throw new ArithmeticException("Cannot calculate area.").Populate(enricher);
```

### SourceMember property customization

As the contextual logger uses the `SourceContext` property name for the type, it seems to be logical to use the `SourceMember` property name for the member in that type.

Should you need to customize the log event property name for whatever reason, use the `SourceMemberNamingConfiguration.Initialize()` method as below:

```csharp
using EgonsoftHU.Extensions.Logging;
using Serilog;

namespace ConsoleApp;

class Program
{
    private static ILogger logger;

    public static void Main(string[] args)
    {
        SourceMemberNamingConfiguration.Initialize("CallerMemberName");

        /* Log.Logger configuration omitted for clarity */

        logger = Log.Logger.ForContext<Program>();

        // The log event property will be { "CallerMemberName": "Main" }
        logger.Here().Information("First message");
    }
}
```

**Please note:**
- This setting is global within the application.
- Unless `Initialize()` method is called, the default property name (`SourceMember`) will be used.
- This method may be called only once, preferably before you log anything. Subsequent calls will throw an `InvalidOperationException`.
