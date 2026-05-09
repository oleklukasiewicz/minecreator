using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Helpers
{
    public static class TextureManupulationHelper
    {
        public static Image<Rgba32> MoveByVector(Image<Rgba32> image, Rectangle area, Point vector)
        {
            var part = image.Clone();
            part.Mutate(x => x.Crop(area));
           
            image.ProcessPixelRows(accessor =>
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = area.Left; x < area.Right; x++)
                    {
                        row[x] = new Rgba32(0, 0, 0, 0);
                    }
                }
            });
            var targetPosition = new Point(area.Location.X + vector.X, area.Location.Y + vector.Y);
            image.Mutate(x => x.DrawImage(part, targetPosition, new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src }));
            return image;
        }
        public static Image<Rgba32> DrawOnVisible(Image<Rgba32> source, Image<Rgba32> layer)
        {
            var image = source.Clone();
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        if (row[x].A > 0)
                        {
                            row[x] = layer[x, y];
                        }
                    }
                }
            });
            return image;
        }
    }
}
