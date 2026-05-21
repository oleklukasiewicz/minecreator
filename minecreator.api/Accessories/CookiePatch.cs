using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class CookiePatch: OutfitAccessoryItem
    {
        public CookiePatch()
        {
            Type = OutfitAccessory.IMAGES;
            IsReadyForColor = false;
            Styles = new List<OutfitStyle>() { OutfitStyle.WINTER, OutfitStyle.CASUAL };
            Size = new System.Drawing.Point(6, 4);
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAAECAYAAACtBE5DAAAAAXNSR0IArs4c6QAAAF5JREFUCJljZGBgYJiWav6fAQoWnXrPcOLiLUbGaanm/xedes9gJ8cAB6IC3AxMD168Z4gzE2RQkBBkOPQIInjr2ScGRgYGBoYQM8n/grxcDAwMDAzvP39jWHPqOSMAInwbSXM0LVoAAAAASUVORK5CYII=";
        }
    }
}
