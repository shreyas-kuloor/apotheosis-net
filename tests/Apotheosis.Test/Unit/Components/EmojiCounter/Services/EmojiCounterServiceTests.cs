using Apotheosis.Core.Components.EmojiCounter.Models;
using Apotheosis.Infrastructure.Data;
using Apotheosis.Infrastructure.Entities;
using Apotheosis.Test.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Apotheosis.Core.Components.EmojiCounter.Services;

namespace Apotheosis.Test.Unit.Components.EmojiCounter.Services;

public sealed class EmojiCounterServiceTests : IDisposable
{
    const ulong emojiId = 123;
    const ulong guildId = 321;
    const string name = "test";

    private readonly ApotheosisDbContext _context;
    private readonly EmojiCounterService _emojiCounterService;

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
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 1,
        };

        await _emojiCounterService.IncrementEmojiCountAsync(guildId, emojiId, name);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task IncrementEmojiCountAsync_UpdatesEmojiUsageAndIncreasesCountBy1_GivenExistingEmojiUsage()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 5,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 6,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.IncrementEmojiCountAsync(guildId, emojiId, name);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_DoesNothing_GivenNoExistingEmojiUsage()
    {
        await _emojiCounterService.DecrementEmojiCountAsync(guildId, emojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        actualEmojiUsage.Should().BeNull();
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_DoesNothing_GivenExistingEmojiUsageWithCount0()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 0,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 0,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.DecrementEmojiCountAsync(guildId, emojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task DecrementEmojiCountAsync_UpdatesEmojiUsageAndDecreasesCountBy1_GivenExistingEmojiUsage()
    {
        var existingEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 5,
        };

        var expectedEmojiUsage = new EmojiUsage
        {
            Name = name,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 4,
        };

        await _context.EmojiUsages.AddAsync(existingEmojiUsage);
        await _context.SaveChangesAsync();

        await _emojiCounterService.DecrementEmojiCountAsync(guildId, emojiId);

        var actualEmojiUsage = await _context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        actualEmojiUsage.Should().BeEquivalentTo(expectedEmojiUsage, options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task GetEmojiCountsForGuildAsync_GetsAllEmojiCountsForProvidedGuildId_GivenExistingEmojiUsagesForGuildId()
    {
        List<EmojiUsage> existingEmojiUsages = 
        [
            new EmojiUsage
            {
                Name = name,
                EmojiId = emojiId,
                GuildId = guildId,
                Count = 5,
            },
            new EmojiUsage
            {
                Name = "test2",
                EmojiId = 124,
                GuildId = guildId,
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
                Name = name,
                EmojiId = emojiId,
                GuildId = guildId,
                Count = 5,
            },
            new EmojiCounterDto
            {
                Name = "test2",
                EmojiId = 124,
                GuildId = guildId,
                Count = 2
            },
        ];

        await _context.EmojiUsages.AddRangeAsync(existingEmojiUsages);
        await _context.SaveChangesAsync();

        var actualEmojiUsages = await _emojiCounterService.GetEmojiCountsForGuildAsync(guildId);

        actualEmojiUsages.Should().BeEquivalentTo(expectedEmojiUsages);
    }

    private void ClearDatabase()
    {
        _context.EmojiUsages.RemoveRange(_context.EmojiUsages);
        _context.SaveChanges();
    }
}
