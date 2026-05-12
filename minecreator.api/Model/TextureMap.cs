using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace minecreator.api.Model
{
    public enum TextureMapPart
    {
        HEAD,
        BODY,
        LEFT_ARM,
        RIGHT_ARM,
        LEFT_LEG,
        RIGHT_LEG,
    }
    public class ModelMapPartArea
    {
        public TextureMapPart Part { get; set; }
        public Rectangle Area { get; set; }
        public Rectangle OuterArea { get; set; }
        public ModelMapPartArea(TextureMapPart part, Rectangle area, Rectangle outerArea)
        {
            Part = part;
            Area = area;
            OuterArea = outerArea;
        }
    }
    public static class ModelMaps
    {
        public static Dictionary<TextureMapPart, ModelMapPartArea> CLASSIC_MODEL = new Dictionary<TextureMapPart, ModelMapPartArea>()
        {
            {TextureMapPart.HEAD, new ModelMapPartArea(TextureMapPart.HEAD, new Rectangle(0, 0, 32, 16), new Rectangle(32, 0, 32, 16))},
            {TextureMapPart.BODY, new ModelMapPartArea(TextureMapPart.BODY, new Rectangle(16, 16, 24, 16), new Rectangle(16, 32, 24, 16))},
            {TextureMapPart.LEFT_ARM, new ModelMapPartArea(TextureMapPart.LEFT_ARM, new Rectangle(32, 48, 16, 16), new Rectangle(48, 48, 16, 16))},
            {TextureMapPart.RIGHT_ARM, new ModelMapPartArea(TextureMapPart.RIGHT_ARM, new Rectangle(40, 16, 16, 16), new Rectangle(40, 32, 16, 16))},
            {TextureMapPart.LEFT_LEG, new ModelMapPartArea(TextureMapPart.LEFT_LEG, new Rectangle(16, 48, 16, 16), new Rectangle(0, 48, 16, 16))},
            {TextureMapPart.RIGHT_LEG, new ModelMapPartArea(TextureMapPart.RIGHT_LEG, new Rectangle(0, 16, 16, 16), new Rectangle(0, 32, 16, 16))}
        };
        public static Dictionary<TextureMapPart, ModelMapPartArea> SLIM_MODEL = new Dictionary<TextureMapPart, ModelMapPartArea>()
        {
            {TextureMapPart.HEAD, CLASSIC_MODEL[TextureMapPart.HEAD] },
            {TextureMapPart.BODY, CLASSIC_MODEL[TextureMapPart.BODY] },
            {TextureMapPart.LEFT_ARM, new ModelMapPartArea(TextureMapPart.LEFT_ARM, new Rectangle(32, 48, 14, 16), new Rectangle(40, 48, 14, 16))},
            {TextureMapPart.RIGHT_ARM, new ModelMapPartArea(TextureMapPart.RIGHT_ARM, new Rectangle(40, 16, 14, 16), new Rectangle(40, 32, 14, 16))},
            {TextureMapPart.LEFT_LEG, CLASSIC_MODEL[TextureMapPart.LEFT_LEG] },
            {TextureMapPart.RIGHT_LEG, CLASSIC_MODEL[TextureMapPart.RIGHT_LEG] }
        };
    }
    public class TextureMapFullPart
    {
        public Image<Rgba32> Part { get; set; }
        public Image<Rgba32> OuterPart { get; set; }
    }
    public class TextureMap
    {
        private Image<Rgba32> _texture;
        private OutfitModel _model;
        public TextureMap()
        {
            _texture = new Image<Rgba32>(64, 64);
        }
        public TextureMap(int width, int height)
        {
            _texture = new Image<Rgba32>(width, height);
        }
        private Dictionary<TextureMapPart, ModelMapPartArea> GetModelMap()
        {
            return _model == OutfitModel.CLASSIC ? ModelMaps.CLASSIC_MODEL : ModelMaps.SLIM_MODEL;
        }

        public Image<Rgba32> GetPart(TextureMapPart part)
        {
            var model = GetModelMap();
            var area = model[part];
            var partImage = _texture.Clone();
            partImage.Mutate(x => x.Crop(area.Area));
            return partImage;
        }
        public Image<Rgba32> GetOuterPart(TextureMapPart part)
        {
            var model = GetModelMap();
            var area = model[part];
            var partImage = _texture.Clone();
            partImage.Mutate(x => x.Crop(area.OuterArea));
            return partImage;
        }
        public Image<Rgba32> GetPart(ModelMapPartArea area)
        {
            var partImage = _texture.Clone();
            partImage.Mutate(x => x.Crop(area.Area));
            return partImage;
        }
        public Image<Rgba32> GetOuterPart(ModelMapPartArea area)
        {
            var partImage = _texture.Clone();
            partImage.Mutate(x => x.Crop(area.OuterArea));
            return partImage;
        }
        public TextureMapFullPart GetFullPart(TextureMapPart part)
        {
            return new TextureMapFullPart
            {
                Part = GetPart(part),
                OuterPart = GetOuterPart(part)
            };
        }
        public void SetFullPart(TextureMapFullPart fullPart, TextureMapPart part)
        {
            SetPart(part, fullPart.Part);
            SetOuterPart(part, fullPart.OuterPart);
        }
        public void SetPart(TextureMapPart part, Image<Rgba32> image)
        {
            var model = GetModelMap();
            var area = model[part];
            _texture.Mutate(x => x.DrawImage(image, new Point(area.Area.X, area.Area.Y), new GraphicsOptions()
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.Src,
                BlendPercentage = 1f
            }));
        }
        public void SetOuterPart(TextureMapPart part, Image<Rgba32> image)
        {
            var model = GetModelMap();
            var area = model[part];
            _texture.Mutate(x => x.DrawImage(image, new Point(area.OuterArea.X, area.OuterArea.Y), new GraphicsOptions()
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.Src,
                BlendPercentage = 1f
            }));

        }
        public Image<Rgba32> Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        public OutfitModel Model
        {
            get { return _model; }
            set { _model = value; }

        }
        public TextureMap CopyParts(TextureMap source, List<TextureMapPart> parts)
        {
            foreach (var part in parts)
            {
                var innerPart = source.GetPart(part);
                if (innerPart != null)
                    this.SetPart(part, innerPart);
                var outerPart = source.GetOuterPart(part);
                if (outerPart != null)
                    this.SetOuterPart(part, outerPart);
            }
            return this;
        }
    }
}
