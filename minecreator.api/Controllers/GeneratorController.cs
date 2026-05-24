using Microsoft.AspNetCore.Mvc;
using minecreator.api.Model;
using minecreator.api.Services;
using SixLabors.ImageSharp;

namespace minecreator.api.Controllers
{
    [ApiController]
    public class GeneratorController : Controller
    {
        private readonly IModuleService _moduleService;
        private readonly IConfigurationService _configurationService;
        public GeneratorController(IModuleService moduleService, IConfigurationService configurationService)
        {
            _moduleService = moduleService;
            _configurationService = configurationService;
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetConfiguration()
        {
            var appconfig = _configurationService.GetConfig();
            var modulesconfig = _moduleService.GetModulesOptions();
            var response = new ConfigurationResponse(appconfig, modulesconfig).ToResponse();


            return Ok(response);
        }
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateRequest request)
        {
            var list = new List<OutfitConfiguration>();
            foreach (var item in request.Outfits)
            {
                list.Add(item.ToConfig(request.Model));
            }

            var modulesResults = new List<ModuleOutfitsResult>();
            foreach (var item in list)
            {
                var results = _moduleService.GenerateOutfits(item);
                modulesResults.AddRange(results);
            }
            var sets = new List<TextureMap>();
            if (request.GenerateSets)
            {
                sets = _moduleService.GenerateSets(modulesResults);
            }

            var moduleTextures = new List<GenerateResponse>();
            foreach (var item in modulesResults)
            {
                foreach (var sample in item.Samples)
                {
                    var texture = sample.ToBase64();
                    var stream = new MemoryStream();
                    sample.Texture.SaveAsPng(stream);
                    var image = stream.ToArray();
                    moduleTextures.Add(new GenerateResponse()
                    {
                        Config = new OutfitConfiguration()
                        {
                            Id = item.OutfitId,
                            Type = item.Type
                        },
                        Image = image
                    });
                }
            }
            var setsTextures = new List<GenerateResponse>();
            foreach (var item in sets)
            {
                var texture = item.Texture;
                var stream = new MemoryStream();
                texture.SaveAsPng(stream);
                var image = stream.ToArray();
                setsTextures.Add(new GenerateResponse()
                {
                    Image = image
                });
            }

            return Ok(new { outfits = moduleTextures, sets = setsTextures });
        }
        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromBody] GenerateRequest request)
        {
            var list = new List<OutfitConfiguration>();
            var response = new List<GenerateResponse>();
            list.Add(request.Outfits[0].ToConfig(request.Model));
            byte[] image = null;
            foreach (var item in list)
            {
                item.Samples = 0;
                var result = _moduleService.GenerateTexture(item);
                var texture = result.ToBase64();
                var stream = new MemoryStream();
                result.Texture.SaveAsPng(stream);
                image = stream.ToArray();
                response.Add(new GenerateResponse()
                {
                    Config = item,
                    Image = image
                });
            }
            return Ok(new { outfits = response });
        }
    }
}
