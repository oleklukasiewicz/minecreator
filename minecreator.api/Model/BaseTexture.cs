namespace minecreator.api.Model
{
    public class BaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; }=new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }
    }
}
