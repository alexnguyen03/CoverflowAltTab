# ARCHITECTURE

## Architectural Style

Use a modular monolith with a narrow extension surface.

This means:

- one app
- a few clearly separated modules
- no distributed complexity
- a small public API for extensions

## Module Boundaries

## Host

Responsibilities:

- application startup
- dependency wiring
- extension discovery and load
- composition root

Should not contain:

- window enumeration logic
- rendering rules
- selection logic

## Core

Responsibilities:

- `WindowInfo`
- switch session lifecycle
- selected index management
- commit/cancel rules
- session state machine

Should not depend on:

- WPF
- Win32 P/Invoke details

## Platform

Responsibilities:

- Win32 interop
- DWM interop
- hotkeys/hooks
- foreground activation
- OS-specific window metadata

Should not decide:

- product policy
- render behavior

## UI

Responsibilities:

- overlay window
- visual state binding
- built-in list rendering
- future built-in coverflow rendering

Should not own:

- session truth
- extension discovery

## Extensibility

Responsibilities:

- extension contracts
- extension loading
- contribution registration
- API version boundary

Should stay small in v1.

## Recommended V1 Extension Surface

```csharp
public interface IExtension
{
    string Id { get; }
    string Name { get; }
    void Initialize(IHostContext context);
}

public interface IWindowFilter
{
    bool ShouldInclude(WindowInfo window);
}

public interface ISortStrategy
{
    IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows);
}
```

## Data Flow

```text
Input
-> Host starts session
-> Platform enumerates windows
-> Built-in and extension filters run
-> Sort strategy runs
-> Core updates selection state
-> UI renders current state
-> Commit activates selected window
```

## Why Not A Bigger Plugin System Yet

- The app still needs to prove its core interaction model.
- More plugin surfaces mean more unstable contracts.
- Rendering plugins are harder to debug than filter/sort plugins.
- Safety and isolation are not required for the first self-developed extension test.

## Evolution Path

### Today

- modular monolith
- DLL-based extension loading
- minimal contracts

### Next

- versioned extension API
- optional manifest files
- renderer contributions

### Later

- event-based extension hooks
- richer diagnostics
- stronger isolation if needed
