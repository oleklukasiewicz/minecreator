using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public enum TextureGlobalColor
    {
        Base,
        BottomOutline,
        FrontOutline,
        Accessories,
    }
    public class TextureColorPallete
    {
        public TextureGlobalColor ColorType { get; set; }
        public List<Rgba32> Colors { get; set; }
    }
    public class ColorPallete
    {
        public Rgba32 BaseColor { get; set; }
        public List<Rgba32> Colors { get; set; }
    }
    public static class ColorHelper
    {
        private static int _palleteColorCount = 5;
        private static int _maxPalletsCount = 5;
        public static List<ColorPallete> COLORS_PALLETE { get; private set; } = new List<ColorPallete>();

        public static ColorPallete DEFAULT_PALLETE
        {
            get => COLORS_PALLETE.Where(x => x.BaseColor.R == x.BaseColor.G && x.BaseColor.R == x.BaseColor.B).FirstOrDefault();
        }
        public static int PalleteColorSize => _palleteColorCount;
        public static int MaxPalletsCount => _maxPalletsCount;
        public static void Init(int palletesize=5,int maxPalletsCount=5)
        {
            _palleteColorCount = palletesize;
            _maxPalletsCount = maxPalletsCount;

            int[] colorValue = new int[]
            {
                128,96
            };
            

            var mutations = new List<Rgba32>();

            mutations.Add(new Rgba32((byte)colorValue[0], (byte)colorValue[0], (byte)colorValue[0], 255));

            foreach (var r in colorValue)
            {
                foreach (var g in colorValue)
                {
                    foreach (var b in colorValue)
                    {
                        if (r == g && g == b)
                        {
                            continue;
                        }
                        mutations.Add(new Rgba32((byte)r, (byte)g, (byte)b, 255));
                        if (mutations.Count >= (_maxPalletsCount - 2))
                        {
                            break;
                        }
                    }
                }
            }
            foreach (var color in mutations)
            {
                var colors = ExpandPalette(new List<Rgba32>(), color, _palleteColorCount);
                var colropallete = new ColorPallete
                {
                    BaseColor = color,
                    Colors = colors
                };
                COLORS_PALLETE.Add(colropallete);
            }
        }


        public static Rgba32 MapColor(Rgba32 color, Rgba32 targetBaseColor)
        {
            var targetPallete = COLORS_PALLETE.FirstOrDefault(p => p.BaseColor == targetBaseColor);
            var baseColorPallete = GetPallete(color);
            if (baseColorPallete == null)
            {
                return color;
            }
            var colorIndex = baseColorPallete.Colors.IndexOf(color);
            if (colorIndex != -1)
            {
                return targetPallete.Colors[colorIndex];
            }
            return color;
        }
        public static Rgba32 MapColor(Rgba32 color, List<ColorPallete> pallets)
        {
            var foundpallete = GetPallete(color);
            if (foundpallete == null)
                return color;
            var palleteIndex = COLORS_PALLETE.IndexOf(foundpallete);
            var colorIndex = foundpallete.Colors.IndexOf(color);
            return pallets[palleteIndex].Colors[colorIndex];
        }
        public static Rgba32 MapToPallete(Rgba32 color, ColorPallete pallete)
        {
            var basePallete = GetPallete(color);
            if (basePallete == null)
                return color;
            var colorIndex = basePallete.Colors.IndexOf(color);
            if (colorIndex != -1)
            {
                return pallete.Colors[colorIndex];
            }
            return color;
        }
        public static ColorPallete GetPallete(Rgba32 color, List<ColorPallete> pallets = null)
        {
            if (pallets == null)
                pallets = COLORS_PALLETE;
            return pallets.Where(p => p.Colors.Contains(color)).FirstOrDefault();
        }
        public static Dictionary<TextureGlobalColor, Rgba32> GLOBAL_COLORS = new Dictionary<TextureGlobalColor, Rgba32>()
    {
        { TextureGlobalColor.Base, new Rgba32(128, 128, 128, 255) },
        { TextureGlobalColor.BottomOutline, new Rgba32(96, 128, 96, 255) },
        { TextureGlobalColor.FrontOutline, new Rgba32(128, 96, 96, 255) },
        { TextureGlobalColor.Accessories, new Rgba32(96, 96, 128, 255) },
    };
        public static List<Rgba32> ExtractAndSortColorsByLuminance(Image<Rgba32> image)
        {
            var colors = new HashSet<Rgba32>();

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    foreach (ref Rgba32 pixel in pixelRow)
                    {
                        if (pixel.A > 0)
                        {
                            colors.Add(pixel);
                        }
                    }
                }
            });
            return colors
                .OrderBy(c => ColorSpaceConverter.ToHsl(c).L)
                .ToList();
        }
        public static List<Rgba32> GetColorsFromPallete(List<Rgba32> palette, Rgba32 baseColor)
        {
            int diffRG = baseColor.R - baseColor.G;
            int diffGB = baseColor.G - baseColor.B;
            int diffRB = baseColor.R - baseColor.B;

            int tolerance = 0;

            return palette
                .Where(c =>
                {
                    int cDiffRG = c.R - c.G;
                    int cDiffGB = c.G - c.B;
                    int cDiffRB = c.R - c.B;

                    bool matchRG = Math.Abs(cDiffRG - diffRG) <= tolerance;
                    bool matchGB = Math.Abs(cDiffGB - diffGB) <= tolerance;
                    bool matchRB = Math.Abs(cDiffRB - diffRB) <= tolerance;

                    return matchRG && matchGB && matchRB;
                })
                .OrderBy(c => c.R + c.G + c.B)
                .ToList();
        }
        public static List<Rgba32> ExpandPalette(List<Rgba32> inputColors, Rgba32 baseColor, int targetCount = 5)
        {
            int sideCount = targetCount / 2;
            float baseBrightness = baseColor.R + baseColor.G + baseColor.B;

            var darkerInputs = inputColors
                .Where(c => (c.R + c.G + c.B) < baseBrightness)
                .OrderByDescending(c => c.R + c.G + c.B)
                .Distinct()
                .ToList();

            var brighterInputs = inputColors
                .Where(c => (c.R + c.G + c.B) > baseBrightness)
                .OrderBy(c => c.R + c.G + c.B)
                .Distinct()
                .ToList();
            int stepR = 16, stepG = 16, stepB = 16;
            if (darkerInputs.Count > 0)
            {
                stepR = Math.Abs(baseColor.R - darkerInputs[0].R);
                stepG = Math.Abs(baseColor.G - darkerInputs[0].G);
                stepB = Math.Abs(baseColor.B - darkerInputs[0].B);
            }
            else if (brighterInputs.Count > 0)
            {
                stepR = Math.Abs(brighterInputs[0].R - baseColor.R);
                stepG = Math.Abs(brighterInputs[0].G - baseColor.G);
                stepB = Math.Abs(brighterInputs[0].B - baseColor.B);
            }

            List<Rgba32> finalLower = new List<Rgba32>();
            for (int i = 0; i < sideCount; i++)
            {
                if (i < darkerInputs.Count)
                {
                    finalLower.Add(darkerInputs[i]);
                }
                else
                {
                    var refCol = finalLower.Count > 0 ? finalLower.Last() : baseColor;
                    finalLower.Add(new Rgba32(
                        (byte)Math.Clamp(refCol.R - stepR, 0, 255),
                        (byte)Math.Clamp(refCol.G - stepG, 0, 255),
                        (byte)Math.Clamp(refCol.B - stepB, 0, 255),
                        255));
                }
            }

            List<Rgba32> finalUpper = new List<Rgba32>();
            for (int i = 0; i < sideCount; i++)
            {
                if (i < brighterInputs.Count)
                {
                    finalUpper.Add(brighterInputs[i]);
                }
                else
                {
                    var refCol = finalUpper.Count > 0 ? finalUpper.Last() : baseColor;
                    finalUpper.Add(new Rgba32(
                        (byte)Math.Clamp(refCol.R + stepR, 0, 255),
                        (byte)Math.Clamp(refCol.G + stepG, 0, 255),
                        (byte)Math.Clamp(refCol.B + stepB, 0, 255),
                        255));
                }
            }

            var result = new List<Rgba32>();
            result.AddRange(finalLower.OrderBy(c => c.R + c.G + c.B));
            result.Add(baseColor);
            result.AddRange(finalUpper.OrderBy(c => c.R + c.G + c.B));

            return result;
        }
        public static List<Rgba32> GeneratePallete(Rgba32 baseColor, int colorCount, int hueShift, int saturationShift, int valueShift)
        {
            var pallete = new List<Rgba32>();
            var shadeCount = 0;

            if (colorCount % 2 == 0)
                shadeCount = (colorCount / 2) - 1;
            else
                shadeCount = colorCount / 2;

            var hslColor = ColorSpaceConverter.ToHsl(baseColor);

            for (var i = 0; i < colorCount; i++)
            {
                var shadeIndex = i - shadeCount;

                float newHue = (hslColor.H + (shadeIndex * hueShift)) % 360;
                if (newHue < 0) newHue += 360;

                float saturationChange = (shadeIndex * saturationShift) / 100f;
                var newSaturation = Math.Clamp(hslColor.S - saturationChange, 0, 1);

                float valueChange = (shadeIndex * valueShift) / 100f;
                var newValue = Math.Clamp(hslColor.L + valueChange, 0, 1);

                var newColor = ColorSpaceConverter.ToRgb(new Hsl(newHue, newSaturation, newValue));
                pallete.Add(newColor);
            }
            return pallete;
        }
        public static List<Rgba32> GenerateDefaultPallete(Rgba32 baseColor)
        {
            return GeneratePallete(baseColor, _palleteColorCount, 15, 8, 5);
        }
        public static Rgba32 GetBrighter(List<Rgba32> pallete, Rgba32 color)
        {
            var index = pallete.IndexOf(color);
            if (index < pallete.Count - 1)
            {
                return pallete[index + 1];
            }
            return color;
        }
        public static Rgba32 GetBrighter(ColorPallete pallete, Rgba32 color)
        {
            var index = pallete.Colors.IndexOf(color);
            if (index < pallete.Colors.Count - 1)
            {
                return pallete.Colors[index + 1];
            }
            return color;
        }
        public static Rgba32 GetContrast(Rgba32 baseColor, List<Rgba32> colors)
        {
            if (colors == null || colors.Count == 0)
                return baseColor;
            float baseLuminance = GetRelativeLuminance(baseColor);

            Rgba32 bestColor = colors[0];
            float maxDifference = -1f;

            foreach (var color in colors)
            {
                float currentLuminance = GetRelativeLuminance(color);
                float difference = Math.Abs(baseLuminance - currentLuminance);

                if (difference > maxDifference)
                {
                    maxDifference = difference;
                    bestColor = color;
                }
            }

            return bestColor;
        }
        private static float GetRelativeLuminance(Rgba32 color)
        {
            return (0.2126f * color.R / 255f) + (0.7152f * color.G / 255f) + (0.0722f * color.B / 255f);
        }
        public static Rgba32 GetDominant(Image<Rgba32> image)
        {
            Rgba32 dominantColor = default;
            if (image != null)
            {
                var colorCounts = new Dictionary<Rgba32, int>();
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                        foreach (ref Rgba32 pixel in pixelRow)
                        {
                            if (pixel.A > 0)
                            {
                                if (colorCounts.ContainsKey(pixel))
                                {
                                    colorCounts[pixel]++;
                                }
                                else
                                {
                                    colorCounts[pixel] = 1;
                                }
                            }
                        }
                    }
                });
                dominantColor = colorCounts.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
            }
            return dominantColor;
        }
    }
}
