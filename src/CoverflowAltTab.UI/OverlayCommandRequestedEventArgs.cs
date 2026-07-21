namespace CoverflowAltTab.UI;

public sealed class OverlayCommandRequestedEventArgs : EventArgs
{
    public OverlayCommandRequestedEventArgs(OverlayCommand command)
    {
        Command = command;
    }

    public OverlayCommand Command { get; }
}
