using Microsoft.Extensions.FileSystemGlobbing.Internal;
using minecreator.api.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;

namespace minecreator.api.Helpers
{
    public static class ModuleHelper
    {
        public static TextureMapFullPart ProcessTexturePart(TextureMapFullPart source, OutfityTypeCharacteristics characteristics, TexturePattern pattern,
            Func<OutfityTypeCharacteristics, Image<Rgba32>, Image<Rgba32>, TexturePattern, TextureMapFullPart> processPart, bool dontApplypattern = false)
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
        public static async Task<Image<Rgba32>> Merge(List<Image<Rgba32>> images, OutfitModel type, bool flatten = false, List<TextureMapPart> excludedPartsFromFlatten = null)
        {
            if (excludedPartsFromFlatten == null)
                excludedPartsFromFlatten = new List<TextureMapPart> { TextureMapPart.HEAD };

            var modelMap = type == OutfitModel.CLASSIC ? ModelMaps.CLASSIC_MODEL : ModelMaps.SLIM_MODEL;

            //get size of first image
            var size = images.First().Size;
            //create new canvas
            var canvas = new Image<Rgba32>(size.Width, size.Height);
            //iterate parts
            for (var i = 0; images.Count > i; i++)
            {
                var image = images[i];
                for (var j = 0; modelMap.Count > j; j++)
                {
                    var part = modelMap.ElementAt(j);
                    var mergedpart = await ReplaceLowerPart(image, canvas, part.Value);

                    canvas = mergedpart;
                }
            }
            if (flatten)
            {
                //flatten image
                for (var i = 0; modelMap.Count > i; i++)
                {
                    var part = modelMap.ElementAt(i);
                    if (excludedPartsFromFlatten.Contains(part.Key))
                    {
                        continue;
                    }
                    canvas = await FlatPart(canvas, part.Value);
                }
            }


            return canvas;
        }
        private static async Task<Image<Rgba32>> ReplaceLowerPart(Image<Rgba32> image, Image<Rgba32> lowerLayer, ModelMapPartArea modelMap)
        {
            //get source image inner and outer parts
            var upperOuter = image.Clone();
            upperOuter.Mutate(x => x.Crop(modelMap.OuterArea));
            var upperInner = image.Clone();
            upperInner.Mutate(x => x.Crop(modelMap.Area));

            //merge inner parts
            lowerLayer.Mutate(x => x.DrawImage(upperInner, new Point(modelMap.Area.X, modelMap.Area.Y), 1));
            //iterate over pixels of outer parts
            for (var x = 0; modelMap.OuterArea.Width > x; x++)
            {
                for (var y = 0; modelMap.OuterArea.Height > y; y++)
                {
                    var upperInnerPixel = upperInner[x, y];

                    if (upperInnerPixel.R != 0 || upperInnerPixel.G != 0 || upperInnerPixel.B != 0)
                        lowerLayer[x + modelMap.OuterArea.X, y + modelMap.OuterArea.Y] = new Rgba32(0, 0, 0, 0);
                }
            }
            lowerLayer.Mutate(x => x.DrawImage(upperOuter, new Point(modelMap.OuterArea.X, modelMap.OuterArea.Y), 1));

            //paste lowerlayer into lowerImage
            // lowerLayer.Mutate(x => x.DrawImage(lowerLayerClone, new Point(modelMap.OuterTextureArea.X, modelMap.OuterTextureArea.Y), 1));

            return lowerLayer;

        }

        private static async Task<Image<Rgba32>> FlatPart(Image<Rgba32> image, ModelMapPartArea modelMap)
        {
            var outer = image.Clone();
            outer.Mutate(x => x.Crop(modelMap.OuterArea));
            image.Mutate(x => x.DrawImage(outer, new Point(modelMap.Area.X, modelMap.Area.Y), 1));
            //clear outer part iterate over pixels of outer parts
            for (var x = 0; modelMap.OuterArea.Width > x; x++)
            {
                for (var y = 0; modelMap.OuterArea.Height > y; y++)
                {
                    image[x + modelMap.OuterArea.X, y + modelMap.OuterArea.Y] = new Rgba32(0, 0, 0, 0);
                }
            }
            return image;

        }
    }
}
