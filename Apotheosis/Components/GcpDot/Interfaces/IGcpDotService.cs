namespace Apotheosis.Components.GCPDot.Interfaces;

public interface IGcpDotService
{
    Task<(Color centerDotColor, Color edgeDotColor)> GetGcpDotAsync();
}