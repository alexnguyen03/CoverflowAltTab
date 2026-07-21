namespace CoverflowAltTab.Core.Models;

public sealed class SwitchSession
{
    public SwitchSession(
        IReadOnlyList<WindowInfo> windows,
        nint originalForegroundWindowHandle,
        string activeSortStrategyId)
    {
        if (windows.Count == 0)
        {
            throw new ArgumentException("Session requires at least one window.", nameof(windows));
        }

        SessionId = Guid.NewGuid();
        Windows = windows;
        OriginalForegroundWindowHandle = originalForegroundWindowHandle;
        ActiveSortStrategyId = activeSortStrategyId;
        SelectedIndex = 0;
        State = SwitchSessionState.Open;
    }

    public Guid SessionId { get; }

    public SwitchSessionState State { get; private set; }

    public IReadOnlyList<WindowInfo> Windows { get; }

    public int SelectedIndex { get; private set; }

    public nint OriginalForegroundWindowHandle { get; }

    public string ActiveSortStrategyId { get; }

    public WindowInfo SelectedWindow => Windows[SelectedIndex];

    public void MoveNext()
    {
        SelectedIndex = Services.SelectionNavigator.MoveNext(SelectedIndex, Windows.Count);
    }

    public void MovePrevious()
    {
        SelectedIndex = Services.SelectionNavigator.MovePrevious(SelectedIndex, Windows.Count);
    }

    public void Close()
    {
        State = SwitchSessionState.Idle;
    }
}
