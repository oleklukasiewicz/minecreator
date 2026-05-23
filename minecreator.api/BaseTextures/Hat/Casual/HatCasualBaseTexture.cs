using minecreator.api.Model;

namespace minecreator.api.BaseTextures.Hat.Casual
{
    public class HatCasualBaseTexture : BaseTexture
    {
        public HatCasualBaseTexture()
        {
            Model = OutfitModel.BOTH;
            Type = OutfitType.HAT;
            Styles = new List<OutfitStyle>() { OutfitStyle.CASUAL, OutfitStyle.SUMMER };
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAAA9QTFRFAAAAcHBwgICAkJCQoKCgDt5M4wAAAAV0Uk5TAP////8c0CZSAAAAYElEQVRYhe3RQQrAIAxEUc3P/c9ckrYgXVSadDkPVLLIoGaMC54YVQ4eqx5gBmatgNAIIPo7T4gEvHODHEI9oDzGuxGWY6n3Aefvg0Fuj3obYHPa2/r8JBEREREREfnPAdCRAcW540XqAAAAAElFTkSuQmCC";
        }
    }
}
