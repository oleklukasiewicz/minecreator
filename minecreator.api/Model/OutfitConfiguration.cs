using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Model
{
    public enum OutfitType
    {
        TOP,
        BOTTOM,
        SHOES,
        HAT,

    }
    public enum OutfitStyle
    {
        CASUAL,
        WINTER,
        FORMAL,
        SUMMER
    }
    public enum OutfitAccessory
    {
        GLASSES,
        PINS,
        BUTTONS,
        IMAGES,
    }
    public enum OutfitModel
    {
        CLASSIC,
        SLIM,
        BOTH
    }

    public class OutfitConfiguration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public OutfitType Type { get; set; }
        public OutfitStyle Style { get; set; }
        public List<Rgba32> Colors { get; set; } = new List<Rgba32>();
        public string Seed { get; set; }
        public List<OutfitAccessory> Accessories { get; set; } = new List<OutfitAccessory>();
        public int Samples { get; set; }
        public OutfitModel Model { get; set; }
    }
}
