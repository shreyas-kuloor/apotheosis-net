using Apotheosis.Components.GCPDot.Interfaces;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Apotheosis.Components.GCPDot.Services;

public class GcpDotService : IGcpDotService
{
    private readonly IGcpDotNetworkDriver _gcpDotNetworkDriver;

    public GcpDotService(IGcpDotNetworkDriver gcpDotNetworkDriver)
    {
        _gcpDotNetworkDriver = gcpDotNetworkDriver;
    }
    
    public async Task<Image> GetGcpDotAsync()
    {
        var stringGcpResponse =
            await _gcpDotNetworkDriver.SendRequestAsync("/gcpindex.php?current=1", HttpMethod.Get, null);

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

        return CreateCircleImage(centerDotColor, edgeDotColor, 50);
    }

    private static Image CreateCircleImage(Color centerColor, Color edgeColor, int radius)
    {
        var image = new Image<Rgba32>(radius*2, radius*2);
        
        image.Mutate(context => context.Fill(Color.Transparent));

        var gradientBrush = new RadialGradientBrush(
            new PointF(radius, radius / 2f), 
            radius,
            GradientRepetitionMode.None, 
            new ColorStop(0, centerColor), 
            new ColorStop(1, edgeColor));
        
        image.Mutate(context => context.Fill(gradientBrush, new EllipsePolygon(radius, radius, radius)));

        return image;
    }
}