# ROADMAP

## Product Vision

Build a Windows window switcher that starts as a reliable Alt+Tab replacement and grows into an extension-ready switching framework.

## Development Roadmap

## Milestone 1 - Architecture-First MVP

Goal:

Create a working switcher with clean boundaries and one real external extension hook.

Expected output:

- Standalone WPF app
- Core session engine
- Window enumeration
- Basic overlay list
- External extension loading

Features:

- Show overlay on trigger
- Cycle selection
- Commit and cancel
- Load one extension from disk
- Extension modifies filtering or sorting

Exit criteria:

- End-to-end demo works consistently on a development machine.

## Milestone 2 - Better Window Quality

Goal:

Make the switcher feel less like a prototype and more like a real desktop tool.

Features:

- Better filtering rules
- Title/process/icon metadata
- Handling for minimized windows
- Better ordering strategy
- Optional current-monitor behavior

Exit criteria:

- The window list feels trustworthy in day-to-day use.

## Milestone 3 - Thumbnail MVP

Goal:

Introduce live previews without destabilizing the architecture.

Features:

- DWM thumbnail service
- Thumbnail host control
- Preview fallback states
- Preview lifecycle management

Exit criteria:

- At least the selected item can show a reliable live thumbnail.

## Milestone 4 - Built-In Coverflow Renderer

Goal:

Move from list switching to visual switching.

Features:

- Visual slot model
- Built-in coverflow renderer
- Keyboard-driven animation
- Center-focused selection
- Basic easing and z-ordering

Exit criteria:

- Switching feels visually distinct and still functionally solid.

## Milestone 5 - Extension Expansion

Goal:

Make the project genuinely extensible without committing too early to unsafe surfaces.

Features:

- Versioned extension contracts
- Extension manifests if needed
- Event hooks for session lifecycle
- Built-in and external filter/sort/renderer registration
- Basic diagnostics for plugin loading

Exit criteria:

- A second custom extension can be added without changing host code.

## Milestone 6 - Native-Like Integration

Goal:

Get closer to a seamless system experience.

Features:

- Better keyboard interception
- Closer Alt+Tab parity
- More stable activation behavior
- Improved overlay timing
- Optional sticky mode

Exit criteria:

- The app feels safe enough for daily use outside test mode.

## Milestone 7 - Shell/Injection Experiments

Goal:

Only after the standalone app is stable, evaluate deeper integration.

Features:

- Windhawk feasibility study
- Explorer lifecycle mapping
- Reuse of core and renderer modules
- Safety checks and rollback strategy

Exit criteria:

- Clear decision on whether shell injection is worth the maintenance cost.

## Recommended Backlog Order

1. Solution skeleton
2. Core session state
3. Window enumeration
4. Overlay list UI
5. Input trigger loop
6. Activation/cancel flow
7. Extension loader
8. Sample extension
9. Better filtering
10. Thumbnail experiment
11. Coverflow renderer

## Extension Roadmap

### Extension API v1

Purpose:

- prove external contributions work

Hooks:

- `IWindowFilter`
- `ISortStrategy`

### Extension API v2

Purpose:

- allow richer UI and behavior customization

Hooks:

- `ISessionObserver`
- `IRenderer`
- `ICommandContribution`

### Extension API v3

Purpose:

- allow advanced community add-ons

Hooks:

- workspace/grouping policies
- custom thumbnail decorators
- automation and scripting

## Suggested Documentation To Add Next

- `ARCHITECTURE.md`
- `EXTENSION_API.md`
- `MVP_CHECKLIST.md`
- `DECISIONS.md`
