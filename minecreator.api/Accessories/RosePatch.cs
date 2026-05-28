using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class RosePatch: IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.IMAGES;
        public string Texture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAICAYAAADaxo44AAAAAXNSR0IArs4c6QAAAIlJREFUCJlNiyEOwjAYRt+/bDUVcyPBoEpSTrAEvUtwEyyHwU8jplD4CkhIMBU1Sy1LKIaOPfe9vA9+OGuTszblLVn2MdLqmqAKmvcHGcy/WpXQx8jRexGAwdgUVMFzHAFodU0JEFTBqUtMQXM/PwQ8M91+l1ggWb42E1VTsb7B5epkGbE9mPn1Bb37K6YbCo50AAAAAElFTkSuQmCC";
        public string OuterTexture { get; set; }
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(6, 8);
        public bool IsReadyForColor { get; set; } = false;
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();

        public RosePatch()
        {
        }
    }
}
