using minecreator.api.Helpers;
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
            return Workspace.AccessoryTexture;
        }

        public override TextureMap GenerateAccessoryTexture()
        {
            return Workspace.AccessoryTexture;
        }

        public override TextureMap GenerateBaseTexture()
        {
            Workspace.Texture.CopyParts(Workspace.BaseTexture, new List<TextureMapPart>() { TextureMapPart.HEAD });
            return Workspace.BaseTexture;
        }

        public override TextureMap GenerateColoredTexture()
        {
            return Workspace.Texture;
        }

        public override TextureMap GenerateDetailsTexture()
        {
            if (Configuration.Style == OutfitStyle.WINTER)
            {
                var texture = TextureManupulationHelper.CopyOnlyWithPallete(Workspace.Texture.Texture, new List<SixLabors.ImageSharp.PixelFormats.Rgba32>()
                {
                    ColorHelper.DEFAULT_PALLETE.Colors[ColorHelper.PalleteColorSize-1],
                    ColorHelper.DEFAULT_PALLETE.Colors[ColorHelper.PalleteColorSize-2],
                });
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.HEAD, texture);
            }
            else
            {
                var border = TextureManupulationHelper.DetectOutline(Workspace.Texture.Texture, false, true, false, false, 1);
                var img = Workspace.DetailsTexture.GetOuterPart(TextureMapPart.HEAD);
                TextureManupulationHelper.CopyRectangles(img, Workspace.Texture.Texture, border);
                Workspace.DetailsTexture.SetOuterPart(TextureMapPart.HEAD, img);
            }



            return Workspace.DetailsTexture;
        }
    }
}
