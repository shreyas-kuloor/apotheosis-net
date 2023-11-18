using Apotheosis.Components.GCPDot.Interfaces;
using Apotheosis.Components.ImageUpload.Interfaces;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

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
        var gcpDotImage = await _gcpDotService.GetGcpDotAsync();
        using (gcpDotImage)
        {
            using (var ms = new MemoryStream())
            {
                await gcpDotImage.SaveAsPngAsync(ms);

                var gcpDotImageBytes = ms.ToArray();

                var gcpDotImageBase64String = Convert.ToBase64String(gcpDotImageBytes);

                var gcpDotImageUrl = await _imageUploadService.UploadImageAsync(gcpDotImageBase64String);
                
                await RespondAsync(embed: new EmbedBuilder().WithImageUrl($"{gcpDotImageUrl}").Build());
            }
        }
    }
}