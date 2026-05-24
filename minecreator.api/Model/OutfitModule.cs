using minecreator.api.Helpers;
using minecreator.api.Model.Interface;

namespace minecreator.api.Model
{
    public abstract class OutfitModule : IOutfitModule
    {
        public abstract OutfitType OutfitType { get; }
        public OutfitConfiguration Configuration { get; set; }
        public OutfitModuleWorkspace Workspace { get; set; }
        public OutfitModuleOptions Options { get; set; } = new OutfitModuleOptions();

        public abstract TextureMap GenerateAccessories();

        public abstract TextureMap GenerateAccessoryTexture();

        public abstract TextureMap GenerateBaseTexture();

        public abstract TextureMap GenerateColoredTexture();

        public abstract TextureMap GenerateDetailsTexture();
        public OutfitModuleOptions GetOptions()
        {
            return Options;
        }

        public TextureMap MergeTextures(bool details, bool accessories)
        {
            if (details)
            {
                Workspace.Texture.Texture = TextureManupulationHelper.Merge(Workspace.Texture.Texture, Workspace.DetailsTexture.Texture);
            }
            if (accessories)
            {
                Workspace.Texture.Texture = TextureManupulationHelper.Merge(Workspace.Texture.Texture, Workspace.AccessoryTexture.Texture);
            }
            return Workspace.Texture;
        }

        public bool SetConfiguration(OutfitConfiguration config)
        {
            Configuration = config;
            Workspace = new OutfitModuleWorkspace();
            Workspace.Characteristics = new OutfityTypeCharacteristics(Configuration.Seed, config.Style,config.Samples);
            Workspace.BaseTexture = BaseTextureHelper.LoadBaseTexture(Configuration.Style, Configuration.Type, Configuration.Model, Workspace.Characteristics.Material);

            var userColorsPallets = new List<ColorPallete>();
            foreach (var color in Configuration.Colors)
            {
                var pallete = ColorHelper.GenerateDefaultPallete(color);
                userColorsPallets.Add(new ColorPallete
                {
                    BaseColor = color,
                    Colors = pallete
                });
            }

            Workspace.UserPallets = userColorsPallets;
            return true;
        }
    }
    public class OutfitModuleWorkspace
    {
        public TextureMap BaseTexture { get; set; }
        public TextureMap Texture { get; set; } = new TextureMap();
        public TextureMap DetailsTexture { get; set; } = new TextureMap();
        public TextureMap AccessoryTexture { get; set; } = new TextureMap();
        public List<OutfitAccessoryLocation> AccessoryLocations { get; set; } = new List<OutfitAccessoryLocation>();
        public OutfityTypeCharacteristics Characteristics { get; set; }
        public List<ColorPallete> UserPallets { get; set; }
    }
    public class OutfitModuleOptions
    {
        public List<OutfitAccessory> Accessory { get; set; } = new List<OutfitAccessory>();
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle> { OutfitStyle.CASUAL };

    }
}
