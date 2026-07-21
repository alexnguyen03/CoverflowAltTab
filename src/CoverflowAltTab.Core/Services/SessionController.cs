using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Services;

public sealed class SessionController
{
    private readonly WindowPipeline _windowPipeline;

    public SessionController(WindowPipeline windowPipeline)
    {
        _windowPipeline = windowPipeline;
    }

    public SwitchSession? CurrentSession { get; private set; }

    public SwitchSessionState State => CurrentSession?.State ?? SwitchSessionState.Idle;

    public SessionOpenResult OpenSession(
        IReadOnlyList<WindowInfo> rawWindows,
        nint originalForegroundWindowHandle,
        IReadOnlyList<IWindowFilterRule> filters,
        IWindowSortRule sortRule)
    {
        if (CurrentSession is not null)
        {
            return new SessionOpenResult(
                false,
                "Session already open.",
                null,
                new WindowPipelineResult(Array.Empty<WindowInfo>(), rawWindows.Count, 0, sortRule.Id));
        }

        var pipelineResult = _windowPipeline.ProcessWindows(rawWindows, filters, sortRule, originalForegroundWindowHandle);
        if (pipelineResult.Windows.Count == 0)
        {
            return new SessionOpenResult(false, "No eligible windows.", null, pipelineResult);
        }

        CurrentSession = new SwitchSession(
            pipelineResult.Windows,
            originalForegroundWindowHandle,
            pipelineResult.SortStrategyId);

        return new SessionOpenResult(true, string.Empty, CurrentSession, pipelineResult);
    }

    public bool MoveNext()
    {
        if (CurrentSession is null)
        {
            return false;
        }

        CurrentSession.MoveNext();
        return true;
    }

    public bool MovePrevious()
    {
        if (CurrentSession is null)
        {
            return false;
        }

        CurrentSession.MovePrevious();
        return true;
    }

    public CommitSessionResult CommitCurrentSession()
    {
        if (CurrentSession is null)
        {
            return new CommitSessionResult(false, "No open session.", null, null);
        }

        var session = CurrentSession;
        var selectedWindow = session.SelectedWindow;
        session.Close();
        CurrentSession = null;

        return new CommitSessionResult(true, string.Empty, selectedWindow, session.SessionId);
    }

    public bool CancelCurrentSession()
    {
        if (CurrentSession is null)
        {
            return false;
        }

        CurrentSession.Close();
        CurrentSession = null;
        return true;
    }
}
