using Microsoft.AspNetCore.Mvc;
using minecreator.api.Helpers;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextureController : ControllerBase
    {

        [HttpGet("test-top-module")]
        public IActionResult TestTopOutfitModule(
            [FromQuery] OutfitStyle style = OutfitStyle.CASUAL,
            [FromQuery] OutfitModel model = OutfitModel.CLASSIC,
            [FromQuery] string colorHex = "#262B38",
            [FromQuery] string seed = "olek128")
        {
            try
            {

                var color = Color.ParseHex(colorHex);
                var rgbs = color.ToPixel<Rgba32>();
                var config = new OutfitConfiguration
                {
                    Type = OutfitType.TOP,
                    Style = style,
                    Colors = new List<SixLabors.ImageSharp.PixelFormats.Rgba32>
                    {
                        rgbs
                    },
                    Model = model,
                    Seed = seed
                };

                var module = new minecreator.api.Modules.TopOutfitTypeModule(config);

                var textureMap = module.GenerateBaseTexture();
                textureMap = module.GenerateColoredTexture();
                var image = textureMap.Texture;

                if (image == null)
                {
                    return NotFound("Failed to generate texture map from TopOutfitTypeModule.");
                }

                using var ms = new MemoryStream();
                image.Save(ms, new PngEncoder());
                return File(ms.ToArray(), "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate module texture: {ex.Message}");
            }
        }
    }
}