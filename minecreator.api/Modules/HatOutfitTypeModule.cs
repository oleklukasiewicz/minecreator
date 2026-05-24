using minecreator.api.Helpers;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Modules
{
    public class HatOutfitTypeModule : OutfitModule
    {
        public override OutfitType OutfitType => OutfitType.HAT;
        public HatOutfitTypeModule()
        {
            Options.Styles = new List<OutfitStyle>() { OutfitStyle.CASUAL, OutfitStyle.SUMMER, OutfitStyle.WINTER };
        }

        public override TextureMap GenerateAccessories()
        {
            return Workspace.AccessoryTexture;
        }

        public override TextureMap GenerateAccessoryTexture()
        {
            return Workspace.AccessoryTexture;
        }

        public override TextureMap GenerateBaseTexture()
        {
            Workspace.Texture.CopyParts(Workspace.BaseTexture, new List<TextureMapPart>() { TextureMapPart.HEAD });
            var materialCharacteristic = Workspace.Characteristics.Material % 3;

            var headpart = Workspace.Texture.GetPart(TextureMapPart.HEAD);

            Point patternSize = new Point(24, 24);
            TexturePattern headpattern = null;
            materialCharacteristic = 2;
            if (materialCharacteristic == 1) // stripes
            {

                var stripes = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                .Take(Configuration.Colors.Count)
                .OrderBy(c =>
                {
                    int colorKey = c.R | (c.G << 8) | (c.B << 16);
                    return (Workspace.Characteristics.BaseDecoration ^ colorKey).GetHashCode();
                })
                .ToList();

                var stripespattern = new List<int>();
                int h = (int)Workspace.Characteristics.Hash;

                int maxPossibleColors = Math.Max(1, Configuration.Colors.Count);
                int stripesCount = 2 + (Math.Abs(h) % maxPossibleColors);

                for (int i = 0; i < stripesCount; i++)
                {
                    int width = (Math.Abs(h ^ (i * 137)) % 2) + 1;
                    stripespattern.Add(width);
                }
                if (stripes.Count > stripespattern.Count)
                {
                    stripespattern = stripespattern.Slice(0, stripespattern.Count).ToList();
                }
                var stripesPattern = PatternHelper.Stripes(patternSize, new Point(0, 0), stripespattern, stripes);
                headpattern = stripesPattern;

                stripesPattern.Texture.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < 8; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        for (int x = 0; x < row.Length; x++)
                        {
                            if (row[x].A == 0)
                                continue;
                            row[x] = ColorHelper.DEFAULT_PALLETE.BaseColor;
                        }
                    }
                });
                headpart = PatternHelper.ApplyPattern(headpart, stripesPattern);
                Workspace.Texture.SetPart(TextureMapPart.HEAD, headpart);
            }
            else if (materialCharacteristic == 2) // flannel
            {
                var stripes = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                    .Take(Configuration.Colors.Count)
                    .OrderBy(c =>
                    {
                        int colorKey = c.R | (c.G << 8) | (c.B << 16);
                        return (Workspace.Characteristics.BaseDecoration ^ colorKey).GetHashCode();
                    })
                    .ToList();

                var stripespattern = new List<int>();
                int h = (int)Workspace.Characteristics.Hash;

                int maxPossibleColors = Math.Max(1, Configuration.Colors.Count);
                int stripesCount = 2 + (Math.Abs(h) % maxPossibleColors);

                for (int i = 0; i < stripesCount; i++)
                {
                    int width = (Math.Abs(h ^ (i * 137)) % 2) + 1;
                    stripespattern.Add(width);
                }
                if (stripes.Count > stripespattern.Count)
                {
                    stripespattern = stripespattern.Slice(0, stripespattern.Count).ToList();
                }
                var stripesPattern = PatternHelper.VerticalStripes(patternSize, new Point(0, 0), stripespattern, stripes);
                headpattern = stripesPattern;
                headpart = PatternHelper.ApplyPattern(headpart, stripesPattern);

                var topheadPart = headpart.Clone();

                topheadPart = TextureManupulationHelper.CenteredStripes(topheadPart, new Rectangle(0, 8, headpart.Width, 1), new Rectangle(8, 0, 8, 8));

                Workspace.Texture.SetPart(TextureMapPart.HEAD, topheadPart);
            }
            return Workspace.BaseTexture;
        }

        public override TextureMap GenerateColoredTexture()
        {
            Workspace.Texture.Texture.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var color = row[x];
                        if (color.A == 0)
                            continue;
                        row[x] = ColorHelper.MapColor(color, Workspace.UserPallets);
                    }
                }
            });
            //paint on details texture
            var contrastColor = ColorHelper.GetContrast(Workspace.UserPallets[0].BaseColor, Workspace.UserPallets.Select(x => x.BaseColor).ToList());
            var contractPallete = ColorHelper.GetPallete(contrastColor, Workspace.UserPallets);
            Workspace.DetailsTexture.Texture.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var color = row[x];
                        if (color.A == 0)
                            continue;
                        row[x] = ColorHelper.MapToPallete(color, contractPallete);
                    }
                }
            });

            return Workspace.Texture;
        }

        public override TextureMap GenerateDetailsTexture()
        {
            //get last 2 colros from global pallets
            var colors = new List<Rgba32>();
            foreach (var pallete in ColorHelper.COLORS_PALLETE)
            {
                colors.Add(pallete.Colors.Last());
                colors.Add(pallete.Colors[ColorHelper.PalleteColorSize - 2]);
            }
            if (Configuration.Style == OutfitStyle.WINTER)
            {
                var texture = TextureManupulationHelper.CopyOnlyWithPallete(Workspace.Texture.Texture, colors);
                Workspace.DetailsTexture.SetPart(TextureMapPart.HEAD, texture);
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.HEAD, texture);
            }
            else
            {
                var border = TextureManupulationHelper.DetectOutline(Workspace.Texture.Texture, false, true, false, false, 1);
                var img = Workspace.DetailsTexture.GetOuterPart(TextureMapPart.HEAD);
                TextureManupulationHelper.CopyRectangles(img, Workspace.Texture.Texture, border);
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.HEAD, img);
            }



            return Workspace.DetailsTexture;
        }
    }
}
