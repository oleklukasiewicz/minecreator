using minecreator.api.Helpers;
using minecreator.api.Model;
using minecreator.api.Model.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Reflection.PortableExecutable;

namespace minecreator.api.Modules
{
    public class TopOutfitTypeModule : IOutfitModule
    {
        private OutfitConfiguration _config;
        private TextureMap _texturemap;
        private TextureMap _accessoriestexturemap;
        private List<OutfitAccessoryLocation> _accessoriesLocations = new List<OutfitAccessoryLocation>();
        public TopOutfitTypeModule(OutfitConfiguration config)
        {
            SetConfiguration(config);
        }
        public bool SetConfiguration(OutfitConfiguration config)
        {
            _config = config;
            return true;
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
            Image<Rgba32> bodyPattern = null;
            Image<Rgba32> leftarmPattern = null;
            Image<Rgba32> rightarmPattern = null;
            if (materialCharacteristic > 0)
            {

                if (materialCharacteristic == 1) // flannel
                {
                    bodyPattern = PatternHelper.Flannel(new Point(24, 24), 2, new Point(0, 0), colors);
                    if (_config.Model == OutfitModel.SLIM)
                    {
                        leftarmPattern = PatternHelper.Flannel(new Point(24, 24), 2, new Point(0, 1), colors);
                        rightarmPattern = PatternHelper.Flannel(new Point(24, 24), 2, new Point(1, 1), colors);
                    }
                    else
                    {
                        leftarmPattern = PatternHelper.Flannel(new Point(24, 24), 2, new Point(1, 1), colors);
                        rightarmPattern = PatternHelper.Flannel(new Point(24, 24), 2, new Point(1, 1), colors);
                    }
                }
                else if (materialCharacteristic == 2) // stripes
                {
                    var stripespattern = new List<int>();
                    var stripesColors = Math.Max(2, colors.Count - Math.Abs((int)characteristics.Hash % colors.Count));
                    for (int i = 0; i < stripesColors; i++)
                    {
                        stripespattern.Add(Math.Abs((int)characteristics.Hash % (10 * (i + 1)) % 2) + 1);
                    }

                    bodyPattern = PatternHelper.Stripes(new Point(24, 24), new Point(0, 0), stripespattern, colors);
                    leftarmPattern = bodyPattern;
                    rightarmPattern = bodyPattern;
                }
            }



            var fronttexture = ProcessMainBody(textureMap, characteristics, bodyPattern);
            textureMap.SetFullPart(fronttexture, TextureMapPart.BODY);

            var leftArmPart = ProcessArmBody(textureMap, characteristics, leftarmPattern, TextureMapPart.LEFT_ARM);
            textureMap.SetFullPart(leftArmPart, TextureMapPart.LEFT_ARM);
            var rightArmPart = ProcessArmBody(textureMap, characteristics, rightarmPattern, TextureMapPart.RIGHT_ARM);
            textureMap.SetFullPart(rightArmPart, TextureMapPart.RIGHT_ARM);


            var imageFromtextureMap = textureMap.Texture.ToBase64String(PngFormat.Instance);

            _texturemap = textureMap;
            return textureMap;
        }


        public TextureMap GenerateAccessoryArea()
        {
            var characteristics = new OutfityTypeCharacteristics(_config.Seed);

            var bodyPart = _texturemap.GetPart(TextureMapPart.BODY).Clone();
            var outerBodyPart = _texturemap.GetOuterPart(TextureMapPart.BODY).Clone();
            var accessoriesColor = ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Accessories];

            var backArea = new Rectangle(17, 6, 6, 6);
            using (var backPart = bodyPart.Clone(x => x.Crop(backArea)))
            {
                var filledBack = TextureManupulationHelper.FillWithAltPallete(
                    backPart,
                    new Rectangle(0, 0, backPart.Width, backPart.Height),
                    ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base],
                    accessoriesColor);

                bodyPart.Mutate(x => x.DrawImage(filledBack, backArea.Location, 1f));

                _accessoriesLocations.Add(new OutfitAccessoryLocation
                {
                    Location = backArea,
                    Type = OutfitAccessory.IMAGES
                });
            }

            var colors = ColorHelper.ExtractAndSortColorsByLuminance(bodyPart);
            var accessoryPallete = ColorHelper.GetColorsFromPallete(colors, accessoriesColor);
            var frontBorderPallete = ColorHelper.GetColorsFromPallete(colors, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.FrontOutline]);

            var frontLengthCharacteristic = (characteristics.Length / 10) % 4;
            if (frontLengthCharacteristic == 1)
                frontLengthCharacteristic = 0;

            if (frontLengthCharacteristic > 0)
            {
                var frontRect = new Rectangle(4, 0, 4, 16);
                using (var frontPart = bodyPart.Clone(x => x.Crop(frontRect)))
                {
                    var processedFront = TextureManupulationHelper.CopyOnlyWithPallete(frontPart, frontBorderPallete);

                    processedFront = TextureManupulationHelper.FillWithAltPallete(
                        processedFront,
                        new Rectangle(0, 0, frontRect.Width, frontRect.Height),
                        ColorHelper.GLOBAL_COLORS[TextureGlobalColor.FrontOutline],
                        accessoriesColor);

                    outerBodyPart.Mutate(x => x.DrawImage(processedFront, frontRect.Location, 1f));
                }
            }
            else
            {
                var frontRect = new Rectangle(5, 7, 6, 6);
                using (var frontPart = bodyPart.Clone(x => x.Crop(frontRect)))
                {
                    var filledFront = TextureManupulationHelper.FillWithAltPallete(
                        frontPart,
                        new Rectangle(0, 0, frontPart.Width, frontPart.Height),
                        ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base],
                        accessoriesColor);
                    bodyPart.Mutate(x => x.DrawImage(filledFront, frontRect.Location, 1f));

                    _accessoriesLocations.Add(new OutfitAccessoryLocation
                    {
                        Location = frontRect,
                        Type = OutfitAccessory.IMAGES
                    });
                }
            }

            var finalOuterBodyPart = TextureManupulationHelper.CopyOnlyWithPallete(outerBodyPart, accessoryPallete);
            var finalBodyPart = TextureManupulationHelper.CopyOnlyWithPallete(bodyPart, accessoryPallete);

            _accessoriestexturemap = new TextureMap
            {
                Texture = new Image<Rgba32>(_texturemap.Texture.Width, _texturemap.Texture.Height),
            };
            _accessoriestexturemap.SetPart(TextureMapPart.BODY, finalBodyPart);
            _accessoriestexturemap.SetOuterPart(TextureMapPart.BODY, finalOuterBodyPart);
            return _accessoriestexturemap;
        }
        public TextureMap GenerateAccessoryTexture()
        {
            var characteristics = new OutfityTypeCharacteristics(_config.Seed);
            
            var location = _accessoriesLocations.Last();

            var acc = AccessoriesHelper.GetAccessoriesForLocation(location);
            var test = AccessoriesHelper.ApplyConfigurationToAccessory(acc[0], _config);

            var frontpart = _texturemap.GetPart(TextureMapPart.BODY).Clone();
            var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver };
            frontpart.Mutate(x => x.DrawImage(test,new Point(location.Location.X,location.Location.Y), gfxOptions));
            _texturemap.SetPart(TextureMapPart.BODY, frontpart);
            return _texturemap;
        }
        public TextureMap GenerateColoredTexture()
        {
            var characteristics = new OutfityTypeCharacteristics(_config.Seed);
            var pallets = new List<List<Rgba32>>();
            foreach (var color in _config.Colors)
            {
                pallets.Add(ColorHelper.GenerateDefaultPallete(color));
            }
            var maxColors = pallets.Max(p => p.Count);

            //apply colors to the texturemap
            var texture = _texturemap.Texture.Clone();
            var colors = ColorHelper.ExtractAndSortColorsByLuminance(texture);
            var expandedpallete = ColorHelper.ExpandToGlobalPallets(colors, maxColors);
            //replace colors in the texture with the generated pallete
            texture.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var currentColor = row[x];
                        if (currentColor.A == 0)
                            continue;
                        var maxColors = characteristics.Material % pallets.Count + 1;
                        var colorIndex = expandedpallete[TextureGlobalColor.Base].Colors.IndexOf(currentColor);
                        if (colorIndex > -1)
                        {
                            var pallete = pallets[0];
                            row[x] = pallete[Math.Min(colorIndex, pallete.Count - 1)];
                            continue;
                        }

                        colorIndex = expandedpallete[TextureGlobalColor.FrontOutline].Colors.IndexOf(currentColor);
                        if (colorIndex > -1)
                        {
                            var palleteIndex = pallets.Count > 1 ? 1 : 0;
                            if (maxColors < 1)
                                palleteIndex = maxColors;
                            var pallete = pallets[palleteIndex];
                            row[x] = pallete[Math.Min(colorIndex, pallete.Count - 1)];
                            continue;
                        }

                        colorIndex = expandedpallete[TextureGlobalColor.BottomOutline].Colors.IndexOf(currentColor);

                        if (colorIndex > -1)
                        {
                            var palleteIndex = pallets.Count > 2 ? 2 : 1;
                            if (maxColors < 2)
                                palleteIndex = 0;
                            var pallete = pallets[palleteIndex];
                            row[x] = pallete[Math.Min(colorIndex, pallete.Count - 1)];
                            continue;
                        }
                    }
                }
            });
            _texturemap.Texture = texture;
            return _texturemap;
        }
        private TextureMapFullPart ProcessMainBody(TextureMap source, OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern)
        {
            var processedpart = ModuleHelper.ProcessTexturePart(source.GetFullPart(TextureMapPart.BODY), characteristics, pattern, ProcessMainBodyPart, ProcessMainBodyOuterPart);

            source.SetFullPart(processedpart, TextureMapPart.BODY);
            return processedpart;
        }
        private TextureMapFullPart ProcessArmBody(TextureMap source, OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, TextureMapPart part)
        {
            var processedpart = ModuleHelper.ProcessTexturePart(source.GetFullPart(part), characteristics, pattern, ProcessArmBodyPart, ProcessArmBodyOuterPart);
            source.SetFullPart(processedpart, part);
            return processedpart;
        }
        private Image<Rgba32> ProcessMainBodyPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, Image<Rgba32> part)
        {
            if (pattern != null)
            {
                part = TextureManupulationHelper.DrawOnVisible(part, pattern);
            }
            var colors = ColorHelper.ExtractAndSortColorsByLuminance(part);
            var bottomOutlines = TextureManupulationHelper.DetectOutline(part, false, true, false, false, 1);

            var frontLengthCharacteristic = (characteristics.Length / 10) % 4;
            if (frontLengthCharacteristic == 1)
                frontLengthCharacteristic = 0;

            var spreadBase = (frontLengthCharacteristic) * 2;
            var exludedPoints = new List<Point>();
            if (frontLengthCharacteristic > 0)
            {
                exludedPoints.Add(new Point(8 - spreadBase / 2, 15));
                exludedPoints.Add(new Point(7 + spreadBase / 2, 15));
            }
            foreach (var b in bottomOutlines)
            {
                //remove inner corner from outline
                part = TextureManupulationHelper.FillWithAltPallete(part, b, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base], ColorHelper.GLOBAL_COLORS[TextureGlobalColor.BottomOutline], exludedPoints);
            }
            if (frontLengthCharacteristic > 0)
            {

                var frontpart = part.Clone();
                frontpart.Mutate(x => x.Crop(new Rectangle(4, 0, 8, 16)));

                var centerPixelX = frontpart.Width / 2;

                frontpart.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        var spread = (y <= 4 && spreadBase == 4) ? spreadBase + 2 : spreadBase;

                        var startX = Math.Max(0, centerPixelX - spread / 2);
                        var endX = Math.Min(frontpart.Width, centerPixelX + spread / 2);

                        for (int x = startX; x < endX; x++)
                        {
                            if (x == startX || x == endX - 1)
                            {

                                var color = row[x];
                                var brighter = ColorHelper.GetBrighter(colors, color);

                                if (((characteristics.Material / 10) % 3) != 1)
                                {
                                    row[x] = brighter;
                                }
                            }
                            else
                            {
                                row[x] = new Rgba32(0, 0, 0, 0);
                            }
                        }
                    }
                });
                var onlyfront = frontpart.Clone();
                onlyfront.Mutate(x => x.Crop(new Rectangle(0, 4, 8, 12)));

                var frontOulines = TextureManupulationHelper.DetectOutline(onlyfront, false, false, true, true, 1, true);
                foreach (var rect in frontOulines)
                {
                    var newRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height);
                    frontpart = TextureManupulationHelper.FillWithAltPallete(frontpart, newRect, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base], ColorHelper.GLOBAL_COLORS[TextureGlobalColor.FrontOutline]);
                }

                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                part.Mutate(x => x.DrawImage(frontpart, new Point(4, 0), gfxOptions));
            }
            return part;
        }
        private Image<Rgba32> ProcessMainBodyOuterPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, Image<Rgba32> outerPart, Image<Rgba32> innerPart)
        {

            var frontpart = outerPart.Clone();
            frontpart.Mutate(x => x.Crop(new Rectangle(4, 4, 8, 12)));

            var innerFrontPart = innerPart.Clone();
            innerFrontPart.Mutate(x => x.Crop(new Rectangle(4, 4, 8, 12)));

            var colors = ColorHelper.ExtractAndSortColorsByLuminance(innerPart);
            var frontBorderPallete = ColorHelper.GetColorsFromPallete(colors, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.FrontOutline]);

            frontpart = TextureManupulationHelper.CopyOnlyWithPallete(innerPart, frontBorderPallete);

            var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
            outerPart.Mutate(x => x.DrawImage(frontpart, new Point(0, 0), gfxOptions));
            return outerPart;
        }
        private Image<Rgba32> ProcessArmBodyPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, Image<Rgba32> part)
        {
            if (pattern != null)
            {
                part = TextureManupulationHelper.DrawOnVisible(part, pattern);
            }
            var overallOffset = 0;
            var armLength = characteristics.Length % 5;
            for (int step = 1; step <= armLength; step++)
            {
                int height = (step == 2 || step == 3) ? 4 : 3;
                int yOffset = 11 - step + (3 - height);
                overallOffset = yOffset + Math.Abs(3 - height);

                part = TextureManupulationHelper.MoveByVector(part, new Rectangle(0, yOffset, 16, height), new Point(0, -1));

            }
            var armOutline = TextureManupulationHelper.DetectOutline(part, false, true, false, false, 2 - (characteristics.Material % 2));
            foreach (var arm in armOutline)
            {
                part = TextureManupulationHelper.FillWithAltPallete(part, arm, ColorHelper.GLOBAL_COLORS[TextureGlobalColor.Base], ColorHelper.GLOBAL_COLORS[TextureGlobalColor.BottomOutline]);
            }
            return part;
        }
        public Image<Rgba32> ProcessArmBodyOuterPart(OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, Image<Rgba32> part, Image<Rgba32> innerpart)
        {
            var armLength = characteristics.Length % 5;
            if (armLength > 1)
            {
                var materialFinish = innerpart.Clone();
                var imagetest = materialFinish.ToBase64String(PngFormat.Instance);
                var isSlimmer = characteristics.Material % 2;
                materialFinish.Mutate(x => x.Crop(new Rectangle(0, 11 - armLength + isSlimmer, 16, 2 - isSlimmer)));
                var gfxOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };
                part.Mutate(x => x.DrawImage(materialFinish, new Point(0, 11 - armLength + isSlimmer), gfxOptions));
            }
            return part;
        }

    }
}
