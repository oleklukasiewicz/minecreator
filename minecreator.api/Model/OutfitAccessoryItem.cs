using System.Drawing;

namespace minecreator.api.Model
{
    public class OutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; }
        public string Texture { get; set; }
        public string OuterTexture { get; set; }
        public Point Size { get; set; }
        public bool IsReadyForColor { get; set; }
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; } = false;
    }
}
