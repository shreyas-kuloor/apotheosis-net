using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Apotheosis.Server.Utils;

public static class ImageUtils
{
    public static Image CreateCircleImage(Color centerColor, Color edgeColor, int radius)
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