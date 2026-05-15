namespace minecreator.api.Model
{
    public class ConfigurationResponse
    {
        private AppConfig _appConfig;
        private Dictionary<OutfitType, OutfitModuleOptions> _modulesConfig;

        public ConfigurationResponse(AppConfig appConfig, Dictionary<OutfitType, OutfitModuleOptions> modulesConfig)
        {
            _appConfig = appConfig;
            _modulesConfig = modulesConfig;
        }
        public dynamic ToResponse()
        {
            return new
            {
                AppConfig = _appConfig,
                ModulesConfig = _modulesConfig.Select(k => new
                {
                    Name = k.Key.ToString(),
                    Accessory = k.Value.Accessory.Select(a => a.ToString()).ToArray(),
                    Styles = k.Value.Styles.Select(s => s.ToString()).ToArray()
                }).ToList()
            };
        }
    }
}
