# EXTENSION API V1

## Purpose

Define the smallest public extension surface needed to test real add-ons without overbuilding a plugin platform.

## Design Principles

- keep the API small
- prefer stable hooks over powerful hooks
- allow behavior customization without exposing core internals
- make host ownership clear

## Supported Contribution Types

Version 1 supports only:

1. `IWindowFilter`
2. `ISortStrategy`

This is intentional.

Renderer plugins, session interception, and custom input hooks stay out of scope for v1.

## Public Contracts

```csharp
public interface IExtension
{
    string Id { get; }
    string Name { get; }
    void Initialize(IHostContext context);
}

public interface IHostContext
{
    void RegisterWindowFilter(IWindowFilter filter);
    void RegisterSortStrategy(ISortStrategy strategy);
    ILogger Logger { get; }
}

public interface IWindowFilter
{
    bool ShouldInclude(WindowInfo window);
}

public interface ISortStrategy
{
    string Id { get; }
    IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows);
}
```

## Loading Model

The host loads extensions from the `extensions/` directory.

### Initial loading approach

- scan `.dll` files in the extensions folder
- find types implementing `IExtension`
- instantiate them
- call `Initialize`

No sandboxing is provided in v1.

## Extension Lifecycle

```text
Host startup
-> discover extension assemblies
-> instantiate extension types
-> call Initialize(context)
-> extension registers filters or sort strategies
-> host uses registered contributions during session creation
```

## Registration Rules

### Window filters

- multiple filters are allowed
- filters are combined by host policy
- simplest host policy: a window must pass all registered filters

### Sort strategies

- only one active strategy should be used at a time
- v1 host may pick the first registered external strategy, or fall back to built-in default

Recommended v1 rule:

- built-in default exists
- external sort strategy can replace it if explicitly selected in code/config

## Failure Behavior

### Extension load failure

If an extension fails to load:

- log the error
- skip the extension
- continue starting the host

### Extension runtime failure

If a filter or sort strategy throws:

- log the error
- skip that contribution for the current session if possible
- do not crash intentionally, though host stability is still best-effort in v1

## Versioning

V1 is informal but should already assume future compatibility pressure.

Guidelines:

- keep interfaces tiny
- avoid leaking WPF types into the public API
- avoid exposing mutable host internals

## Out Of Scope For V1

- manifest files
- dependency metadata
- capability permissions
- hot reloading
- extension UI injection
- renderer plugins
- script-based extensions
- out-of-process isolation

## Example Use Cases

### Filter extension

- hide windows from a specific process
- hide windows with short or empty titles
- show only windows from the current monitor later

### Sort strategy extension

- alphabetical by title
- by process name
- custom recency model later

## Success Criteria

The API is good enough for v1 when:

1. an external extension can be loaded from disk
2. the extension can register at least one filter or sort strategy
3. the host uses that contribution during a real session
4. extension failure does not block host startup
