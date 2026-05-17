using minecreator.api.Model;
using minecreator.api.Model.Interface;

namespace minecreator.api.Services
{
    public interface IModuleService
    {
        IOutfitModule GetModule(OutfitType type);
        Dictionary<OutfitType, OutfitModuleOptions> GetModulesOptions();
        void RegisterModule(OutfitType type, IOutfitModule module);
        TextureMap GenerateTexture(OutfitConfiguration config);
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
    }
}
