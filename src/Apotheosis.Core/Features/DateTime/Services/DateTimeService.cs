using Apotheosis.Core.Features.DateTime.Interfaces;

namespace Apotheosis.Core.Features.DateTime.Services;

[Scoped]
public sealed class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}