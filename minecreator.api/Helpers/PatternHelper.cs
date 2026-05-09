using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class PatternHelper
    {
        public static Image<Rgba32> Flannel(Point dimensions, int cellSize, Point offset, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    int cellX = (x + offset.X) / cellSize;
                    int cellY = (y + offset.Y) / cellSize;

                    bool isVerticalStripe = (cellX % 2 == 0);
                    var isHorizontalStripe = (cellY % 2 == 0);

                    int colorIndex;
                    if (isVerticalStripe && isHorizontalStripe)
                    {
                        colorIndex = Math.Min(2, colors.Count - 1);
                    }
                    else if (isVerticalStripe || isHorizontalStripe)
                    {
                        colorIndex = Math.Min(1, colors.Count - 1);
                    }
                    else
                    {
                        colorIndex = 0;
                    }

                    image[x, y] = colors[colorIndex];
                }
            }
            return image;
        }
        public static Image<Rgba32> Stripes(Point dimensions, Point offset, List<int> stripesWidths, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);
            for (int y = 0; y < dimensions.Y; y++)
            {
                int currentY = stripesWidths.Sum() == 0 ? 0 : (y + offset.Y) % stripesWidths.Sum();
                if (currentY < 0) currentY += stripesWidths.Sum();

                int stripeIndex = 0;
                int accumulatedWidth = 0;

                for (int i = 0; i < stripesWidths.Count; i++)
                {
                    accumulatedWidth += stripesWidths[i];
                    if (currentY < accumulatedWidth)
                    {
                        stripeIndex = Math.Min(i, colors.Count - 1);
                        break;
                    }
                }

                for (int x = 0; x < dimensions.X; x++)
                {
                    var color = colors[0];

                    if (stripeIndex < stripesWidths.Count)
                        color = colors[stripeIndex];
                    image[x, y] = color;
                }
            }

            return image;
        }
    }
}
