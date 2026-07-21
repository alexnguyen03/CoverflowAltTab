# PLAN

## Objective

Ship a maintainable MVP for a Windows Alt+Tab replacement with a real extension hook and one external test extension.

## Scope For MVP

### In scope

- Standalone desktop app
- Basic Alt+Tab session state machine
- Window enumeration and filtering
- Simple overlay UI
- Selection movement with keyboard
- Commit and cancel actions
- Minimal extension contracts
- External extension loading from disk
- One sample extension for local testing later

### Out of scope

- Windhawk integration
- Explorer injection
- Full native Alt+Tab replacement behavior
- Complex 3D rendering
- Sandboxed plugin model
- Multi-monitor perfection
- Stage Manager-like grouping
- Settings UI
- Packaging/distribution polish

## MVP Success Criteria

The MVP is successful when all of the following are true:

1. Pressing the chosen trigger starts a switch session.
2. The app builds a clean list of eligible windows.
3. The overlay shows the list and current selection.
4. `Tab` and `Shift+Tab` update selection correctly.
5. Releasing the modifier commits the selected window.
6. `Esc` cancels without changing focus.
7. The app loads at least one external extension from `extensions/`.
8. One extension changes behavior in a visible way.

## Recommended MVP Architecture

Use a modular monolith with explicit boundaries.

```text
Host
├── App bootstrap
├── Dependency registration
├── Extension loading
└── Session startup

Core
├── Window model
├── Switch session state machine
├── Selection logic
└── Activation workflow

Platform
├── Win32 window enumeration
├── DWM interop
├── Foreground activation
└── Keyboard/hotkey interop

UI
├── Overlay window
├── View models
└── Built-in list renderer

Extensibility
├── Public contracts
├── Extension loader
└── Registry for extension contributions
```

## Why This Architecture Fits Now

### Strengths

- Small enough to implement quickly.
- Clean enough to avoid rewrite after MVP.
- Real plugin boundary from day one.
- Keeps Win32 concerns out of UI code.
- Keeps UI concerns out of switch logic.

### Tradeoffs

- Slightly slower than writing everything directly in one project.
- Public extension interfaces need discipline.
- External plugins can still crash the host in v1.
- Some abstractions will be intentionally simple and may need revision later.

## Extension Strategy For MVP

Do not build a full plugin platform yet.

Start with only these extension points:

1. `IWindowFilter`
2. `ISortStrategy`
3. `IRenderer` or defer renderer plugins until after the first overlay works

### Minimum extension contract set

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

### Recommendation

For the very first pass, expose only:

- `IWindowFilter`
- `ISortStrategy`

Leave `IRenderer` as built-in until the session flow is stable.

## Implementation Phases

## Phase 0 - Foundation

Deliverables:

- Create solution structure
- Create document set
- Decide names for core modules
- Decide first trigger strategy

Notes:

- If true Alt+Tab interception is too heavy at first, use a temporary shortcut for development.
- Keep all public contracts in one small assembly or folder.

## Phase 1 - Session Core

Deliverables:

- `WindowInfo`
- `SwitchSession`
- Selection index logic
- Commit/cancel flow
- In-memory session state machine

Definition of done:

- Session logic can be tested without UI.

## Phase 2 - Platform Integration

Deliverables:

- Enumerate top-level windows
- Basic window filtering
- Foreground activation
- Trigger input for session start and advance

Definition of done:

- App can list and activate real windows on the machine.

## Phase 3 - Overlay UI

Deliverables:

- Always-on-top overlay window
- Basic list rendering
- Selected item highlight
- Show/hide transitions if easy

Definition of done:

- User can see and control the current selection live.

## Phase 4 - Extension Loader

Deliverables:

- Load extension assemblies from `extensions/`
- Find types implementing `IExtension`
- Register contributed filters/sorters
- Log load success/failure

Definition of done:

- An external plugin changes the final window list or order.

## Phase 5 - Coverflow Preparation

Deliverables:

- Separate rendering data from core session state
- Add visual slot model
- Keep list renderer as default
- Prepare coverflow renderer as a built-in experimental mode

Definition of done:

- Coverflow can be added without rewriting session/core code.

## Suggested First Tasks

1. Create the solution and projects.
2. Define `WindowInfo` and `SwitchSession`.
3. Implement window enumeration in `Platform`.
4. Render a basic overlay list.
5. Add extension contracts and loader.
6. Create one sample extension that filters out specific windows.

## Risks

### Technical risks

- Hooking real Alt+Tab too early increases debugging complexity.
- Window filtering is more complex than it looks.
- Some apps may behave badly with activation.
- DWM thumbnail behavior may vary by window type later.

### Product risks

- Over-engineering plugin support before the app works.
- Spending too long on visuals before the switching flow is stable.
- Building public contracts that are too broad too early.

## Guardrails

- Do not start with Explorer injection.
- Do not start with Windhawk.
- Do not make renderer plugins public before the built-in list renderer feels stable.
- Keep v1 extension API tiny and versioned.
