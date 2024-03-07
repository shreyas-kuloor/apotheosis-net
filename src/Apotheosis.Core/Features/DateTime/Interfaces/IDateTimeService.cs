namespace Apotheosis.Core.Features.DateTime.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}