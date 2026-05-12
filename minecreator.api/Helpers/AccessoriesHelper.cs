using minecreator.api.Accessories;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public class OutfitAccessoryLocation
    {
        public Rectangle Location { get; set; }
        public OutfitAccessory Type { get; set; }
    }
    public static class AccessoriesHelper
    {
        private static List<OutfitAccessoryItem> _accessories = new List<OutfitAccessoryItem>()
        {
            new CreeperPatch()
        };
        public static List<OutfitAccessoryItem> GetAccessoriesForOutfit(OutfitAccessory type)
        {
            return _accessories.Where(a => a.Type == type).ToList();
        }
        public static List<OutfitAccessoryItem> GetAccessoriesForLocation(OutfitAccessoryLocation location)
        {
            return _accessories.Where(a => a.Type == location.Type && a.Size.X <= location.Location.Width && a.Size.Y <= location.Location.Height).ToList();
        }
        public static Image<Rgba32> LoadAccessory(OutfitAccessoryItem accessory)
        {
            var imgBytes = Convert.FromBase64String(accessory.Texture);
            var texture = Image.Load<Rgba32>(imgBytes);
            return texture;
        }
        public static Image<Rgba32> ApplyConfigurationToAccessory(OutfitAccessoryItem accessory, OutfitConfiguration config)
        {
            var texture = LoadAccessory(accessory);
            if (accessory.IsReadyForColor == false)
                return texture;

            //coloring
            var baseColor = config.Colors[0];
            var contrastColor = ColorHelper.GetContrast(baseColor, config.Colors);
            var colorpallete = ColorHelper.GenerateDefaultPallete(contrastColor);
            var colors = ColorHelper.ExtractAndSortColorsByLuminance(texture);
            colors = ColorHelper.ExpandPalette(colors, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base], colorpallete.Count);

            texture=TextureManupulationHelper.ReplacePallete(texture, colors, colorpallete);
            return texture;
        }
    }
}

