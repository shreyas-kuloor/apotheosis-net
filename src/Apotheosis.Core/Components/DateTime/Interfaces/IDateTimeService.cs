namespace Apotheosis.Core.Components.DateTime.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}