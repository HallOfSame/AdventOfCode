using System;

namespace Helpers.Logging;

public class ProgressLogger
{
    public event EventHandler<(string, MessageType)>? OnProgress;

    public void LogProgress(string message, MessageType type = MessageType.Normal)
    {
        OnProgress?.Invoke(this, (message, type));
    }
}