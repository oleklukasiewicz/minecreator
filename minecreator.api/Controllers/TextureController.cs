using Microsoft.AspNetCore.Mvc;
using minecreator.api.Helpers;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextureController : ControllerBase
    {

        //[HttpGet("test-top-module")]
        //public IActionResult TestTopOutfitModule(
        //    [FromQuery] OutfitStyle style = OutfitStyle.CASUAL,
        //    [FromQuery] OutfitModel model = OutfitModel.CLASSIC,
        //    [FromQuery] string colorsHex = "#111f2f,#3cbef5",
        //    [FromQuery] string seed = "olek128")
        //{
        //    try
        //    {

        //        var colors = colorsHex.Split(',').Select(Color.ParseHex).Select(c => c.ToPixel<Rgba32>()).ToList();
        //        var config = new OutfitConfiguration
        //        {
        //            Type = OutfitType.TOP,
        //            Style = style,
        //            Colors = colors,
        //            Model = model,
        //            Seed = seed,
        //            Accessories = new List<OutfitAccessory>
        //            {
                      
                      
        //            }
        //        };


        //        //var module = new minecreator.api.Modules.TopOutfitTypeModule(config);

        //        //var textureMap = module.GenerateBaseTexture();
        //        //textureMap = module.GenerateDetailsTexture();
        //        //textureMap = module.GenerateAccessoryTexture();
        //        //textureMap = module.GenerateColoredTexture();
        //        //var acctexture = module.GenerateAccessories();
        //        //var txt = acctexture.ToBase64();
        //        //textureMap = module.MergeTextures(true, true);


        //        //var image = textureMap.Texture;

        //        //if (image == null)
        //        //{
        //        //    return NotFound("Failed to generate texture map from TopOutfitTypeModule.");
        //        //}

        //        //using var ms = new MemoryStream();
        //        //image.Save(ms, new PngEncoder());
        //        //return File(ms.ToArray(), "image/png");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Failed to generate module texture: {ex.Message}");
        //    }
        //}
    }
}