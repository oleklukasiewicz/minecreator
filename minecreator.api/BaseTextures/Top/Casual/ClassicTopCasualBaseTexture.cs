using minecreator.api.Model;

namespace minecreator.api.Bases.Top.Casual
{
    public class ClassicTopCasualBaseTexture : IBaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }

        public ClassicTopCasualBaseTexture()
        {
            Model = OutfitModel.CLASSIC;
            Type = OutfitType.TOP;
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAAA9QTFRFAAAAYGBgcHBwgICAkJCQaKYofAAAAAV0Uk5TAP////8c0CZSAAAAvklEQVRYhe2W0Q7DIAhFlcv/f/Migt1MS7UuZks4Dw2x9QjGUFMKgiBwyCTkfhwnY+eCnPXRCbBNQJTS0xJyQQUay36gcIzhUkDymplZPm8xmE1BBNwIWKD3eJ+g1lg+t3prjLoJNuZmgIauVmbOlNALytRZgez8ZwZYzWDuHOBYjWp4OeFWoPGwwFJEd5SHBUEQBB5uDxwTOF14k2ChhNZktU9PC1pr11/bHwrsPmD3g/0ZfE+ApT1YOAe/zwtwdwXHIZhqrQAAAABJRU5ErkJggg==";
        }

    }
}
