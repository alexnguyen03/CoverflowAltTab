# MVP SPEC

## Purpose

Define the exact behavior of the first working version so implementation can stay focused and testable.

## MVP Goal

Build a standalone Windows desktop app that:

1. Starts a window switching session with a development hotkey.
2. Enumerates eligible windows.
3. Displays a simple overlay list.
4. Moves selection with keyboard input.
5. Activates the selected window when the session is committed.
6. Cancels cleanly without changing focus.
7. Loads at least one external extension that changes filtering or sorting behavior.

## Development Trigger

Use a temporary development trigger instead of replacing native `Alt+Tab` immediately.

### Chosen trigger

`Ctrl + Alt + Space`

Reason:

- avoids conflicting with the native switcher during development
- easier to debug
- allows the app to mature before deep keyboard interception work

## Session Flow

```text
Idle
-> Trigger pressed
-> Build window list
-> Show overlay
-> Move selection with Tab / Shift+Tab
-> Enter commits selection
-> Esc cancels session
-> Overlay closes
-> Return to Idle
```

## Commit And Cancel Rules

### Commit

The session commits when:

- `Enter` is pressed while the overlay is open

Commit result:

- activate the currently selected window
- close the overlay
- clear current session state

### Cancel

The session cancels when:

- `Esc` is pressed while the overlay is open
- the session becomes invalid due to no eligible windows

Cancel result:

- do not change the foreground window
- close the overlay
- clear current session state

## Selection Rules

- If the window list is not empty, initial selection is index `0`.
- `Tab` moves forward and wraps.
- `Shift+Tab` moves backward and wraps.
- If only one window exists, selection stays on that item.

## UI Scope

### Included in MVP

- always-on-top overlay
- simple list layout
- selected item highlight
- title text
- optional process name if easy

### Not included in MVP

- coverflow rendering
- live thumbnails
- blur and fancy animation
- settings UI
- mouse interaction polish

## Window Eligibility Rules

The app should include only user-facing top-level windows.

### Include when possible

- visible top-level windows
- windows with meaningful title text
- windows that can reasonably be activated by the user

### Exclude initially

- invisible windows
- windows with empty titles
- tool windows
- shell helper windows
- cloaked or system-only windows if detectable easily
- the switcher app's own window

## Extension Scope In MVP

Extensions may change:

- which windows are included
- how windows are sorted

Extensions may not change yet:

- session state machine
- keyboard routing
- activation logic
- host startup

## Logging Expectations

The host should log:

- trigger received
- number of windows discovered
- number of windows after filtering
- active sort strategy
- extension load success/failure
- commit/cancel result

## Definition Of Done

The MVP is done when:

1. the app starts and listens for the development trigger
2. the overlay opens reliably
3. the overlay shows a filtered list of real windows
4. selection movement works
5. commit activates the target window
6. cancel leaves focus unchanged
7. one external extension is loaded and visibly affects behavior
