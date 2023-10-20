using Apotheosis.Components.Logging.Interfaces;
using Discord;
using Microsoft.Extensions.Logging;


namespace Apotheosis.Components.Logging.Services;

public class LogService : ILogService
{
    private readonly ILogger _logger;

    public LogService(ILogger logger)
    {
        this._logger = logger;
    }

    public Task Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };
        _logger.Log(logLevel, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    public void LogError(Exception? exception, string? message, string? source)
    {
        _logger.LogError(exception, "[{Source}] {Message}", source, message);
    }
}
