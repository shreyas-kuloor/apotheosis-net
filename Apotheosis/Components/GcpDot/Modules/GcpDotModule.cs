using Apotheosis.Components.GCPDot.Interfaces;
using Apotheosis.Components.ImageUpload.Interfaces;
using Apotheosis.Utils;
using Discord;
using Discord.Interactions;
using SixLabors.ImageSharp.Formats.Png;

namespace Apotheosis.Components.GCPDot.Modules;

public class GcpDotModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGcpDotService _gcpDotService;
    private readonly IImageUploadService _imageUploadService;

    public GcpDotModule(
        IGcpDotService gcpDotService,
        IImageUploadService imageUploadService)
    {
        _gcpDotService = gcpDotService;
        _imageUploadService = imageUploadService;
    }

    [SlashCommand("dot", "Get the real-time GCP Dot")]
    public async Task GetGcpDotAsync()
    {
        var (centerColor, edgeColor) = await _gcpDotService.GetGcpDotAsync();
        
        var gcpDotImage = ImageUtils.CreateCircleImage(centerColor, edgeColor, 50);

        var gcpDotImageBase64String = gcpDotImage.ToBase64String(PngFormat.Instance).Split(",")[1];
        
        var gcpDotImageUrl = await _imageUploadService.UploadImageAsync(gcpDotImageBase64String);

        await RespondAsync(embed: new EmbedBuilder().WithImageUrl($"{gcpDotImageUrl}").Build());
    }
}