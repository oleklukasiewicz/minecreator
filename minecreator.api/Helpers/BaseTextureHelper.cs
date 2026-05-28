using System.Reflection;
using minecreator.api.Bases.Top.Casual;
using minecreator.api.BaseTextures.Bottom.Casual;
using minecreator.api.BaseTextures.Shoes.Casual;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class BaseTextureHelper
    {
        private static List<IBaseTexture> baseTextures;

        static BaseTextureHelper()
        {
            var baseType = typeof(IBaseTexture);
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            baseTextures = types.Select(t => (IBaseTexture)Activator.CreateInstance(t)).ToList();
        }
        public static TextureMap? LoadBaseTexture(OutfitStyle style, OutfitType type, OutfitModel model, int hash=1)
        {
            IBaseTexture baseTexture;
            var found = baseTextures.Where(bt => (bt.Styles.Contains(style) || bt.Styles.Count == 0) && bt.Type == type && (bt.Model == model || bt.Model == OutfitModel.BOTH)).ToList();
            if (found?.Count() > 0)
            {
                baseTexture = found[hash % found.Count()];
            }
            else
                baseTexture = found?.FirstOrDefault();

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
