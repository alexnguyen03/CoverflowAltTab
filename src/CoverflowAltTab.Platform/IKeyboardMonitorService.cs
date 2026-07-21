namespace CoverflowAltTab.Platform;

public interface IKeyboardMonitorService : IDisposable
{
    event EventHandler<KeyEventRecord>? KeyEventReceived;

    void Start();

    void Stop();
}
