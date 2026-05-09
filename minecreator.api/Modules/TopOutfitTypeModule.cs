using minecreator.api.Helpers;
using minecreator.api.Model;
using minecreator.api.Model.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Modules
{
    public class TopOutfitTypeModule : IOutfitModule
    {
        private OutfitConfiguration _config;
        private TextureMap _texturemap;
        public TopOutfitTypeModule(OutfitConfiguration config)
        {
            SetConfiguration(config);
        }
        public TextureMap GenerateAccessoryTexture()
        {
            throw new NotImplementedException();
        }

        public TextureMap GenerateBaseTexture()
        {
            var textureMap = new TextureMap();

            var basetextureMap = BaseTextureHelper.LoadBaseTexture(_config.Style, _config.Type, _config.Model);
            var colors = ColorHelper.ExtractAndSortColorsByLuminance(basetextureMap.Texture);

            textureMap.CopyParts(basetextureMap, new List<TextureMapPart>
            { TextureMapPart.BODY, TextureMapPart.LEFT_ARM, TextureMapPart.RIGHT_ARM });

            var characteristics = new OutfityTypeCharacteristics(_config.Seed);
            //apply material characteristic for body
            var materialCharacteristic = (characteristics.Material / 10) % 3;
            if (materialCharacteristic > 0)
            {
                var bodyPart = textureMap.GetPart(TextureMapPart.BODY);
                var rightArm = textureMap.GetPart(TextureMapPart.RIGHT_ARM);
                var leftArm = textureMap.GetPart(TextureMapPart.LEFT_ARM);
                //flannel
                if (materialCharacteristic == 1)
                {
                    var tiledArea = PatternHelper.Flannel(new Point(24, 24), 2, new Point(0, 0), colors);

                    bodyPart = TextureManupulationHelper.DrawOnVisible(bodyPart, tiledArea);
                    textureMap.SetPart(TextureMapPart.BODY, bodyPart);

                    var tiledArea1 = PatternHelper.Flannel(new Point(24, 24), 2, new Point(1, 1), colors);

                    leftArm = TextureManupulationHelper.DrawOnVisible(leftArm, tiledArea1);
                    textureMap.SetPart(TextureMapPart.LEFT_ARM, leftArm);

                    rightArm = TextureManupulationHelper.DrawOnVisible(rightArm, tiledArea1);
                    textureMap.SetPart(TextureMapPart.RIGHT_ARM, rightArm);
                }
                if (materialCharacteristic == 2)
                {
                    var stripespattern = new List<int>();
                    var stripesColors = colors.Count - Math.Abs((int)characteristics.Hash % colors.Count);
                    if (stripesColors <= 1)
                        stripesColors = 2;
                    for (int i = 0; i < stripesColors; i++)
                    {
                        var value = Math.Abs(((int)characteristics.Hash % (10 * (i+1))) % 3);
                        stripespattern.Add((int)value);
                    }
                    var stripedArea = PatternHelper.Stripes(new Point(24, 24), new Point(0, 0), stripespattern, colors);

                    bodyPart = TextureManupulationHelper.DrawOnVisible(bodyPart, stripedArea);
                    textureMap.SetPart(TextureMapPart.BODY, bodyPart);
                    leftArm = TextureManupulationHelper.DrawOnVisible(leftArm, stripedArea);
                    textureMap.SetPart(TextureMapPart.LEFT_ARM, leftArm);
                    rightArm = TextureManupulationHelper.DrawOnVisible(rightArm, stripedArea);
                    textureMap.SetPart(TextureMapPart.RIGHT_ARM, rightArm);
                }
            }



            //apply length characteristic for front part of the body
            var tintCharacteristic = (characteristics.BaseDecoration / 10) % 3;
            var frontLengthCharacteristic = (characteristics.Length / 10) % 3;
            if (frontLengthCharacteristic > 0)
            {
                var bodypart = textureMap.GetPart(TextureMapPart.BODY);
                var bodyOuterpart = textureMap.GetOuterPart(TextureMapPart.BODY);

                var frontpart = bodypart.Clone();
                var frontOuterpart = bodyOuterpart.Clone();

                frontpart.Mutate(x => x.Crop(new Rectangle(4, 4, 8, 12)));
                frontOuterpart.Mutate(x => x.Crop(new Rectangle(4, 4, 8, 12)));

                var spreadBase = (frontLengthCharacteristic + 1) * 2;
                var centerPixelX = frontpart.Width / 2;

                frontpart.ProcessPixelRows(frontOuterpart, (frontAccessor, outerAccessor) =>
                {
                    for (int y = 0; y < frontAccessor.Height; y++)
                    {
                        var frontRow = frontAccessor.GetRowSpan(y);
                        var outerRow = outerAccessor.GetRowSpan(y);

                        var spread = (y == 0 && spreadBase == 4) ? 6 : spreadBase;
                        var startX = Math.Max(0, centerPixelX - spread / 2);
                        var endX = Math.Min(frontpart.Width, centerPixelX + spread / 2);

                        for (int x = startX; x < endX; x++)
                        {
                            if (x == startX || x == endX - 1)
                            {
                                var brighter = ColorHelper.GetBrighter(colors, frontRow[x]);

                                if (materialCharacteristic != 1)
                                {
                                    frontRow[x] = brighter;
                                    outerRow[x] = brighter;
                                }
                                else
                                {
                                    outerRow[x] = frontRow[x];
                                }
                            }
                            else
                            {
                                frontRow[x] = new Rgba32(0, 0, 0, 0);
                            }
                        }
                    }
                });

                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                bodypart.Mutate(x => x.DrawImage(frontpart, new Point(4, 4), gfxOptions));
                bodyOuterpart.Mutate(x => x.DrawImage(frontOuterpart, new Point(4, 4), gfxOptions));

                textureMap.SetPart(TextureMapPart.BODY, bodypart);
                textureMap.SetOuterPart(TextureMapPart.BODY, bodyOuterpart);
            }

            //apply length characteristic for arms
            var armsLengthCharacteristic = (characteristics.Length) % 5;
            if (armsLengthCharacteristic > 0)
            {
                var leftArmPart = textureMap.GetPart(TextureMapPart.LEFT_ARM);
                var leftSleeve = GenerateArmPart(leftArmPart, textureMap.GetOuterPart(TextureMapPart.LEFT_ARM), armsLengthCharacteristic, materialCharacteristic);

                textureMap.SetPart(TextureMapPart.LEFT_ARM, leftSleeve.Part);
                textureMap.SetOuterPart(TextureMapPart.LEFT_ARM, leftSleeve.OuterPart);

                var rightArmPart = textureMap.GetPart(TextureMapPart.RIGHT_ARM);
                var rightSleeve = GenerateArmPart(rightArmPart, textureMap.GetOuterPart(TextureMapPart.RIGHT_ARM), armsLengthCharacteristic, materialCharacteristic);

                textureMap.SetPart(TextureMapPart.RIGHT_ARM, rightSleeve.Part);
                textureMap.SetOuterPart(TextureMapPart.RIGHT_ARM, rightSleeve.OuterPart);
            }

            var imageFromtextureMap = textureMap.Texture.ToBase64String(PngFormat.Instance);

            _texturemap = textureMap;
            return textureMap;
        }
        private TextureMapFullPart GenerateArmPart(Image<Rgba32> armPart, Image<Rgba32> outerArmpart, int armCharacteristics, int materialCharacteristics)
        {
            var sleeve = armPart.Clone();
            var overallOffset = 0;
            for (int step = 1; step <= armCharacteristics; step++)
            {
                int height = (step == 2 || step == 3) ? 4 : 3;
                int yOffset = 11 - step + (3 - height);
                overallOffset = yOffset + Math.Abs(3 - height);

                sleeve = TextureManupulationHelper.MoveByVector(sleeve, new Rectangle(0, yOffset, 16, height), new Point(0, -1));

            }
            var fullpart = new TextureMapFullPart();
            fullpart.Part = sleeve;

            if (armCharacteristics > 1 && armCharacteristics < 5)
            {
                var materialFinish = sleeve.Clone();
                var imagetest = materialFinish.ToBase64String(PngFormat.Instance);
                var outerSleeve = outerArmpart.Clone();

                var isSlimmer = materialCharacteristics % 2;
                materialFinish.Mutate(x => x.Crop(new Rectangle(0, overallOffset + isSlimmer, 16, 2 - isSlimmer)));
                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                outerSleeve.Mutate(x => x.DrawImage(materialFinish, new Point(0, overallOffset + isSlimmer), gfxOptions));
                fullpart.OuterPart = outerSleeve;
            }
            else
            {
                fullpart.OuterPart = outerArmpart.Clone();
            }

            return fullpart;
        }
        public TextureMap GenerateColoredTexture()
        {
            var pallete = ColorHelper.GenerateDefaultPallete(_config.Colors[0]);
            //apply colors to the texturemap
            var texture = _texturemap.Texture.Clone();
            var colors = ColorHelper.ExtractAndSortColorsByLuminance(texture);
            //replace colors in the texture with the generated pallete
            texture.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        for (int x = 0; x < row.Length; x++)
                        {
                            var colorIndex = colors.IndexOf(row[x]);
                            if (colorIndex != -1)
                            {
                                row[x] = pallete[Math.Min(colorIndex, pallete.Count - 1)];
                            }
                        }
                    }
                });
            _texturemap.Texture = texture;
            return _texturemap;
        }

        public bool SetConfiguration(OutfitConfiguration config)
        {
            _config = config;
            return true;
        }
    }
}
