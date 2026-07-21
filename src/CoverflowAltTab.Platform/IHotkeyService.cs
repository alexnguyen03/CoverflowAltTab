namespace CoverflowAltTab.Platform;

public interface IHotkeyService : IDisposable
{
    event EventHandler? Triggered;

    void Start();

    void Stop();
}
