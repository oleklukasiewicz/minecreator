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

    public static class ColorHelper
    {
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
            int stepR = 25, stepG = 25, stepB = 25;
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
            return GeneratePallete(baseColor, 5, 15, 8, 5);
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
        public static Dictionary<TextureGlobalColor, TextureColorPallete> ExpandToGlobalPallets(List<Rgba32> inputColors, int maxColors = 5)
        {
            var result = new Dictionary<TextureGlobalColor, TextureColorPallete>();
            foreach (var inputColor in GLOBAL_COLORS)
            {
                var matchedColors = GetColorsFromPallete(inputColors, inputColor.Value);
                var expanded = ExpandPalette(matchedColors, inputColor.Value, maxColors);
                var pallete = new TextureColorPallete
                {
                    ColorType = inputColor.Key,
                    Colors = expanded
                };
                if (!result.ContainsKey(inputColor.Key))
                {
                    result[inputColor.Key] = pallete;
                }
            }
            return result;
        }
        public static Rgba32 GetContrast(Rgba32 baseColor, List<Rgba32> pallete)
        {
            if (pallete == null || pallete.Count == 0)
                return baseColor;
            float baseLuminance = GetRelativeLuminance(baseColor);

            Rgba32 bestColor = pallete[0];
            float maxDifference = -1f;

            foreach (var color in pallete)
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
    }
}
