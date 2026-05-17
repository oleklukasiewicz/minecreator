using Microsoft.Extensions.FileSystemGlobbing.Internal;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace minecreator.api.Helpers
{
    public static class ModuleHelper
    {
        public static TextureMapFullPart ProcessTexturePart(TextureMapFullPart source, OutfityTypeCharacteristics characteristics, TexturePattern pattern,
            Func<OutfityTypeCharacteristics, Image<Rgba32>, Image<Rgba32>, TexturePattern?, TextureMapFullPart> processPart,bool dontApplypattern=false)
        {
            var innerpart = source.Part;
            var outerpart = source.OuterPart;
            if (pattern != null && !dontApplypattern)
                innerpart = PatternHelper.ApplyPattern(innerpart, pattern);

            var innerPartresult = processPart(characteristics, innerpart, outerpart, pattern);
            source.Part = innerPartresult.Part;
            source.OuterPart = outerpart;
            return source;
        }
    }
}
