using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class CreeperPatch : IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.IMAGES;
        public string Texture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAGCAYAAADgzO9IAAAAAXNSR0IArs4c6QAAAD5JREFUCJl9jMENADEMwpxOkBEzQkZixGzQvqhO9ygvIxmiqjaApABwXwCZifNlbNoGuHObM4OkeF919/7zAdqTFsJ2FotbAAAAAElFTkSuQmCC";
        public string OuterTexture { get; set; }
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(6, 6);
        public bool IsReadyForColor { get; set; } = true;
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();

        public CreeperPatch()
        {
        }
    }
}
