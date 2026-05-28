using minecreator.api.Helpers;
using minecreator.api.Model;
using minecreator.api.Model.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Formats.Png;

namespace minecreator.api.Services
{
    public interface IModuleService
    {
        IOutfitModule GetModule(OutfitType type);
        Dictionary<OutfitType, OutfitModuleOptions> GetModulesOptions();
        void RegisterModule(OutfitType type, IOutfitModule module);
        TextureMap GenerateTexture(OutfitConfiguration config);
        public ModuleOutfitsResult GenerateOutfits(OutfitConfiguration config);
        public List<ModuleOutfitsResult> GenerateSets(List<ModuleOutfitsResult> outfits);
        public TextureMap GenerateFlatTexture(TextureMap textureMap);
    }

    public class ModuleService : IModuleService
    {
        private static readonly Type[] _moduleTypes = typeof(ModuleService).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IOutfitModule).IsAssignableFrom(t))
            .ToArray();

        private readonly Dictionary<OutfitType, IOutfitModule> _modules;
        public ModuleService()
        {
            _modules = new Dictionary<OutfitType, IOutfitModule>();

            foreach (var type in _moduleTypes)
            {
                var module = (IOutfitModule)Activator.CreateInstance(type)!;
                RegisterModule(module.OutfitType, module);
            }
        }
        public void RegisterModule(OutfitType type, IOutfitModule module)
        {
            _modules[type] = module;
        }
        public IOutfitModule GetModule(OutfitType type)
        {
            return _modules.TryGetValue(type, out var module) ? module : null;
        }
        public Dictionary<OutfitType, OutfitModuleOptions> GetModulesOptions()
        {
            var options = new Dictionary<OutfitType, OutfitModuleOptions>();
            foreach (var kvp in _modules)
            {
                options[kvp.Key] = kvp.Value.GetOptions();
            }
            return options;
        }
        public TextureMap GenerateTexture(OutfitConfiguration config)
        {
            if (config.Colors.Count == 0)
                config.Colors.Add(ColorHelper.DEFAULT_PALLETE.BaseColor);

            var module = GetModule(config.Type);
            if (module == null) throw new Exception($"No module registered for outfit type {config.Type}");
            module.SetConfiguration(config);
            module.GenerateBaseTexture();
            module.GenerateDetailsTexture();
            module.GenerateAccessoryTexture();
            module.GenerateColoredTexture();
            module.GenerateAccessories();
            var result = module.MergeTextures(true, true);
            return result;
        }
        public ModuleOutfitsResult GenerateOutfits(OutfitConfiguration config)
        {
            var samples = config.Samples;
            if (samples <= 0) samples = 1;

            var result = new ModuleOutfitsResult();
            result.Configuration = config;
            result.OutfitId = config.Id;
            result.Type = config.Type;

            for (int i = 0; i < samples; i++)
            {
                var configforSample = config;
                config.Samples = i;
                var textureMap = GenerateTexture(configforSample);
                result.Samples.Add(textureMap);
            }
            return result;
        }
        public TextureMap GenerateFlatTexture(TextureMap textureMap)
        {
            var texture = new TextureMap();
            texture.CopyParts(textureMap, new List<TextureMapPart>
                    {
                        TextureMapPart.HEAD
                    });
            var partsToMerge = new List<TextureMapPart>()
                    {
                        TextureMapPart.BODY,
                        TextureMapPart.LEFT_ARM,
                        TextureMapPart.RIGHT_ARM,
                        TextureMapPart.LEFT_LEG,
                        TextureMapPart.RIGHT_LEG
                    };
            foreach (var part in partsToMerge)
            {
                var innerpart = textureMap.GetPart(part);
                var outerpart = textureMap.GetOuterPart(part);

                var merged = TextureManupulationHelper.Merge(innerpart, outerpart);
                texture.SetPart(part, merged);
            }

            return texture;
        }
        public List<ModuleOutfitsResult> GenerateSets(List<ModuleOutfitsResult> outfits)
        {
            OutfitType primaryOutfitType;
            var results = new List<ModuleOutfitsResult>();
            var typeOrderForPicking = new List<OutfitType>
    {
        OutfitType.TOP,
        OutfitType.BOTTOM,
        OutfitType.HAT,
        OutfitType.SHOES
    };

            primaryOutfitType = typeOrderForPicking
                .AsEnumerable()
                .FirstOrDefault(type => outfits.Any(x => x.Type == type));

            var primaryOutfits = outfits.Where(x => x.Type == primaryOutfitType).ToList();

            foreach (var primary in primaryOutfits)
            {
                var textures = primary.Samples;
                foreach (var texture in textures)
                {
                    var dominantColor = ColorHelper.GetDominant(texture.Texture);
                    var dominantHue = ColorSpaceConverter.ToHsl(dominantColor).H;
                    float complementaryHue = (dominantHue + 180) % 360;

                    var dominatFromOthers = outfits
                      .Where(x => x.Type != primaryOutfitType)
                      .SelectMany(x => x.Samples, (parent, sample) => new
                      {
                          Type = parent.Type,
                          texture = sample.Texture,
                          color = ColorHelper.GetDominant(sample.Texture)
                      })
                      .ToList();

                    var mostSimilarColorsPerType = dominatFromOthers
                        .GroupBy(c => c.Type)
                        .Select(group => group
                            .OrderBy(c => ColorHelper.GetHueDistance(ColorSpaceConverter.ToHsl(c.color).H, dominantHue))
                            .First()
                        )
                        .ToList();

                    var mostComplementaryColorsPerType = dominatFromOthers
                        .GroupBy(c => c.Type)
                        .Select(group => group
                            .OrderBy(c => ColorHelper.GetHueDistance(ColorSpaceConverter.ToHsl(c.color).H, complementaryHue))
                            .First()
                        )
                        .ToList();

                    var typeOrder = new List<OutfitType>
            {
                OutfitType.BOTTOM,
                OutfitType.SHOES,
                OutfitType.TOP,
                OutfitType.HAT
            };

                    var orderedSimilar = mostSimilarColorsPerType
                        .OrderBy(x => typeOrder.IndexOf(x.Type))
                        .ToList();

                    if (orderedSimilar.Any())
                    {
                        var currentTexture = texture.Texture.Clone();
                        foreach (var item in orderedSimilar)
                        {
                            currentTexture = TextureManupulationHelper.Merge(currentTexture, item.texture);
                        }
                        results.Add(new ModuleOutfitsResult
                        {
                            OutfitId = primary.OutfitId,
                            Type = primary.Type,
                            Configuration = primary.Configuration,
                            Samples = primary.Samples,
                            Texture = new TextureMap { Texture = currentTexture }
                        });
                    }

                    var orderedComplementary = mostComplementaryColorsPerType
                        .OrderBy(x => typeOrder.IndexOf(x.Type))
                        .ToList();

                    if (orderedComplementary.Any())
                    {
                        var currentTexture = texture.Texture.Clone();
                        foreach (var item in orderedComplementary)
                        {
                            currentTexture = TextureManupulationHelper.Merge(currentTexture, item.texture);
                        }
                        results.Add(new ModuleOutfitsResult
                        {
                            OutfitId = primary.OutfitId,
                            Type = primary.Type,
                            Configuration = primary.Configuration,
                            Samples = primary.Samples,
                            Texture = new TextureMap { Texture = currentTexture }
                        });
                    }
                }
            }
            //remove duplicates
            results = results
                .GroupBy(x => x.Texture.Texture.ToBase64String(PngFormat.Instance))
                .Select(g => g.First())
                .ToList();

            return results;
        }
    }
}
