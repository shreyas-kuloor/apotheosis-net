using Apotheosis.Core.Features.EmojiCounter.Aggregates;
using Apotheosis.Core.Features.EmojiCounter.Models;

namespace Apotheosis.Test.Unit.Components.EmojiCounter.Services;

public sealed class EmojiCounterServiceTests : IDisposable
{
    const ulong EmojiId = 123;
    const ulong GuildId = 321;
    const string Name = "test";

    readonly ApotheosisDbContext _context;
    readonly EmojiCounterService _emojiCounterService;

    public EmojiCounterServiceTests()
    {
        _context = InMemoryDbContextBuilder.CreateInMemoryDbContext();
        _emojiCounterService = new EmojiCounterService(_context);
    }

    public void Dispose()
    {
        ClearDatabase();
    }

    [Fact]
    public async Task IncrementEmojiCountAsync_CreatesNewEmojiUsageWithCount1_GivenNoExistingEmojiUsage()
    {
        var expectedEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 1,
        };

        await _emojiCounterService.IncrementEmojiCountAsync(GuildId, EmojiId, Name);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == GuildId && e.EmojiId == EmojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task IncrementEmojiCountAsync_UpdatesEmojiUsageAndIncreasesCountBy1_GivenExistingEmojiUsage()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 5,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 6,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.IncrementEmojiCountAsync(GuildId, EmojiId, Name);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == GuildId && e.EmojiId == EmojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_DoesNothing_GivenNoExistingEmojiUsage()
    {
        await _emojiCounterService.DecrementEmojiCountAsync(GuildId, EmojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == GuildId && e.EmojiId == EmojiId);

        actualEmojiUsage.Should().BeNull();
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_DoesNothing_GivenExistingEmojiUsageWithCount0()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 0,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 0,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.DecrementEmojiCountAsync(GuildId, EmojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == GuildId && e.EmojiId == EmojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_UpdatesEmojiUsageAndDecreasesCountBy1_GivenExistingEmojiUsage()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 5,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = Name,
            EmojiId = EmojiId,
            GuildId = GuildId,
            Count = 4,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.DecrementEmojiCountAsync(GuildId, EmojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == GuildId && e.EmojiId == EmojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task GetEmojiCountsForGuildAsync_GetsAllEmojiCountsForProvidedGuildId_GivenExistingEmojiUsagesForGuildId()
    {
        List<EmojiUsage> existingEmojiUsages =
        [
            new EmojiUsage
            {
                Name = Name,
                EmojiId = EmojiId,
                GuildId = GuildId,
                Count = 5,
            },
            new EmojiUsage
            {
                Name = "test2",
                EmojiId = 124,
                GuildId = GuildId,
                Count = 2
            },
            new EmojiUsage
            {
                Name = "test3",
                EmojiId = 125,
                GuildId = 322,
                Count = 2
            },
        ];

        List<EmojiCounterDto> expectedEmojiUsages =
        [
            new EmojiCounterDto
            {
                Name = Name,
                EmojiId = EmojiId,
                GuildId = GuildId,
                Count = 5,
            },
            new EmojiCounterDto
            {
                Name = "test2",
                EmojiId = 124,
                GuildId = GuildId,
                Count = 2
            },
        ];

        await _context.EmojiUsages.AddRangeAsync(existingEmojiUsages);
        await _context.SaveChangesAsync();

        var actualEmojiUsages = await _emojiCounterService.GetEmojiCountsForGuildAsync(GuildId);

        actualEmojiUsages.Should().BeEquivalentTo(expectedEmojiUsages);
    }

    void ClearDatabase()
    {
        _context.EmojiUsages.RemoveRange(_context.EmojiUsages);
        _context.SaveChanges();
    }
}
