using System.Reflection;
using minecreator.api.Accessories;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Helpers
{
    public class OutfitAccessoryLocation
    {
        public Rectangle Location { get; set; }
        public OutfitAccessory Type { get; set; }
    }
    public static class AccessoriesHelper
    {
        private static List<IOutfitAccessoryItem> _accessories;

        static AccessoriesHelper()
        {
            var accessoryType = typeof(IOutfitAccessoryItem);
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => accessoryType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != accessoryType);

            _accessories = types.Select(t => (IOutfitAccessoryItem)Activator.CreateInstance(t)).ToList();
        }
        public static List<IOutfitAccessoryItem> GetAccessoriesForLocation(OutfitAccessoryLocation location, OutfitStyle style)
        {
            return _accessories.Where(a => a.Type == location.Type && a.Size.X <= location.Location.Width && a.Size.Y <= location.Location.Height && (a.Styles.Count() == 0 || a.Styles.Contains(style))).ToList();
        }
        public static Image<Rgba32> LoadAccessory(IOutfitAccessoryItem accessory, bool useOuterTexture = false)
        {
            var textureData = useOuterTexture && !string.IsNullOrEmpty(accessory.OuterTexture) ? accessory.OuterTexture : accessory.Texture;
            var imgBytes = Convert.FromBase64String(textureData);
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
        public static List<OutfitAccessoryLocation> GetLocationsForConfig(List<OutfitAccessoryLocation> locations, OutfitConfiguration config, int seed)
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
        public static TextureMap ProcessAccessoriesForPart(TextureMapPart part, OutfitModuleWorkspace workspace, OutfitConfiguration configuration)
        {
            var appliedAccTextureMap = new TextureMap()
            {
                Texture = new Image<Rgba32>(workspace.Texture.Texture.Width, workspace.Texture.Texture.Height)
            };

            var accessoryCharacteristics = workspace.Characteristics.BaseDecoration;
            var locations = AccessoriesHelper.GetLocationsForConfig(workspace.AccessoryLocations, configuration, workspace.Characteristics.BaseDecoration);

            var bodypart = new Image<Rgba32>(workspace.Texture.GetPart(part).Width, workspace.Texture.GetPart(part).Height);
            var outerBodyPart = new Image<Rgba32>(workspace.Texture.GetOuterPart(part).Width, workspace.Texture.GetOuterPart(part).Height);

            var dominantColor = ColorHelper.GetDominant(workspace.Texture.Texture);
            var pallete = ColorHelper.GetPallete(dominantColor, workspace.UserPallets);
            var contrastColor = ColorHelper.GetContrast(pallete.BaseColor, workspace.UserPallets.Select(x => x.BaseColor).ToList());
            var contractPallete = ColorHelper.GetPallete(contrastColor, workspace.UserPallets);

            foreach (var location in locations)
            {
                var accessoryItems = AccessoriesHelper.GetAccessoriesForLocation(location, configuration.Style);
                if (accessoryItems.Count == 0) continue;

                var selectedAccessory = accessoryItems[accessoryCharacteristics % accessoryItems.Count];
                Image<Rgba32> accessoryTexture = null;

                if (selectedAccessory.IsForGeneration && selectedAccessory.Type == OutfitAccessory.BUTTONS)
                {
                    var buttonsSpacing = ((int)workspace.Characteristics.Hash % 3) + 2;
                    if (buttonsSpacing < 2)
                        buttonsSpacing = 2;
                    var buttonsTexture = new Image<Rgba32>(location.Location.Width, location.Location.Height);

                    for (int y = 1; y < buttonsTexture.Height; y += buttonsSpacing)
                    {
                        for (int x = 0; x < buttonsTexture.Width; x += buttonsSpacing)
                        {
                            buttonsTexture[x, y] = ColorHelper.DEFAULT_PALLETE.Colors[0];
                        }
                    }
                    accessoryTexture = buttonsTexture;
                }
                else if (!selectedAccessory.IsForGeneration)
                {
                    accessoryTexture = AccessoriesHelper.LoadAccessory(selectedAccessory);
                }

                if (accessoryTexture == null) continue;

                if (selectedAccessory.IsReadyForColor)
                {
                    var selectedPallete = selectedAccessory.UseBaseColor ? pallete : contractPallete;
                    accessoryTexture = AccessoriesHelper.PaintAccessory(accessoryTexture, selectedPallete);
                }

                int centeredX = location.Location.Location.X + (location.Location.Width - accessoryTexture.Width) / 2;
                var centeredPoint = new Point(centeredX, location.Location.Location.Y);

                var targetImage = selectedAccessory.IsForOuterLayer ? outerBodyPart : bodypart;
                targetImage.Mutate(ctx => ctx.DrawImage(accessoryTexture, centeredPoint, PixelColorBlendingMode.Normal, 1f));

                if (!selectedAccessory.IsForOuterLayer && selectedAccessory.OuterTexture?.Length > 0)
                {
                    var outerAccessoryTexture = AccessoriesHelper.LoadAccessory(selectedAccessory, true);
                    if (outerAccessoryTexture != null)
                    {
                        if (selectedAccessory.IsReadyForColor)
                        {
                            var selectedPallete = selectedAccessory.UseBaseColor ? pallete : contractPallete;
                            accessoryTexture = AccessoriesHelper.PaintAccessory(accessoryTexture, selectedPallete);
                        }

                        int outerCenteredX = location.Location.Location.X + (location.Location.Width - outerAccessoryTexture.Width) / 2;
                        var outerCenteredPoint = new Point(outerCenteredX, location.Location.Location.Y);

                        outerBodyPart.Mutate(ctx => ctx.DrawImage(outerAccessoryTexture, outerCenteredPoint, PixelColorBlendingMode.Normal, 1f));
                    }
                }
            }

            appliedAccTextureMap.SetPart(part, bodypart);
            appliedAccTextureMap.SetOuterPart(part, outerBodyPart);

            return appliedAccTextureMap;
        }
    }
}