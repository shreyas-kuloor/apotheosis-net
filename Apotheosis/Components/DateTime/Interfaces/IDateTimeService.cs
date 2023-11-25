namespace Apotheosis.Components.DateTime.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}