using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class BaseButtonsAccessory : OutfitAccessoryItem
    {
        public BaseButtonsAccessory()
        {
            Type = OutfitAccessory.BUTTONS;
            IsReadyForColor = false;
            Size = new System.Drawing.Point(1, 1);
            IsForGeneration = true;
            IsForOuterLayer = true;
        }
    }
}
