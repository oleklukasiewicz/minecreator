using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class CookiePatch: IOutfitAccessoryItem
    {
        public OutfitAccessory Type { get; set; } = OutfitAccessory.IMAGES;
        public string Texture { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAECAYAAACtBE5DAAAAAXNSR0IArs4c6QAAAF5JREFUCJljZGBgYJiWav6fAQoWnXrPcOLiLUbGaanm/xedes9gJ8cAB6IC3AxMD168Z4gzE2RQkBBkOPQIInjr2ScGRgYGBoYQM8n/grxcDAwMDAzvP39jWHPqOSMAInwbSXM0LVoAAAAASUVORK5CYII=";
        public string OuterTexture { get; set; }
        public System.Drawing.Point Size { get; set; } = new System.Drawing.Point(6, 4);
        public bool IsReadyForColor { get; set; } = false;
        public bool UseBaseColor { get; set; }
        public bool IsForGeneration { get; set; }
        public bool IsForOuterLayer { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>() { OutfitStyle.WINTER, OutfitStyle.CASUAL };

        public CookiePatch()
        {
        }
    }
}
