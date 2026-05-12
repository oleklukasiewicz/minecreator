using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class ModuleHelper
    {
        public static TextureMapFullPart ProcessTexturePart(TextureMapFullPart source, OutfityTypeCharacteristics characteristics, Image<Rgba32> pattern, 
            Func<OutfityTypeCharacteristics, Image<Rgba32>, Image<Rgba32>, Image<Rgba32>> processPart, 
            Func<OutfityTypeCharacteristics, Image<Rgba32>, Image<Rgba32>, Image<Rgba32>, Image<Rgba32>> processOuterPart)
        {
            var innerpart = source.Part;
            var innerPartresult = processPart(characteristics, pattern, innerpart);
            source.Part = innerPartresult;

            var outerpart = source.OuterPart;
            var outerPartresult = processOuterPart(characteristics, pattern, outerpart,innerPartresult);
            source.OuterPart = outerPartresult;

            return source;
        }
    }
}
