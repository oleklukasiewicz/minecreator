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
            byte[] image = null;
            foreach (var item in list)
            {
                var result = _moduleService.GenerateTexture(item);

                var stream = new MemoryStream();
                result.Texture.SaveAsPng(stream);
                image = stream.ToArray();
            }


            return Ok(image);
        }
    }
}
