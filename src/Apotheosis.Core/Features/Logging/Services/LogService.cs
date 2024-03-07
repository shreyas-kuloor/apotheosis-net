using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Core.Features.Logging.Services;

public sealed class LogService<T>(ILogger<T> logger) : ILogService<T> where T : class
{
    public void LogWarning(Exception? exception, string? message)
    {
        logger.LogWarning(exception, "{Message}", message);
    }

    public void LogError(Exception? exception, string? message)
    {
        logger.LogError(exception, "{Message}", message);
    }

    public void Log(LogLevel logLevel, Exception? exception, string? message)
    {
        logger.Log(logLevel, exception, "{Message}", message);
    }
}