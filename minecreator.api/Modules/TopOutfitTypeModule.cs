
using minecreator.api.Helpers;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Modules
{
    public class TopOutfitTypeModule : OutfitModule
    {
        public override OutfitType OutfitType => OutfitType.TOP;

        public TopOutfitTypeModule()
        {
            Options.Accessory = new List<OutfitAccessory> { OutfitAccessory.BUTTONS, OutfitAccessory.IMAGES };
            Options.Styles = new List<OutfitStyle> { OutfitStyle.CASUAL, OutfitStyle.SUMMER, OutfitStyle.WINTER };
        }
        public override TextureMap GenerateBaseTexture()
        {
            Workspace.Texture.CopyParts(Workspace.BaseTexture, new List<TextureMapPart>
            { TextureMapPart.BODY, TextureMapPart.LEFT_ARM, TextureMapPart.RIGHT_ARM });

            var materialCharacteristic = Workspace.Characteristics.Material % 9;


            TexturePattern bodyPattern = null;
            TexturePattern leftarmPattern = null;
            TexturePattern rightarmPattern = null;
            Point patternSize = new Point(24, 24);

            if (materialCharacteristic > 0)
            {

                if (materialCharacteristic == 1) // flannel
                {
                    bodyPattern = PatternHelper.Flannel(patternSize, 2, new Point(0, 0), ColorHelper.DEFAULT_PALLETE.Colors);
                    if (Configuration.Model == OutfitModel.SLIM)
                    {
                        leftarmPattern = PatternHelper.Flannel(patternSize, 2, new Point(0, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                        rightarmPattern = PatternHelper.Flannel(patternSize, 2, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                    }
                    else
                    {
                        leftarmPattern = PatternHelper.Flannel(patternSize, 2, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                        rightarmPattern = PatternHelper.Flannel(patternSize, 2, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                    }
                }
                else if (materialCharacteristic == 2) // stripes
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
                    bodyPattern = stripesPattern;

                    stripesPattern.Texture.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < 4; y++)
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

                    leftarmPattern = stripesPattern;
                    rightarmPattern = stripesPattern;
                }
                else if (materialCharacteristic == 3) //hawaii
                {
                    var colors = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                    .Take(Configuration.Colors.Count)
                    .ToList();
                    var hawaiiPattern = PatternHelper.Hawaii(patternSize, (int)Workspace.Characteristics.Hash, new Point(0, 0), colors);
                    bodyPattern = hawaiiPattern;
                    leftarmPattern = hawaiiPattern;
                    rightarmPattern = hawaiiPattern;
                }
                else if (materialCharacteristic == 4) //knit
                {
                    var colors = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                    .Take(Configuration.Colors.Count)
                    .ToList();
                    var knitPattern = PatternHelper.Knit(patternSize, new Point(0, 0), colors);
                    bodyPattern = knitPattern;
                    leftarmPattern = knitPattern;
                    rightarmPattern = knitPattern;
                }
                else if (materialCharacteristic == 5)//agryle
                {
                    var colors = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                                            .Take(Configuration.Colors.Count)
                                            .ToList();
                    var agrylePattern = PatternHelper.Argyle(patternSize, new Point(0, 0), colors);
                    bodyPattern = agrylePattern;
                    leftarmPattern = agrylePattern;
                    rightarmPattern = agrylePattern;

                }
                else if (materialCharacteristic == 6)//Herringbone
                {
                    var colors = ColorHelper.COLORS_PALLETE.Select(x => x.BaseColor)
                                            .Take(Configuration.Colors.Count)
                                            .ToList();
                    var herringbonePattern = PatternHelper.Herringbone(patternSize, new Point(0, 0), colors);
                    bodyPattern = herringbonePattern;
                    leftarmPattern = herringbonePattern;
                    rightarmPattern = herringbonePattern;
                }
                else if (materialCharacteristic == 7) // flannel alt
                {
                    bodyPattern = PatternHelper.Flannel(patternSize, 1, new Point(0, 0), ColorHelper.DEFAULT_PALLETE.Colors);
                    if (Configuration.Model == OutfitModel.SLIM)
                    {
                        leftarmPattern = PatternHelper.Flannel(patternSize, 1, new Point(0, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                        rightarmPattern = PatternHelper.Flannel(patternSize, 1, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                    }
                    else
                    {
                        leftarmPattern = PatternHelper.Flannel(patternSize, 1, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                        rightarmPattern = PatternHelper.Flannel(patternSize, 1, new Point(1, 1), ColorHelper.DEFAULT_PALLETE.Colors);
                    }
                }
                else if (materialCharacteristic == 8)
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
                    bodyPattern = stripesPattern;

                    stripesPattern.Texture.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < 4; y++)
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

                    leftarmPattern = stripesPattern;
                    rightarmPattern = stripesPattern;
                }
            }

            var fronttexture = ModuleHelper.ProcessTexturePart(Workspace.Texture.GetFullPart(TextureMapPart.BODY), Workspace.Characteristics, bodyPattern, ProcessMainBodyPart);
            Workspace.Texture.SetFullPart(fronttexture, TextureMapPart.BODY);

            var leftArmPart = ModuleHelper.ProcessTexturePart(Workspace.Texture.GetFullPart(TextureMapPart.LEFT_ARM), Workspace.Characteristics, leftarmPattern, ProcessArmBodyPart);
            Workspace.Texture.SetFullPart(leftArmPart, TextureMapPart.LEFT_ARM);
            var rightArmPart = ModuleHelper.ProcessTexturePart(Workspace.Texture.GetFullPart(TextureMapPart.RIGHT_ARM), Workspace.Characteristics, rightarmPattern, ProcessArmBodyPart);
            Workspace.Texture.SetFullPart(rightArmPart, TextureMapPart.RIGHT_ARM);

            return Workspace.Texture;
        }
        public override TextureMap GenerateDetailsTexture()
        {
            var detailsCharacterists = Workspace.Characteristics.Details;

            Workspace.DetailsTexture = new TextureMap
            {
                Texture = new Image<Rgba32>(Workspace.Texture.Texture.Width, Workspace.Texture.Texture.Height)
            };

            var bodyPart = Workspace.Texture.GetPart(TextureMapPart.BODY).Clone();
            using var targetBodyPart = new Image<Rgba32>(bodyPart.Width, bodyPart.Height);

            if (detailsCharacterists % 2 == 1)
            {
                var bottomOutline = TextureManupulationHelper.DetectOutline(bodyPart, false, true, false, false, 1, false);
                TextureManupulationHelper.CopyRectangles(targetBodyPart, bodyPart, bottomOutline);
            }

            if (detailsCharacterists % 3 == 1)
            {
                var frontRect = new Rectangle(4, 4, 8, 12);
                using var frontPart = bodyPart.Clone(x => x.Crop(frontRect));
                var frontOutline = TextureManupulationHelper.DetectOutline(frontPart, false, false, true, true, 1, true);

                using var tempFrontTarget = new Image<Rgba32>(frontRect.Width, frontRect.Height);
                TextureManupulationHelper.CopyRectangles(tempFrontTarget, frontPart, frontOutline);

                targetBodyPart.Mutate(x => x.DrawImage(tempFrontTarget, new Point(frontRect.X, frontRect.Y), 1f));
            }

            Workspace.DetailsTexture.SetOuterPart(TextureMapPart.BODY, targetBodyPart);

            if (detailsCharacterists % 2 == 1)
            {
                var leftArmOuterPart = Workspace.Texture.GetOuterPart(TextureMapPart.LEFT_ARM).Clone();
                var rightArmOuterPart = Workspace.Texture.GetOuterPart(TextureMapPart.RIGHT_ARM).Clone();

                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.LEFT_ARM, leftArmOuterPart);
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.RIGHT_ARM, rightArmOuterPart);
            }
            return Workspace.DetailsTexture;

        }
        public override TextureMap GenerateAccessories()
        {
            Workspace.AccessoryTexture = AccessoriesHelper.ProcessAccessoriesForPart(TextureMapPart.BODY, Workspace, Configuration);
            return Workspace.AccessoryTexture;
        }
        public override TextureMap GenerateAccessoryTexture()
        {
            Workspace.AccessoryTexture = new TextureMap
            {
                Texture = new Image<Rgba32>(Workspace.Texture.Texture.Width, Workspace.Texture.Texture.Height),
            };
            var bodypart = Workspace.Texture.GetPart(TextureMapPart.BODY).Clone();
            var backArea = new Rectangle(17, 6, 6, 8);

            var targetBodyPart = Workspace.AccessoryTexture.GetPart(TextureMapPart.BODY);

            targetBodyPart = TextureManupulationHelper.CopyRectangles(targetBodyPart, bodypart, new List<Rectangle> { backArea });
            Workspace.AccessoryLocations.Add(new OutfitAccessoryLocation
            {
                Location = backArea,
                Type = OutfitAccessory.IMAGES
            });

            var frontLengthCharacteristic = Workspace.Characteristics.Length;
            if (frontLengthCharacteristic == 1)
                frontLengthCharacteristic = 0;

            if (frontLengthCharacteristic > 0)
            {
                //front accessory area
                var frontRect = new Rectangle(4, 4, 8, 12);
                var frontArea = bodypart.Clone(x => x.Crop(frontRect));
                var targetFrontArea = targetBodyPart.Clone(x => x.Crop(frontRect));

                var frontOutline = TextureManupulationHelper.DetectOutline(frontArea, false, false, false, true, 1, true);
                var frontOutlinePart = TextureManupulationHelper.CopyRectangles(targetFrontArea, frontArea, frontOutline);
                targetBodyPart.Mutate(x => x.DrawImage(frontOutlinePart, frontRect.Location, 1f));
                foreach (var rect in frontOutline)
                {
                    if (rect.Width < 2 && rect.Height < 2) continue;
                    Workspace.AccessoryLocations.Add(new OutfitAccessoryLocation
                    {
                        Location = new Rectangle(frontRect.X + rect.X, frontRect.Y + rect.Y, rect.Width, rect.Height),
                        Type = OutfitAccessory.BUTTONS
                    });
                }
            }
            else
            {
                var frontArea = new Rectangle(5, 7, 6, 8);

                targetBodyPart = TextureManupulationHelper.CopyRectangles(targetBodyPart, bodypart, new List<Rectangle> { frontArea });
                Workspace.AccessoryLocations.Add(new OutfitAccessoryLocation
                {
                    Location = frontArea,
                    Type = OutfitAccessory.IMAGES
                });
            }
            Workspace.AccessoryTexture.SetPart(TextureMapPart.BODY, targetBodyPart);


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
        private TextureMapFullPart ProcessMainBodyPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> part, Image<Rgba32> outerPart, TexturePattern pattern)
        {
            var frontLengthCharacteristic = characteristics.Length;
            if (frontLengthCharacteristic == 1)
                frontLengthCharacteristic = 0;

            var spreadBase = (frontLengthCharacteristic) * 2;
            var exludedPoints = new List<Point>();
            if (frontLengthCharacteristic > 0)
            {
                exludedPoints.Add(new Point(8 - spreadBase / 2, 15));
                exludedPoints.Add(new Point(7 + spreadBase / 2, 15));
            }
            if (frontLengthCharacteristic > 0)
            {
                var frontpart = part.Clone();
                frontpart.Mutate(x => x.Crop(new Rectangle(4, 0, 8, 16)));
                var frontOuterPart = outerPart.Clone();
                frontOuterPart.Mutate(x => x.Crop(new Rectangle(4, 0, 8, 16)));
                var centerPixelX = frontpart.Width / 2;

                frontpart.ProcessPixelRows(frontOuterPart, (accessor, outerAccessor) =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        var outerRow = outerAccessor.GetRowSpan(y);

                        var spread = (y <= 4 && spreadBase == 4) ? spreadBase + 2 : spreadBase;
                        var startX = Math.Max(0, centerPixelX - spread / 2);
                        var endX = Math.Min(frontpart.Width, centerPixelX + spread / 2);

                        for (int x = startX; x < endX; x++)
                        {
                            if (x == startX || x == endX - 1)
                            {

                                if (((characteristics.Material / 10) % 3) != 1)
                                {
                                    var color = row[x];
                                    var pallete = ColorHelper.GetPallete(color);
                                    var brighter = ColorHelper.GetBrighter(pallete, color);
                                    row[x] = brighter;
                                    if (y >= 4)
                                        outerRow[x] = brighter;
                                }
                            }
                            else
                            {
                                row[x] = new Rgba32(0, 0, 0, 0);
                                outerRow[x] = new Rgba32(0, 0, 0, 0);
                            }
                        }
                    }
                });

                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                part.Mutate(x => x.DrawImage(frontpart, new Point(4, 0), gfxOptions));
                outerPart.Mutate(x => x.DrawImage(frontOuterPart, new Point(4, 0), gfxOptions));

            }

            return new TextureMapFullPart { Part = part, OuterPart = outerPart };
        }
        private TextureMapFullPart ProcessArmBodyPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> part, Image<Rgba32> outerPart, TexturePattern pattern)
        {
            var armLength = characteristics.Length;
            var images = new[] { part, outerPart };
            for (int step = 1; step <= armLength; step++)
            {
                int height = (step == 2 || step == 3) ? 4 : 3;
                int yOffset = 11 - step + (3 - height);

                foreach (var img in images)
                    TextureManupulationHelper.MoveByVector(img, new Rectangle(0, yOffset, 16, height), new Point(0, -1));
            }

            if (armLength > 1)
            {
                int isSlimmer = (int)(characteristics.Material % 2);
                var rect = new Rectangle(0, 11 - armLength + isSlimmer, 16, 2 - isSlimmer);

                outerPart.Mutate(x => x.DrawImage(part.Clone(ctx => ctx.Crop(rect)), rect.Location, 1f));
            }

            return new TextureMapFullPart { Part = part, OuterPart = outerPart };
        }
    }
}
