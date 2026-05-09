using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class ColorHelper
    {
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
        public static Rgba32 GetBrighter(List<Rgba32> pallete,Rgba32 color)
        {
            var index = pallete.IndexOf(color);
            if (index < pallete.Count - 1)
            {
                return pallete[index + 1];
            }
            return color;
        }
    }
}
