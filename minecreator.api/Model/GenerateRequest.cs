using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Model
{
    public class GenerateRequest
    {
        public string Model { get; set; }
        public List<OutfitConfigurationModel> Outfits { get; set; }
    }
    public class OutfitConfigurationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Style { get; set; }
        public List<string> Colors { get; set; } = new List<string>();
        public string Seed { get; set; }
        public List<string> Accessories { get; set; } = new List<string>();
        public int Samples { get; set; }
        public OutfitConfiguration ToConfig(string model)
        {
            return new OutfitConfiguration
            {
                Id = this.Id,
                Name = this.Name,
                Type = Enum.Parse<OutfitType>(this.Type.ToUpper()),
                Style = Enum.Parse<OutfitStyle>(this.Style.ToUpper()),
                Colors = this.Colors.Select(c => Rgba32.ParseHex(c)).ToList(),
                Seed = this.Seed,
                Accessories = this.Accessories.Select(a => Enum.Parse<OutfitAccessory>(a.ToUpper())).ToList(),
                Samples = this.Samples,
                Model = Enum.Parse<OutfitModel>(model.ToUpper())
            };
        }
    }
}
