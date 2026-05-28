using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class ContractButtonsAccessory : IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.BUTTONS;
        public string Texture { get; set; }
        public string OuterTexture { get; set; }
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(1, 1);
        public bool IsReadyForColor { get; set; } = true;
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; } = true;
        public bool IsForOuterLayer { get; set; } = true;
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();

        public ContractButtonsAccessory()
        {
        }
    }
}
