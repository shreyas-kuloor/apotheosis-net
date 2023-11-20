﻿using Apotheosis.Components.GCPDot.Interfaces;
using Apotheosis.Utils;
using Discord.Interactions;
using SixLabors.ImageSharp.Formats.Png;

namespace Apotheosis.Components.GCPDot.Modules;

public class GcpDotModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGcpDotService _gcpDotService;

    public GcpDotModule(
        IGcpDotService gcpDotService)
    {
        _gcpDotService = gcpDotService;
    }

    [SlashCommand("dot", "Get the real-time GCP Dot")]
    public async Task GetGcpDotAsync()
    {
        var (centerColor, edgeColor) = await _gcpDotService.GetGcpDotAsync();
        
        var gcpDotImage = ImageUtils.CreateCircleImage(centerColor, edgeColor, 50);

        using var ms = new MemoryStream();
        await gcpDotImage.SaveAsync(ms, PngFormat.Instance);
            
        await Context.Interaction.RespondWithFileAsync(ms, "gcp_dot.png");
    }
}