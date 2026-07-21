# CoverflowAltTab

Coverflow-style Alt+Tab replacement for Windows, designed as a clean MVP first and an extension-ready platform second.

## Current Goal

Build a working desktop app that can:

1. Detect and manage an Alt+Tab switching session.
2. Enumerate user-facing windows.
3. Show a simple overlay UI.
4. Move selection with keyboard input.
5. Activate the selected window on commit.
6. Load at least one external extension for testing.

## Product Direction

This project is intentionally split into two layers:

- `MVP app`: prove the switching flow works and stays maintainable.
- `Extension-ready host`: expose a small, stable surface so custom behavior can be added without rewriting the core.

## MVP Principles

- Keep the first version simple and debuggable.
- Prefer a modular monolith over early micro-modules.
- Expose only a few extension hooks at first.
- Delay deep shell integration and Explorer/Windhawk work until the standalone app is stable.

## Proposed Tech Direction

- Language/UI: `C# + WPF`
- Runtime: `.NET`
- Platform interop: `Win32 + DWM`
- Extension model v1: external `.dll` plugins loaded by the host

## Development Commands

Restore dependencies:

```powershell
dotnet restore .\CoverflowAltTab.sln
```

Build the full solution:

```powershell
dotnet build .\CoverflowAltTab.sln -m:1
```

Run the host app in development:

```powershell
dotnet run --project .\src\CoverflowAltTab.Host\CoverflowAltTab.Host.csproj
```

Build the sample extension into the host `extensions/` folder:

```powershell
dotnet build .\extensions\SampleExtension\SampleExtension.csproj -m:1
```

Recommended dev flow:

```powershell
dotnet build .\extensions\SampleExtension\SampleExtension.csproj -m:1
dotnet run --project .\src\CoverflowAltTab.Host\CoverflowAltTab.Host.csproj
```

## Suggested Repository Layout

```text
CoverflowAltTab/
├── docs/
│   ├── PLAN.md
│   ├── ROADMAP.md
│   └── ARCHITECTURE.md
├── src/
│   ├── Host/
│   ├── Core/
│   ├── Platform/
│   ├── UI/
│   ├── Extensibility/
│   └── BuiltInExtensions/
└── extensions/
    └── SampleExtension/
```

## First Milestone

The first milestone is not "full coverflow".

It is:

```text
Alt down
-> show overlay
-> Tab cycles selection
-> Alt up commits selected window
-> external extension loads successfully
```

Current development hotkey:

```text
Ctrl + Shift + Space
```
