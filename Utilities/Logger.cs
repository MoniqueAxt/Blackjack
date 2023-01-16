using System.Diagnostics;

namespace Utilities;

/// <summary>
/// Classes for logging to Console or File
/// </summary>
public static class Logger
{
    public static Action<string> WriteMessage;

    public static void LogMessage(string msg)
    {
        WriteMessage?.Invoke(msg);
    }
}

public static class LoggingMethods
{
    public static void LogToConsole(string msg)
    {
        //Console.WriteLine(msg);
        Trace.WriteLine(msg);
    }
}

public class FileLogger
{
    private readonly string _logPath;

    public FileLogger(string logPath)
    {
        _logPath = logPath;
        Logger.WriteMessage += LogMessage;
    }

    public void DetachLog() => Logger.WriteMessage -= LogMessage;

    // make sure this can't throw.
    private void LogMessage(string msg)
    {
        try
        {
            using var log = File.AppendText(_logPath);
            log.WriteLine(msg);
            log.Flush();
        }
        catch (Exception) { /*ignored*/ }
    }
}