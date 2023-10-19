using Discord;

namespace Apotheosis.Components.Logging.Interfaces;

public interface ILogService
{
    Task Log(LogMessage message);
}
