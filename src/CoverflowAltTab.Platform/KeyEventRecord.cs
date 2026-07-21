namespace CoverflowAltTab.Platform;

public sealed record KeyEventRecord(
    KeyEventKind Kind,
    int VirtualKeyCode,
    string KeyName,
    bool CtrlPressed,
    bool AltPressed,
    bool ShiftPressed,
    DateTimeOffset Timestamp)
{
    public string ModifiersDisplay =>
        string.Join(
            " + ",
            new[]
            {
                CtrlPressed ? "Ctrl" : null,
                AltPressed ? "Alt" : null,
                ShiftPressed ? "Shift" : null,
                KeyName,
            }.Where(part => !string.IsNullOrWhiteSpace(part)));
}
