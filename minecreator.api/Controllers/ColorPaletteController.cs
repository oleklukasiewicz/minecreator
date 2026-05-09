using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using minecreator.api.Helpers;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorPaletteController : ControllerBase
    {
        [HttpGet("generate")]
        public IActionResult GeneratePalette(
            [FromQuery, Required] string hexColor,
            [FromQuery] int colorCount = 5,
            [FromQuery] int hueShift = 10,
            [FromQuery] int saturationShift = 10,
            [FromQuery] int valueShift = 10)
        {
            try
            {
                // Ensure proper hex format to parse with ImageSharp
                if (!hexColor.StartsWith("#"))
                {
                    hexColor = "#" + hexColor;
                }

                var baseColor = Rgba32.ParseHex(hexColor);

                var palette = ColorHelper.GeneratePallete(baseColor, colorCount, hueShift, saturationShift, valueShift);

                // Return as an array of hex strings
                var hexPalette = palette.Select(c => c.ToHex()).ToList();

                return Ok(hexPalette);
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid color format or parameters: {ex.Message}");
            }
        }

        [HttpGet("generate-preview")]
        public IActionResult GeneratePalettePreview(
            [FromQuery, Required] string hexColor,
            [FromQuery] int colorCount = 5,
            [FromQuery] int hueShift = 10,
            [FromQuery] int saturationShift = 10,
            [FromQuery] int valueShift = 10)
        {
            try
            {
                if (!hexColor.StartsWith("#"))
                {
                    hexColor = "#" + hexColor;
                }

                var baseColor = Rgba32.ParseHex(hexColor);
                var palette = ColorHelper.GeneratePallete(baseColor, colorCount, hueShift, saturationShift, valueShift);

                var htmlBuilder = new System.Text.StringBuilder();
                htmlBuilder.AppendLine("<!DOCTYPE html>");
                htmlBuilder.AppendLine("<html><head><style>");
                htmlBuilder.AppendLine("body { font-family: sans-serif; display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100vh; margin: 0; background-color: #222; }");
                htmlBuilder.AppendLine(".palette { display: flex; gap: 10px; }");
                htmlBuilder.AppendLine(".color-box { width: 100px; height: 100px; border-radius: 8px; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; text-shadow: 1px 1px 2px black; border: 2px solid white; box-shadow: 0 4px 8px rgba(0,0,0,0.5);}");
                htmlBuilder.AppendLine("</style></head><body>");
                htmlBuilder.AppendLine("<div class='palette'>");

                foreach (var color in palette)
                {
                    var hex = "#" + color.ToHex();
                    htmlBuilder.AppendLine($"<div class='color-box' style='background-color: {hex};'>{hex}</div>");
                }

                htmlBuilder.AppendLine("</div></body></html>");

                return Content(htmlBuilder.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid color format or parameters: {ex.Message}");
            }
        }
        [HttpGet("generate-default")]
        public IActionResult GenerateDefaultPalette()
        {
            var defaultColors = new List<string>
            {
                "d5617c",
                "d6976a",
                "d5c473",
                "6ed69c",
                "6ab3db",
                "8a61de",
                "aaaaaa",
                "576944"
            };

            var htmlBuilder = new System.Text.StringBuilder();
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html><head><style>");
            htmlBuilder.AppendLine("body { font-family: sans-serif; display: flex; flex-direction: column; align-items: center; padding: 40px; margin: 0; background-color: #222; color: white; gap: 40px; }");
            htmlBuilder.AppendLine(".palette-container { display: flex; flex-direction: column; align-items: center; gap: 10px; }");
            htmlBuilder.AppendLine(".palette { display: flex; gap: 10px; }");
            htmlBuilder.AppendLine(".color-box { width: 80px; height: 80px; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 12px; font-weight: bold; text-shadow: 1px 1px 2px black; border: 2px solid white; box-shadow: 0 4px 8px rgba(0,0,0,0.5);}");
            htmlBuilder.AppendLine("h2 { margin: 0; text-transform: uppercase; letter-spacing: 2px; }");
            htmlBuilder.AppendLine("</style></head><body>");
            htmlBuilder.AppendLine("<h1>Default Palettes Preview (ColorHelper.GenerateDefaultPallete)</h1>");

            foreach (var hexColor in defaultColors)
            {
                try
                {
                    var baseColor = Rgba32.ParseHex(hexColor);
                    var palette = ColorHelper.GenerateDefaultPallete(baseColor);

                    htmlBuilder.AppendLine("<div class='palette-container'>");
                    htmlBuilder.AppendLine($"<h2>Base: {hexColor}</h2>");
                    htmlBuilder.AppendLine("<div class='palette'>");

                    foreach (var color in palette)
                    {
                        var hex = "#" + color.ToHex();
                        htmlBuilder.AppendLine($"<div class='color-box' style='background-color: {hex};'>{hex}</div>");
                    }

                    htmlBuilder.AppendLine("</div></div>");
                }
                catch (Exception ex)
                {
                    htmlBuilder.AppendLine($"<p>Failed to generate palette for {hexColor}: {ex.Message}</p>");
                }
            }

            htmlBuilder.AppendLine("</body></html>");

            return Content(htmlBuilder.ToString(), "text/html");
        }
    }
}