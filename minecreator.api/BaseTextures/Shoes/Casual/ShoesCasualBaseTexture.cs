using minecreator.api.Model;

namespace minecreator.api.BaseTextures.Shoes.Casual
{
    public class ShoesCasualBaseTexture : IBaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }

        public ShoesCasualBaseTexture()
        {
            Model = OutfitModel.BOTH;
            Type = OutfitType.SHOES;
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAABJQTFRFAAAAYGBgcHBwgICAkJCQoKCgYeuiWAAAAAZ0Uk5TAP//////enng/gAAAFdJREFUWIXt1rEKADEIA9Cmmv//5aPHTS4HOkgxD4dOIbjUtUREApKsBQDdAe61gMlI/5CpRSK4sMEOUgF45zyQC9hm+QblJYqI/NG9MFv/b93fYOC98ABNqQMdMzhRCgAAAABJRU5ErkJggg==";
        }
    }
}
