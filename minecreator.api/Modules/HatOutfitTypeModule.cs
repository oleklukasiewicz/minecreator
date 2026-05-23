using minecreator.api.Model;

namespace minecreator.api.Modules
{
    public class HatOutfitTypeModule : OutfitModule
    {
        public override OutfitType OutfitType => OutfitType.HAT;
        public HatOutfitTypeModule()
        {
            Options.Styles = new List<OutfitStyle>() { OutfitStyle.CASUAL, OutfitStyle.SUMMER, OutfitStyle.WINTER };
        }

        public override TextureMap GenerateAccessories()
        {
            throw new NotImplementedException();
        }

        public override TextureMap GenerateAccessoryTexture()
        {
            throw new NotImplementedException();
        }

        public override TextureMap GenerateBaseTexture()
        {
            throw new NotImplementedException();
        }

        public override TextureMap GenerateColoredTexture()
        {
            throw new NotImplementedException();
        }

        public override TextureMap GenerateDetailsTexture()
        {
            throw new NotImplementedException();
        }
    }
}
