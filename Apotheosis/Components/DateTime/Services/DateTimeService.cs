using Apotheosis.Components.DateTime.Interfaces;

namespace Apotheosis.Components.DateTime.Services;

public sealed class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}