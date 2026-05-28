using Microsoft.AspNetCore.Http.HttpResults;
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
            var sets = new List<ModuleOutfitsResult>();
            if (request.GenerateSets)
            {
                sets = _moduleService.GenerateSets(modulesResults);
            }

            var moduleTextures = new List<GenerateResponse>();
            foreach (var item in modulesResults)
            {
                var sampleIndex = 0;
                foreach (var sample in item.Samples)
                {
                    var result = sample;
                    if (request.GameVersion == "beta")
                    {
                        result = _moduleService.GenerateFlatTexture(result);
                    }
                    var stream = new MemoryStream();
                    result.Texture.SaveAsPng(stream);
                    var image = stream.ToArray();
                    moduleTextures.Add(new GenerateResponse()
                    {
                        Config = new OutfitConfigurationModel()
                        {
                            Id = item.OutfitId,
                            Type = item.Type.ToString(),
                            Name = item.Configuration.Name,
                            Samples = sampleIndex
                        },
                        Image = image
                    });
                    sampleIndex++;
                }
            }
            var setsTextures = new List<GenerateResponse>();
            foreach (var item in sets)
            {
                var result = item;
                if (request.GameVersion == "beta")
                {
                    result.Texture = _moduleService.GenerateFlatTexture(result.Texture);
                }
                var texture = result.Texture.Texture;
                var stream = new MemoryStream();
                texture.SaveAsPng(stream);
                var image = stream.ToArray();
                setsTextures.Add(new GenerateResponse()
                {
                    Config = new OutfitConfigurationModel()
                    {
                        Id = item.OutfitId,
                        Type = item.Type.ToString(),
                        Name = item.Configuration.Name,
                    },
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
                if (request.GameVersion == "beta")
                {
                    result = _moduleService.GenerateFlatTexture(result);
                }
                var texture = result.ToBase64();
                var stream = new MemoryStream();
                result.Texture.SaveAsPng(stream);
                image = stream.ToArray();
                response.Add(new GenerateResponse()
                {
                    Image = image
                });
            }
            return Ok(new { outfits = response });
        }
    }
}
