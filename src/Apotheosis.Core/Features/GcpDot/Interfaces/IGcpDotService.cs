using Color = SixLabors.ImageSharp.Color;

namespace Apotheosis.Core.Features.GcpDot.Interfaces;

public interface IGcpDotService
{
    Task<(Color centerDotColor, Color edgeDotColor)> GetGcpDotAsync();
}