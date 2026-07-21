using System.Diagnostics;
using System.IO;
using CoverflowAltTab.Extensibility;

namespace CoverflowAltTab.Host.Infrastructure;

public sealed class FileDebugLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly object _syncRoot = new();

    public FileDebugLogger(string appBaseDirectory)
    {
        _logDirectory = Path.Combine(appBaseDirectory, "logs");
        Directory.CreateDirectory(_logDirectory);
    }

    public void Debug(string message)
    {
        Write("DEBUG", message, null);
    }

    public void Info(string message)
    {
        Write("INFO", message, null);
    }

    public void Error(string message, Exception? exception = null)
    {
        Write("ERROR", message, exception);
    }

    private void Write(string level, string message, Exception? exception)
    {
        var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
        var line = $"{timestamp} [{level}] {message}";
        if (exception is not null)
        {
            line = $"{line}{Environment.NewLine}{exception}";
        }

        lock (_syncRoot)
        {
            var filePath = Path.Combine(_logDirectory, $"coverflow-alttab-{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(filePath, line + Environment.NewLine);
        }

        System.Diagnostics.Debug.WriteLine(line);
    }
}
