using minecreator.api.Model;

namespace minecreator.api.Modules
{
    public class ShoesOutfitTypeModule : OutfitModule
    {
        public override OutfitType OutfitType => OutfitType.SHOES;
        public ShoesOutfitTypeModule()
        {
            Options.Styles = new List<OutfitStyle>() { OutfitStyle.CASUAL, OutfitStyle.SUMMER, OutfitStyle.WINTER };
        }
        public override TextureMap GenerateBaseTexture()
        {
            Workspace.Texture.CopyParts(Workspace.BaseTexture, new List<TextureMapPart>() { TextureMapPart.RIGHT_LEG, TextureMapPart.LEFT_LEG });

            return Workspace.Texture;
        }

        public override TextureMap GenerateAccessories()
        {
            throw new NotImplementedException();
        }

        public override TextureMap GenerateAccessoryTexture()
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
