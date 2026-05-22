using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public enum TexturePatternBlendType
    {
        Normal = 0,
        BrightnessMap = 1,
        BrightnessMapWithOpacity = 2,
        SingleBrightnessMap = 3,
        SingleBrightnessMapWithOpacity = 4,
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
                    else if (pattern.BlendType == TexturePatternBlendType.SingleBrightnessMapWithOpacity)
                    { 
                        if (patternPixel.A == 0)
                        {
                            result[x, y] = patternPixel;
                            continue;
                        }

                        var basePalette = ColorHelper.COLORS_PALLETE[0];

                        int baseColorIndex = basePalette.Colors.IndexOf(basePalette.BaseColor);
                        if (baseColorIndex == -1) baseColorIndex = 0;

                        int patternColorIndex = basePalette.Colors.FindIndex(c => c.R == patternPixel.R && c.G == patternPixel.G && c.B == patternPixel.B);

                        int distance = 1;

                        if (patternColorIndex != -1)
                        {
                            
                            distance = baseColorIndex - patternColorIndex;
                        }

                        int pixelColorIndex = basePalette.Colors.FindIndex(c => c.R == basePixel.R && c.G == basePixel.G && c.B == basePixel.B);

                        if (pixelColorIndex == -1)
                        {
                            result[x, y] = basePixel;
                            continue;
                        }

                        int mappedColorIndex = pixelColorIndex - distance;
                        if (mappedColorIndex < 0)
                        {
                            mappedColorIndex = 0;
                        }
                        else if (mappedColorIndex >= basePalette.Colors.Count)
                        {
                            mappedColorIndex = basePalette.Colors.Count - 1;
                        }

                        result[x, y] = basePalette.Colors[mappedColorIndex];
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
        public static TexturePattern JeansHoles(Point dimensions, int seed, Point offset, ColorPallete colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);
            int gridSize = 3;

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    int globalX = x + offset.X;
                    int globalY = y + offset.Y;

                    Rgba32 pixelColor = colors.BaseColor;
                    bool holePixelFound = false;

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

                            if ((cellHash % 100) < 50)
                            {
                                int centerOffset = gridSize / 2;
                                int jitterX = (int)(cellHash % 2);
                                int jitterY = (int)((cellHash / 2) % 2);

                                int holeCenterX = currentCellX * gridSize + centerOffset + jitterX;
                                int holeCenterY = currentCellY * gridSize + centerOffset + jitterY;

                                int dx = globalX - holeCenterX;
                                int dy = globalY - holeCenterY;

                                double dist = Math.Sqrt(Math.Pow(dx - 0.15, 2) + Math.Pow(dy + 0.15, 2));

                                int pixelNoise = (int)((cellHash ^ (uint)(globalX * 13 + globalY * 29)) % 3);

                                double targetRadius = 0.75 + (cellHash % 2) * 0.25;
                                if (pixelNoise == 1) targetRadius += 0.2;

                                bool isHoleCenter = dist < targetRadius;
                                bool isBorderPixel = false;

                                if (!isHoleCenter)
                                {
                                    double borderThickness = 0.45 + (pixelNoise * 0.1);
                                    isBorderPixel = dist >= targetRadius && dist <= (targetRadius + borderThickness);
                                }

                                if (isHoleCenter)
                                {
                                    pixelColor = new Rgba32(0, 0, 0, 0);
                                    holePixelFound = true;
                                    break;
                                }

                                if (isBorderPixel)
                                {
                                    pixelColor = ColorHelper.GetBrighter(colors, colors.BaseColor);
                                    holePixelFound = true;
                                    break;
                                }
                            }
                        }
                        if (holePixelFound) break;
                    }

                    image[x, y] = pixelColor;
                }
            }

            return new TexturePattern()
            {
                Texture = image,
                BlendType = TexturePatternBlendType.SingleBrightnessMapWithOpacity
            };
        }
        public static TexturePattern Knit(Point dimensions, Point offset, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);

            int colorCount = Math.Min(colors.Count, 5);

            for (int y = 0; y < dimensions.Y; y++)
            {
                int patternY = (y + offset.Y) % 8;
                if (patternY < 0) patternY += 8;

                for (int x = 0; x < dimensions.X; x++)
                {
                    int patternX = (x + offset.X) % 4;
                    if (patternX < 0) patternX += 4;

                    Rgba32 pixelColor;
                    int c0 = 0;
                    int c1 = colorCount > 1 ? 1 : 0;
                    int c2 = colorCount > 2 ? 2 : (colorCount > 1 ? 1 : 0);
                    int c3 = colorCount > 3 ? 3 : (colorCount > 1 ? 1 : 0);
                    int c4 = colorCount > 4 ? 4 : (colorCount > 1 ? 1 : 0);

                    if (patternY == 0 || patternY == 4)
                    {
                        pixelColor = (patternX == 0 || patternX == 2) ? colors[c0] : colors[c4];
                    }
                    else if (patternY == 1 || patternY == 3)
                    {
                        pixelColor = (patternX == 0 || patternX == 2) ? colors[c1] : colors[c3];
                    }
                    else if (patternY == 2)
                    {
                        pixelColor = (patternX == 0 || patternX == 2) ? colors[c2] : colors[c2];
                    }
                    else if (patternY == 5 || patternY == 7)
                    {
                        pixelColor = (patternX == 1 || patternX == 3) ? colors[c1] : colors[c3];
                    }
                    else
                    {
                        pixelColor = (patternX == 1 || patternX == 3) ? colors[c2] : colors[c2];
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
        public static TexturePattern Argyle(Point dimensions, Point offset, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);

            int colorCount = Math.Min(colors.Count, 5);

            for (int y = 0; y < dimensions.Y; y++)
            {
                int patternY = (y + offset.Y) % 8;
                if (patternY < 0) patternY += 8;

                for (int x = 0; x < dimensions.X; x++)
                {
                    int patternX = (x + offset.X) % 8;
                    if (patternX < 0) patternX += 8;

                    int cBase = 0;
                    int cRomb1 = colorCount > 1 ? 1 : 0;
                    int cRomb2 = colorCount > 2 ? 2 : (colorCount > 1 ? 1 : 0);
                    int cLine1 = colorCount > 3 ? 3 : (colorCount > 1 ? 1 : 0);
                    int cLine2 = colorCount > 4 ? 4 : (colorCount > 2 ? 2 : (colorCount > 1 ? 1 : 0));

                    Rgba32 pixelColor = colors[cBase];

                    if ((patternY == 0 && patternX == 3) || (patternY == 1 && (patternX == 2 || patternX == 4)) ||
                        (patternY == 2 && (patternX == 1 || patternX == 5)) || (patternY == 3 && (patternX == 0 || patternX == 6)) ||
                        (patternY == 4 && (patternX == 1 || patternX == 5)) || (patternY == 5 && (patternX == 2 || patternX == 4)) ||
                        (patternY == 6 && (patternX == 3 || patternX == 7)) || (patternY == 7 && patternX == 4))
                    {
                        pixelColor = colors[cLine1];
                    }
                    else if ((patternY == 0 && patternX == 7) || (patternY == 1 && (patternX == 6 || patternX == 0)) ||
                             (patternY == 2 && (patternX == 5 || patternX == 1)) || (patternY == 3 && (patternX == 4 || patternX == 2)) ||
                             (patternY == 4 && (patternX == 3 || patternX == 3)) || (patternY == 5 && (patternX == 2 || patternX == 4)) ||
                             (patternY == 6 && (patternX == 1 || patternX == 5)) || (patternY == 7 && (patternX == 0 || patternX == 6)))
                    {
                        pixelColor = colors[cLine2];
                    }
                    else if ((patternY + patternX == 3) || (patternY + patternX == 11) || (Math.Abs(patternY - patternX) == 4))
                    {
                        pixelColor = colors[cRomb1];
                    }
                    else if ((patternY + patternX == 7) || (patternY == patternX))
                    {
                        pixelColor = colors[cRomb2];
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
        public static TexturePattern Herringbone(Point dimensions, Point offset, List<Rgba32> colors)
        {
            var image = new Image<Rgba32>(dimensions.X, dimensions.Y);

            if (colors == null || colors.Count == 0)
            {
                colors = new List<Rgba32> { new Rgba32(120, 120, 120) };
            }

            int colorCount = Math.Min(colors.Count, 5);

            for (int y = 0; y < dimensions.Y; y++)
            {
                int patternY = (y + offset.Y) % 4;
                if (patternY < 0) patternY += 4;

                for (int x = 0; x < dimensions.X; x++)
                {
                    int patternX = (x + offset.X) % 4;
                    if (patternX < 0) patternX += 4;

                    int c0 = 0;
                    int c1 = colorCount > 1 ? 1 : 0;
                    int c2 = colorCount > 2 ? 2 : (colorCount > 1 ? 1 : 0);
                    int c3 = colorCount > 3 ? 3 : (colorCount > 2 ? 2 : (colorCount > 1 ? 1 : 0));
                    int c4 = colorCount > 4 ? 4 : (colorCount > 3 ? 3 : (colorCount > 1 ? 1 : 0));

                    Rgba32 pixelColor;

                    if (patternY == 0)
                    {
                        if (patternX == 0 || patternX == 3) pixelColor = colors[c0];
                        else pixelColor = colors[c1];
                    }
                    else if (patternY == 1)
                    {
                        if (patternX == 1 || patternX == 2) pixelColor = colors[c0];
                        else pixelColor = colors[c2];
                    }
                    else if (patternY == 2)
                    {
                        if (patternX == 1 || patternX == 2) pixelColor = colors[c0];
                        else pixelColor = colors[c3];
                    }
                    else
                    {
                        if (patternX == 0 || patternX == 3) pixelColor = colors[c0];
                        else pixelColor = colors[c4];
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
