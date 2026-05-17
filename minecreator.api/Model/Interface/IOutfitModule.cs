namespace minecreator.api.Model.Interface
{
    public interface IOutfitModule
    {
        public OutfitType OutfitType { get; }
        public OutfitConfiguration Configuration { get; set; }
        public OutfitModuleWorkspace Workspace { get; set; }
        public bool SetConfiguration(OutfitConfiguration config);
        public TextureMap GenerateBaseTexture();
        public TextureMap GenerateDetailsTexture();
        public TextureMap GenerateAccessoryTexture();
        public TextureMap GenerateAccessories();
        public TextureMap GenerateColoredTexture();
        public OutfitModuleOptions GetOptions();
        public TextureMap MergeTextures(bool details, bool accessories);
    }
}
