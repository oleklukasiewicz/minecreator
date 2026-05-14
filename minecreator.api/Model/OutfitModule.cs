using minecreator.api.Helpers;
using minecreator.api.Model.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace minecreator.api.Model
{
    public class OutfitModule : IOutfitModule
    {
        public OutfitConfiguration Configuration { get; set; }
        public OutfitModuleWorkspace Workspace { get; set; }

        public TextureMap GenerateAccessories()
        {
            throw new NotImplementedException();
        }

        public TextureMap GenerateAccessoryTexture()
        {
            throw new NotImplementedException();
        }

        public TextureMap GenerateBaseTexture()
        {
            throw new NotImplementedException();
        }

        public TextureMap GenerateColoredTexture()
        {
            throw new NotImplementedException();
        }

        public TextureMap GenerateDetailsTexture()
        {
            throw new NotImplementedException();
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
            Workspace.Characteristics = new OutfityTypeCharacteristics(Configuration.Seed);
            Workspace.BaseTexture = BaseTextureHelper.LoadBaseTexture(Configuration.Style, Configuration.Type, Configuration.Model);

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
}
