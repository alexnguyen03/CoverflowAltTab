# DECISIONS

## Purpose

Record the early architectural decisions so implementation stays aligned and future changes remain explicit.

## Decision 001 - Build As A Standalone App First

Status:

- accepted

Decision:

- start with a standalone desktop application
- do not begin with Explorer injection or Windhawk

Why:

- faster iteration
- safer debugging
- lower complexity for MVP

Consequence:

- native-feeling system replacement comes later
- some input behavior will initially be development-oriented rather than production-like

## Decision 002 - Use C# + WPF For MVP

Status:

- accepted

Decision:

- use `C# + WPF` for the first implementation

Why:

- high development speed
- good fit for desktop overlay UI
- easy to separate Win32 interop from UI code
- sufficient for initial rendering and state management

Consequence:

- low-level shell integration may later require deeper native work
- some performance-sensitive paths may need optimization or partial migration later

## Decision 003 - Use A Modular Monolith

Status:

- accepted

Decision:

- keep one app with clear internal boundaries instead of many isolated systems

Why:

- simplest architecture that still scales
- easier debugging
- keeps refactoring cost low during MVP

Consequence:

- internal discipline matters
- extension API must be intentionally separated from internal code

## Decision 004 - Support Real External Extensions In MVP

Status:

- accepted

Decision:

- include a small but real extension loading path in the MVP

Why:

- validates the future platform direction early
- prevents the codebase from becoming closed and tangled
- allows local developer testing of add-ons immediately

Consequence:

- some extra upfront design work is required
- extension boundaries must be documented before coding too much

## Decision 005 - Keep Extension API V1 Minimal

Status:

- accepted

Decision:

- expose only `IWindowFilter` and `ISortStrategy` in extension API v1

Why:

- low risk
- easy to test
- avoids freezing an immature renderer API too early

Consequence:

- renderer extensibility is deferred
- early external extensions are behavior-oriented, not UI-oriented

## Decision 006 - Use A Development Trigger First

Status:

- accepted

Decision:

- use `Ctrl + Alt + Space` as the first trigger instead of replacing native `Alt+Tab`

Why:

- avoids fighting the operating system too early
- keeps debugging manageable
- lets the switching flow mature before deep keyboard interception work

Consequence:

- the first usable build is a development-mode switcher, not a drop-in system replacement

## Decision 007 - Start With A List Overlay

Status:

- accepted

Decision:

- build a list-based overlay before implementing coverflow rendering

Why:

- proves the switching workflow first
- easier to debug state and activation logic
- avoids visual complexity blocking core progress

Consequence:

- the MVP will not yet showcase the final visual identity
- coverflow becomes a milestone after session flow is stable
