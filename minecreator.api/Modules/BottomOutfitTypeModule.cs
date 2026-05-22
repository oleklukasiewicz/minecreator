using minecreator.api.Helpers;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Modules
{
    public class BottomOutfitTypeModule : OutfitModule
    {
        public override OutfitType OutfitType => OutfitType.BOTTOM;
        public BottomOutfitTypeModule()
        {
            Options.Accessory = new List<OutfitAccessory>() { };
            Options.Styles = new List<OutfitStyle>() { OutfitStyle.CASUAL, OutfitStyle.SUMMER, OutfitStyle.WINTER };
        }
        public override TextureMap GenerateBaseTexture()
        {
            Workspace.Texture.CopyParts(Workspace.BaseTexture, new List<TextureMapPart>() { TextureMapPart.RIGHT_LEG, TextureMapPart.LEFT_LEG });

            var patterndimensions = new Point(Workspace.Texture.GetPart(TextureMapPart.LEFT_LEG).Width, Workspace.Texture.GetPart(TextureMapPart.LEFT_LEG).Height);
            var materialCharacteristic = Workspace.Characteristics.Material % 5;

            TexturePattern leftlegpattern = null;
            TexturePattern rightlegpattern = null;
            if (materialCharacteristic > 0)
            {
                if (materialCharacteristic == 1) //stripes
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
                    var stripesPattern = PatternHelper.Stripes(patterndimensions, new Point(0, 0), stripespattern, stripes);
                    leftlegpattern = stripesPattern;
                    rightlegpattern = stripesPattern;

                }
                else if (materialCharacteristic == 2)//jean holes
                {
                    leftlegpattern = PatternHelper.JeansHoles(patterndimensions, (int)Workspace.Characteristics.Hash, new Point(0, 0), ColorHelper.COLORS_PALLETE[0]);
                    rightlegpattern = PatternHelper.JeansHoles(patterndimensions, (int)Workspace.Characteristics.Hash, new Point(16, 16), ColorHelper.COLORS_PALLETE[0]);
                }
                else if (materialCharacteristic == 3)//Herringbone
                {
                    var colors = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                                           .Take(Configuration.Colors.Count)
                                           .ToList();
                    var herringbonePattern = PatternHelper.Herringbone(patterndimensions, new Point(0, 0), colors);
                    leftlegpattern = herringbonePattern;
                    rightlegpattern = herringbonePattern;


                } else if (materialCharacteristic == 4) // flannel alt
                {
                    leftlegpattern = PatternHelper.Flannel(patterndimensions, 1, new Point(0, 0), ColorHelper.DEFAULT_PALLETE.Colors);
                    rightlegpattern = leftlegpattern;
                }
            }

            var leftLegTexture = ModuleHelper.ProcessTexturePart(Workspace.Texture.GetFullPart(TextureMapPart.LEFT_LEG), Workspace.Characteristics, leftlegpattern, ProcessLegPart, materialCharacteristic == 2);
            var rightLegTexture = ModuleHelper.ProcessTexturePart(Workspace.Texture.GetFullPart(TextureMapPart.RIGHT_LEG), Workspace.Characteristics, rightlegpattern, ProcessLegPart, materialCharacteristic == 2);

            Workspace.Texture.SetFullPart(leftLegTexture, TextureMapPart.LEFT_LEG);
            Workspace.Texture.SetFullPart(rightLegTexture, TextureMapPart.RIGHT_LEG);

            return Workspace.Texture;
        }
        private TextureMapFullPart ProcessLegPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> innerpart, Image<Rgba32> outerpart, TexturePattern pattern)
        {
            //length
            var lengthCharacteristic = Workspace.Characteristics.Length;

            TextureManupulationHelper.MoveByVector(innerpart, new Rectangle(0, 9, innerpart.Width, 4), new Point(0, lengthCharacteristic * -1));
            var leglastrow = 12 + lengthCharacteristic * -1;

            var materialCharacteristic = Workspace.Characteristics.Material % 3;
            if (materialCharacteristic == 2 && pattern != null)
            {
                var tempImage = new Image<Rgba32>(innerpart.Width, innerpart.Height);
                tempImage = TextureManupulationHelper.CopyRectangles(tempImage, innerpart, new List<Rectangle>() { new Rectangle(0, leglastrow, innerpart.Width, 1) });

                var appliedpattern = PatternHelper.ApplyPattern(innerpart, pattern);

                innerpart.Mutate(x => x.DrawImage(appliedpattern, new Point(0, 0), new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src }));
                innerpart.Mutate(x => x.DrawImage(tempImage, new Point(0, 0), new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver }));

                var outertexture = TextureManupulationHelper.CopyOnlyWithPallete(pattern.Texture, new List<Rgba32>() { ColorHelper.GetBrighter(ColorHelper.DEFAULT_PALLETE.Colors, ColorHelper.DEFAULT_PALLETE.BaseColor) });

                var innerOutylines = TextureManupulationHelper.CopyVisibleFromPattern(outerpart, innerpart, outertexture);

                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                outerpart.Mutate(x => x.DrawImage(innerOutylines, new Point(0, 0), gfxOptions));
            }


            return new TextureMapFullPart()
            {
                Part = innerpart,
                OuterPart = outerpart
            };
        }
        public override TextureMap GenerateAccessories()
        {
            return Workspace.AccessoryTexture;
        }

        public override TextureMap GenerateAccessoryTexture()
        {
            return Workspace.AccessoryTexture;
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
            var detailsCharacterists = Workspace.Characteristics.Details % 2;
            var lengthCharacteristic = Workspace.Characteristics.Length;
            Workspace.DetailsTexture = new TextureMap()
            {
                Texture = new Image<Rgba32>(Workspace.Texture.Texture.Width, Workspace.Texture.Texture.Height)
            };
            if (detailsCharacterists == 1)
            {

                var lefLegpart = Workspace.Texture.GetPart(TextureMapPart.LEFT_LEG).Clone();
                var leftlegOuterpart = Workspace.DetailsTexture.GetOuterPart(TextureMapPart.LEFT_LEG).Clone();
                var leftlegBorderRectangle = new Rectangle(0, 12 - lengthCharacteristic, leftlegOuterpart.Width, 1);
                TextureManupulationHelper.CopyRectangles(leftlegOuterpart, lefLegpart, new List<Rectangle> { leftlegBorderRectangle });
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.LEFT_LEG, leftlegOuterpart);

                var rightLegpart = Workspace.Texture.GetPart(TextureMapPart.RIGHT_LEG).Clone();
                var rightlegOuterpart = Workspace.DetailsTexture.GetOuterPart(TextureMapPart.RIGHT_LEG).Clone();
                var rightlegBorderRectangle = new Rectangle(0, 12 - lengthCharacteristic, rightlegOuterpart.Width, 1);
                TextureManupulationHelper.CopyRectangles(rightlegOuterpart, rightLegpart, new List<Rectangle> { rightlegBorderRectangle });
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.RIGHT_LEG, rightlegOuterpart);
            }
            return Workspace.DetailsTexture;
        }
    }
}
