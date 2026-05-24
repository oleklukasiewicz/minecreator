namespace minecreator.api.Model
{
    public class ModuleOutfitsResult
    {
        public string OutfitId { get; set; }
        public OutfitType Type { get; set; }
        public List<TextureMap> Samples { get; set; }= new List<TextureMap>();
    }
}
