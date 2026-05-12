using System.Drawing;

namespace minecreator.api.Model
{
    public class OutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; }
        public string Texture { get; set; }
        public Point Size { get; set; }
        public bool IsReadyForColor { get; set; }
    }
}
