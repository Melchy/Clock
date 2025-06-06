# OBSOLETE

This package is now obsolete. Use [TimeProvider](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview) and [BannedApi analyzer](https://stackoverflow.com/a/77051808) to replace the included annalyzer. 

# Clock

This package provides thread safe testable global `DateTimeUtc` provider.

## NuGet

[Clock](https://www.nuget.org/packages/Clock)

[ClockTesting](https://www.nuget.org/packages/ClockTesting)

## How to use

Install package [Clock](https://www.nuget.org/packages/Clock) and use ``Clock.UtcNow`` instead of ``DateTimeOffest.UtcNow``.

In your test project install package [ClockTesting](https://www.nuget.org/packages/ClockTesting) and mock calls to `Clock.UtcNow` like this:

```csharp
using (new OverrideClock(DateTime.MinValue)) //Clock.UtcNow will provide DateTime.MinValue in this using
{
    DateTimeOffset timeNow = Clock.UtcNow; // timeNow is now set to DateTime.MinValue
}
```

## Analyzer

Clock package contains analyzer which detects all calls to ``DateTimeOffest.UtcNow``, ``DateTimeOffest.Now``, ``DateTime.UtcNow``, ``DateTime.Now`` and suggests replacing with ``Clock.UtcNow``.

This analyzer is very restrictive to help programmer avoid mistakes. If you need less restrictive provider use [this package](https://github.com/dennisroche/DateTimeProvider).

## How it works

Clock package contains static class Clock. Clock has internal static property which provides current time. This property is set by default to ``DateTimeOffset.UtcNow()``. Clock has internals visible to ClockTesting which allows ClockTesting package set Time provider. OverrideClock then overrides clock provider and Dispose method restarts it back to ``DateTimeOffset.UtcNow()``.
