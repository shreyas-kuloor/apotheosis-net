using Microsoft.Extensions.Logging;

namespace Apotheosis.Core.Features.Logging.Interfaces;

public interface ILogService<T> where T : class
{
    void LogWarning(Exception? exception, string? message);

    void LogError(Exception? exception, string? message);

    void Log(LogLevel logLevel, Exception? exception, string? message);
}