using minecreator.api.Model;

namespace minecreator.api.BaseTextures.Bottom.Casual
{
    public class BottomCasualBaseTexture : IBaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }

        public BottomCasualBaseTexture()
        {
            Model = OutfitModel.BOTH;
            Type = OutfitType.BOTTOM;
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAABJQTFRFAAAAYGBgcHBwgICAkJCQoKCgYeuiWAAAAAZ0Uk5TAP//////enng/gAAAFhJREFUWIXtlkEKwCAMBNVN//9lsb3kalcQdAaCpwzJXkwpAACJJjVLoAjtFVgrjOZc04IxvqLqe3+s8jbWxxSkmhYsysCYYL/AzgAA4HC4F/gryeDKe6EDOGsD7CEz0iIAAAAASUVORK5CYII=";
        }
    }
}
