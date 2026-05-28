using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class DuckPatch: IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.IMAGES;
        public string Texture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAECAYAAACtBE5DAAAAAXNSR0IArs4c6QAAAE9JREFUCJljYICCf8tL//873PQfxmdiYGBg+Dsl8r/DpCMMDPwcMHEGRriOw03/Gfg5GBj5VBn+P77MwAJX8uQzA8OTzwz/ZX4wMNnWMQIAddEXuvTnuogAAAAASUVORK5CYII=";
        public string OuterTexture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAECAYAAACtBE5DAAAAAXNSR0IArs4c6QAAACdJREFUCJljYKAI/LvU9f//g/X/GRgYGJhQZK6+ZmBUCGRkYGBgAAD8qgkc4Wz+OwAAAABJRU5ErkJggg==";
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(6, 4);
        public bool IsReadyForColor { get; set; } = false;
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();

        public DuckPatch()
        {
        }
    }
}
