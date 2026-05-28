using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class MccPatch : IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.IMAGES;
        public string Texture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAECAYAAACtBE5DAAAAAXNSR0IArs4c6QAAAEdJREFUCJlj+P9n/n8GBgaG/3/m////0AjCfmj0nxHGgYM3bAwMDAwMLAw3vjB8vfoNRY5bm4uBpXC7F8OHDx9QJAQeCTAAANuCG3HLTGdhAAAAAElFTkSuQmCC";
        public string OuterTexture { get; set; }
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(6, 4);
        public bool IsReadyForColor { get; set; } = true;
        public bool UseBaseColor { get; set; } = true;
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>() { OutfitStyle.SUMMER, OutfitStyle.CASUAL };

        public MccPatch()
        {
        }
    }
}
