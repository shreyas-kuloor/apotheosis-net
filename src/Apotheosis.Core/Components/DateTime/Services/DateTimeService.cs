using Apotheosis.Core.Components.DateTime.Interfaces;

namespace Apotheosis.Core.Components.DateTime.Services;

public sealed class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}