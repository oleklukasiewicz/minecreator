using System.Collections.Generic;
using System.Drawing;

namespace minecreator.api.Model
{
    public interface IOutfitAccessoryItem
    {
        OutfitAccessory Type { get; set; }
        string Texture { get; set; }
        string OuterTexture { get; set; }
        Point Size { get; set; }
        bool IsReadyForColor { get; set; }
        bool UseBaseColor { get; set; }
        bool IsForGeneration { get; set; }
        bool IsForOuterLayer { get; set; }
        List<OutfitStyle> Styles { get; set; }
    }
}