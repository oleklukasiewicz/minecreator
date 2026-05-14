using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;

namespace minecreator.api.Helpers
{
    public enum TexturePatternBlendType
    {
        Normal = 0,
        BrightnessMap = 1
    }
    public class TexturePattern
    {
        public Image<Rgba32> Texture { get; set; }
        public TexturePatternBlendType BlendType { get; set; }
    }
    public static class PatternHelper
    {
        public static Image<Rgba32> ApplyPattern(Image<Rgba32> baseImage, TexturePattern pattern)
        {
            var result = baseImage.Clone();
            for (int y = 0; y < baseImage.Height; y++)
            {
                for (int x = 0; x < baseImage.Width; x++)
                {
                    var basePixel = baseImage[x, y];
                    if (basePixel.A == 0) continue;

                    var patternPixel = pattern.Texture[x % pattern.Texture.Width, y % pattern.Texture.Height];
                    if (pattern.BlendType == TexturePatternBlendType.Normal)
                    {
                        result[x, y] = patternPixel;
                    }
                    else if (pattern.BlendType == TexturePatternBlendType.BrightnessMap)
                    {
                        var mappedColor = ColorHelper.MapColor(basePixel, patternPixel);
                        result[x, y] = mappedColor;
                    }
                }
            }
            return result;
        }
        public static TexturePattern Flannel(Point dimensions, int cellSize, Point offset, List<Rgba32> colors)
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
            return new TexturePattern()
            {
                Texture = image,
                BlendType = TexturePatternBlendType.Normal
            };
        }
        public static TexturePattern Stripes(Point dimensions, Point offset, List<int> stripesWidths, List<Rgba32> colors)
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

            return new TexturePattern()
            {
                Texture = image,
                BlendType = TexturePatternBlendType.BrightnessMap
            };
        }

        public static TexturePattern Hawaii(Point dimensions, int seed, Point offset, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);
            int gridSize = 6;

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    int globalX = x + offset.X;
                    int globalY = y + offset.Y;

                    Rgba32 pixelColor = colors[0];
                    bool flowerPixelFound = false;

                    for (int cy = -1; cy <= 1; cy++)
                    {
                        for (int cx = -1; cx <= 1; cx++)
                        {
                            int currentCellX = (int)Math.Floor((double)globalX / gridSize) + cx;
                            int currentCellY = (int)Math.Floor((double)globalY / gridSize) + cy;

                            int h = seed;
                            h = h * 31 + currentCellX;
                            h = h * 31 + currentCellY;
                            h ^= (h << 13); h ^= (h >> 17); h ^= (h << 5);
                            uint cellHash = (uint)Math.Abs(h);

   
                            if ((cellHash % 100) < 98)
                            {
                                int centerOffset = gridSize / 2;
                                int jitterX = (int)(cellHash % 3) - 1; 
                                int jitterY = (int)((cellHash / 3) % 3) - 1;

                                int flowerX = currentCellX * gridSize + centerOffset + jitterX;
                                int flowerY = currentCellY * gridSize + centerOffset + jitterY;

                                int dx = globalX - flowerX;
                                int dy = globalY - flowerY;

                                if (dx == 0 && dy == 0)
                                {
                                    pixelColor = colors[Math.Min(1, colors.Count - 1)];
                                    flowerPixelFound = true;
                                    break;
                                }
                                else if ((Math.Abs(dx) == 1 && dy == 0) || (Math.Abs(dy) == 1 && dx == 0))
                                {
                                    pixelColor = colors[Math.Min(2, colors.Count - 1)];
                                    flowerPixelFound = true;
                                    break;
                                }
                            }
                        }
                        if (flowerPixelFound) break;
                    }

                    image[x, y] = pixelColor;
                }
            }

            return new TexturePattern()
            {
                Texture = image,
                BlendType = TexturePatternBlendType.BrightnessMap
            };
        }

    }

}
