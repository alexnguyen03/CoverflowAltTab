namespace CoverflowAltTab.Extensibility;

public interface ILogger
{
    void Debug(string message);

    void Info(string message);

    void Error(string message, Exception? exception = null);
}
