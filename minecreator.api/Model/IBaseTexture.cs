namespace minecreator.api.Model
{
    public interface IBaseTexture
    {
        OutfitType Type { get; set; }
        List<OutfitStyle> Styles { get; set; }
        OutfitModel Model { get; set; }
        string Texture { get; set; }
    }
}