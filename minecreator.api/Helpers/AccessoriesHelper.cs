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
        public static Image<Rgba32> PaintAccessory(Image<Rgba32> texture, ColorPallete pallete)
        {

            texture.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var color = row[x];
                        if (color.A == 0)
                            continue;
                        row[x] = ColorHelper.MapToPallete(color, pallete);
                    }
                }
            });
            return texture;
        }
        public static List<OutfitAccessoryLocation> GetLocationsForConfig(List<OutfitAccessoryLocation> locations, OutfitConfiguration config,int seed)
        {
            var result = new List<OutfitAccessoryLocation>();
            var grouped = locations
                .Where(l => config.Accessories.Contains(l.Type))
                .GroupBy(l => l.Type);

            foreach (var group in grouped)
            {
                var available = group.ToList();
                if (available.Count > 0)
                {
                    result.Add(available[seed % available.Count]);
                }
            }
            return result;
        }
    }
}