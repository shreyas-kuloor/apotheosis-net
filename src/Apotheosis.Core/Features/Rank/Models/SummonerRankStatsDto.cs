﻿namespace Apotheosis.Core.Features.Rank.Models;
public record SummonerRankStatsDto
{
    public string? LeagueId { get; set; }

    public string? QueueType { get; set; }

    public string? Tier { get; set; }

    public string? Rank { get; set; }

    public string? SummonerId { get; set; }

    public int LeaguePoints { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public bool Veteran { get; set; }

    public bool Inactive { get; set; }

    public bool FreshBlood { get; set; }

    public bool HotStreak { get; set; }
}
