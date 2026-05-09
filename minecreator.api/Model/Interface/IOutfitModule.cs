using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Model.Interface
{
    public interface IOutfitModule
    {
        public bool SetConfiguration(OutfitConfiguration config);
        public TextureMap GenerateBaseTexture();
        public TextureMap GenerateColoredTexture();
        public TextureMap GenerateAccessoryTexture();
    }
}
