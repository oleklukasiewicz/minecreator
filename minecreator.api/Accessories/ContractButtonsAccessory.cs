using minecreator.api.Model;

namespace minecreator.api.Accessories
{
    public class ContractButtonsAccessory : OutfitAccessoryItem
    {
        public ContractButtonsAccessory()
        {
            Type = OutfitAccessory.BUTTONS;
            IsReadyForColor = true;
            Size = new System.Drawing.Point(1, 1);
            IsForGeneration = true;
            IsForOuterLayer = true;
        }
    }
}
