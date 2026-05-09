using minecreator.api.Bases.Top.Casual;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class BaseTextureHelper
    {
        private static List<BaseTexture> baseTextures = new List<BaseTexture>()
        {
            new TopCasualBaseTexture()
        };
        public static TextureMap? LoadBaseTexture(OutfitStyle style, OutfitType type, OutfitModel model)
        {
            var baseTexture = baseTextures.FirstOrDefault(bt => bt.Style == style && bt.Type == type && bt.Model == model);
            if (baseTexture == null)
            {
                return null;
            }
            var base64Texture = baseTexture.Texture;
            var imgBytes = Convert.FromBase64String(base64Texture);
            var texture = Image.Load<Rgba32>(imgBytes);
            return new TextureMap { Texture = texture };
        }
    }
}
