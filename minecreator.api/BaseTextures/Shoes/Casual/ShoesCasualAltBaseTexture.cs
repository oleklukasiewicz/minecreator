using minecreator.api.Model;

namespace minecreator.api.BaseTextures.Shoes.Casual
{
    public class ShoesCasualAltBaseTexture : IBaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }

        public ShoesCasualAltBaseTexture()
        {

            Model = OutfitModel.BOTH;
            Type = OutfitType.SHOES;
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAABJQTFRFAAAAYGBgcHBwgICAkJCQoKCgYeuiWAAAAAZ0Uk5TAP//////enng/gAAAKFJREFUWIXtlkEOwyAMBLML/v+XI2PSJFwq25U41KOI44jFKOxxFEVRLIBASkBIUiDMCYBkhH3IQkjQP0tAQOgNIGmr/yDTEboyIvSYAE9IugU/mYJhQdwC2tYhMoYQnkK7LoJbUBRF8Q19LnICgf8P/RJQkoK/7gvPthB6bGdX0G/PY3u1hVkb3IKlLmyoPPf52yjcgtkURgINEdpBc0Q4AQeWBRHlg4JKAAAAAElFTkSuQmCC";
        }
    }
}
