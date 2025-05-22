using Apotheosis.Core.Features.GcpDot.Interfaces;
using Color = SixLabors.ImageSharp.Color;

namespace Apotheosis.Server.Features.GcpDot.Services;

[Scoped]
public sealed class GcpDotService(IGcpDotNetworkDriver gcpDotNetworkDriver) : IGcpDotService
{
    /// <summary>
    /// The path for the request to retrieve the gcp stat information.
    /// </summary>
    public const string GcpPath = "/gcpindex.php?current=1";

    public async Task<(Color centerDotColor, Color edgeDotColor)> GetGcpDotAsync()
    {
        var stringGcpResponse =
            await gcpDotNetworkDriver.SendRequestAsync(GcpPath, HttpMethod.Get, null);

        var gcpStats = GcpStatParser.ParseGcpStats(stringGcpResponse);

        var maxGcpValue = gcpStats.Max(gcpStat => gcpStat.Value);

        var (centerDotColor, edgeDotColor) = maxGcpValue switch
        {
            > 0 and <= 0.01 => (Color.Parse("#FFA8C0"), Color.Parse("#FF0064")),
            > 0.01 and <= 0.05 => (Color.Parse("#FF1E1E"), Color.Parse("#840607")),
            > 0.05 and <= 0.08 => (Color.Parse("#FFB82E"), Color.Parse("#C95E00")),
            > 0.08 and <= 0.15 => (Color.Parse("#FFD517"), Color.Parse("#C69000")),
            > 0.15 and <= 0.23 => (Color.Parse("#FFFA40"), Color.Parse("#C6C300")),
            > 0.23 and <= 0.30 => (Color.Parse("#F9FA00"), Color.Parse("#B0CC00")),
            > 0.30 and <= 0.40 => (Color.Parse("#AEFA00"), Color.Parse("#88C200")),
            > 0.40 and <= 0.90 => (Color.Parse("#64FA64"), Color.Parse("#00A700")),
            > 0.90 and <= 0.9125 => (Color.Parse("#64FAAB"), Color.Parse("#00B5C9")),
            > 0.9125 and <= 0.93 => (Color.Parse("#ACF2FF"), Color.Parse("#21BCF1")),
            > 0.93 and <= 0.96 => (Color.Parse("#0EEEFF"), Color.Parse("#0786E1")),
            > 0.96 and <= 0.98 => (Color.Parse("#24CBFD"), Color.Parse("#0000FF")),
            > 0.98 => (Color.Parse("#5655CA"), Color.Parse("#2400A0")),
            _ => (Color.Parse("#CDCDCD"), Color.Parse("#505050"))
        };

        return (centerDotColor, edgeDotColor);
    }

}