using minecreator.api.Model;
using SixLabors.ImageSharp;
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
        public static Image<Rgba32> Merge(Image<Rgba32> source, Image<Rgba32> layer)
        {
            Image<Rgba32> image = source.Clone();
            image.Mutate(x => x.DrawImage(layer, new Point(0, 0), new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver }));
            return image;
        }
        public static List<Rectangle> DetectOutline(Image<Rgba32> image, bool top, bool bottom, bool left, bool right, int thickness = 1, bool onlyInner = false, bool onlyOuter = false)
        {
            var rectangles = new List<Rectangle>();

            image.ProcessPixelRows(accessor =>
            {
                int width = accessor.Width;
                int height = accessor.Height;

                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);

                    for (int x = 0; x < width; x++)
                    {
                        var pixel = row[x];
                        if (pixel.A > 0)
                        {
                            bool isOutline = false;

                            if (top)
                            {
                                for (int i = 1; i <= thickness; i++)
                                {
                                    bool isEdge = (y - i < 0);
                                    if (isEdge && onlyInner) break;
                                    if (!isEdge && onlyOuter) break;

                                    if (isEdge || accessor.GetRowSpan(y - i)[x].A == 0) { isOutline = true; break; }
                                }
                            }
                            if (!isOutline && bottom)
                            {
                                for (int i = 1; i <= thickness; i++)
                                {
                                    bool isEdge = (y + i >= height);
                                    if (isEdge && onlyInner) break;
                                    if (!isEdge && onlyOuter) break;

                                    if (isEdge || accessor.GetRowSpan(y + i)[x].A == 0) { isOutline = true; break; }
                                }
                            }
                            if (!isOutline && left)
                            {
                                for (int i = 1; i <= thickness; i++)
                                {
                                    bool isEdge = (x - i < 0);
                                    if (isEdge && onlyInner) break;
                                    if (!isEdge && onlyOuter) break;

                                    if (isEdge || row[x - i].A == 0) { isOutline = true; break; }
                                }
                            }
                            if (!isOutline && right)
                            {
                                for (int i = 1; i <= thickness; i++)
                                {
                                    bool isEdge = (x + i >= width);
                                    if (isEdge && onlyInner) break;
                                    if (!isEdge && onlyOuter) break;

                                    if (isEdge || row[x + i].A == 0) { isOutline = true; break; }
                                }
                            }

                            if (isOutline)
                            {
                                rectangles.Add(new Rectangle(x, y, 1, 1));
                            }
                        }
                    }
                }
            });

            return MergeRectangles(rectangles);
        }

        private static List<Rectangle> MergeRectangles(List<Rectangle> input)
        {
            if (input == null || input.Count == 0)
                return new List<Rectangle>();

            var rectangles = new List<Rectangle>(input);
            var merged = new List<Rectangle>();

            while (rectangles.Count > 0)
            {
                var current = rectangles[0];
                rectangles.RemoveAt(0);

                bool foundIntersection;
                do
                {
                    foundIntersection = false;
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        var target = rectangles[i];

                        bool sharesX = current.X == target.X && current.Width == target.Width;
                        bool touchesY = current.Y + current.Height == target.Y || target.Y + target.Height == current.Y;

                        bool sharesY = current.Y == target.Y && current.Height == target.Height;
                        bool touchesX = current.X + current.Width == target.X || target.X + target.Width == current.X;

                        if ((sharesX && touchesY) || (sharesY && touchesX))
                        {
                            current = Rectangle.Union(current, target);
                            rectangles.RemoveAt(i);
                            i--;
                            foundIntersection = true;
                        }
                    }
                } while (foundIntersection);

                merged.Add(current);
            }

            return merged;
        }
        public static Image<Rgba32> FillWithAltPallete(Image<Rgba32> image, Rectangle area, Rgba32 baseColor, Rgba32 newbaseColor, List<Point> excludedPoints = null)
        {
            var result = image.Clone();

            result.ProcessPixelRows(accessor =>
            {
                int startY = Math.Max(0, area.Top);
                int endY = Math.Min(accessor.Height, area.Bottom);
                int startX = Math.Max(0, area.Left);
                int endX = Math.Min(accessor.Width, area.Right);

                for (int y = startY; y < endY; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = startX; x < endX; x++)
                    {
                        var pixel = row[x];
                        if (pixel.A == 0) continue;
                        if (excludedPoints != null && excludedPoints.Contains(new Point(x, y))) continue;

                        float rDiff = (float)pixel.R - baseColor.R;
                        float gDiff = (float)pixel.G - baseColor.G;
                        float bDiff = (float)pixel.B - baseColor.B;

                        int newR = Math.Clamp((int)(newbaseColor.R + rDiff), 0, 255);
                        int newG = Math.Clamp((int)(newbaseColor.G + gDiff), 0, 255);
                        int newB = Math.Clamp((int)(newbaseColor.B + bDiff), 0, 255);

                        row[x] = new Rgba32((byte)newR, (byte)newG, (byte)newB, pixel.A);
                    }
                }
            });

            return result;
        }
        public static Image<Rgba32> ReplacePallete(Image<Rgba32> image, List<Rgba32> basePallete, List<Rgba32> newPallete)
        {
            var colorMap = new Dictionary<Rgba32, Rgba32>();

            for (int i = 0; i < basePallete.Count; i++)
            {
                colorMap[basePallete[i]] = newPallete[i];
            }
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var currentColor = row[x];
                        if (currentColor.A == 0)
                            continue;
                        if (colorMap.TryGetValue(currentColor, out var mappedColor))
                        {
                            row[x] = mappedColor;
                        }
                    }
                }
            });
            return image;
        }
        public static Image<Rgba32> CopyOnlyWithPallete(Image<Rgba32> source, List<Rgba32> pallete)
        {
            var result = new Image<Rgba32>(source.Width, source.Height);
            source.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var pixel = row[x];
                        if (pallete.Contains(pixel))
                        {
                            result[x, y] = pixel;
                        }
                    }
                }
            });


            return result;
        }
        public static Image<Rgba32> CopyRectangles(Image<Rgba32> targetImage, Image<Rgba32> source, List<Rectangle> rectangles)
        {
            foreach (var rect in rectangles)
            {
                using (var cropped = source.Clone(x => x.Crop(rect)))
                {
                    targetImage.Mutate(x => x.DrawImage(cropped, rect.Location, 1f));
                }
            }
            return targetImage;
        }
        public static Image<Rgba32> CopyVisibleFromPattern(Image<Rgba32> targetImage, Image<Rgba32> source, Image<Rgba32> pattern)
        {
            var result = targetImage.Clone();
            for (int y = 0; y < pattern.Height; y++)
            {
                for (int x = 0; x < pattern.Width; x++)
                {
                    var patternPixel = pattern[x, y];
                    if (patternPixel.A > 0)
                    {
                        result[x, y] = source[x, y];
                    }
                }
            }
            return result;
        }

        public static Image<Rgba32> CutRectangle(Image<Rgba32> image, Rectangle area)
        {
            var result = image.Clone();
            //reaplce area with transparent pixels
            result.ProcessPixelRows(accessor =>
            {
                int startY = Math.Max(0, area.Top);
                int endY = Math.Min(accessor.Height, area.Bottom);
                int startX = Math.Max(0, area.Left);
                int endX = Math.Min(accessor.Width, area.Right);
                for (int y = startY; y < endY; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = startX; x < endX; x++)
                    {
                        row[x] = new Rgba32(0, 0, 0, 0);
                    }
                }
            });
            return result;
        }
        public static Image<Rgba32> CenteredStripes(
       Image<Rgba32> sourceImage,
       Rectangle paletteSourceRect,
       Rectangle texturePartRect)
        {
            var colors = new List<ColorPallete>();

            int startY = Math.Min(paletteSourceRect.Y, sourceImage.Height - 1);
            int endY = Math.Min(paletteSourceRect.Bottom, sourceImage.Height);
            int startX = Math.Min(paletteSourceRect.X, sourceImage.Width - 1);
            int endX = Math.Min(paletteSourceRect.Right, sourceImage.Width);

            sourceImage.ProcessPixelRows(accessor =>
            {
                for (int y = startY; y < endY; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = startX; x < endX; x++)
                    {
                        var color = row[x];
                        if (color.A == 0)
                            continue;

                        colors.Add(ColorHelper.GetPallete(color));
                    }
                }
            });

            if (colors.Count == 0)
            {
                return sourceImage;
            }

            var croppedPart = sourceImage.Clone();
            croppedPart.Mutate(ctx => ctx.Crop(texturePartRect));

            int size = texturePartRect.Width;
            int centerX = (size - 1) / 2;
            int centerY = (size - 1) / 2;

            int maxGlobalIndex = size * 4;
            int globalIndex = 0;
            int sideIndex = 0;

            foreach (var color in colors)
            {
                if (globalIndex >= maxGlobalIndex) break;

                int edgeX = 0;
                int edgeY = 0;

                if (globalIndex < size)
                {
                    edgeX = 0;
                    edgeY = sideIndex;
                }
                else if (globalIndex < size * 2)
                {
                    edgeX = sideIndex;
                    edgeY = 0;
                }
                else if (globalIndex < size * 3)
                {
                    edgeX = size - 1;
                    edgeY = (size - 1) - sideIndex;
                }
                else
                {
                    edgeX = (size - 1) - sideIndex;
                    edgeY = size - 1;
                }

                var pixelColor = croppedPart[edgeX, edgeY];
                if (pixelColor.A != 0)
                {
                    var mappedColor = ColorHelper.MapToPallete(pixelColor, color);

                    int x = edgeX;
                    int y = edgeY;
                    int dx = Math.Abs(centerX - x);
                    int dy = Math.Abs(centerY - y);
                    int sx = x < centerX ? 1 : -1;
                    int sy = y < centerY ? 1 : -1;
                    int err = dx - dy;

                    while (true)
                    {
                        croppedPart[x, y] = mappedColor;

                        if (x == centerX && y == centerY) break;

                        int e2 = 2 * err;
                        if (e2 > -dy)
                        {
                            err -= dy;
                            x += sx;
                        }
                        if (e2 < dx)
                        {
                            err += dx;
                            y += sy;
                        }
                    }
                }

                sideIndex++;
                globalIndex++;

                if (sideIndex == size)
                {
                    sideIndex = 0;
                }
            }

            sourceImage.Mutate(ctx => ctx.DrawImage(croppedPart, new Point(texturePartRect.X, texturePartRect.Y), 1f));
            return sourceImage;
        }
    }
}
