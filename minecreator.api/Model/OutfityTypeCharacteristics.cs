using Standart.Hash.xxHash;

namespace minecreator.api.Model
{
    public class OutfityTypeCharacteristics
    {
        public OutfityTypeCharacteristics(string seed, OutfitStyle style)
        {
            ExtractFromSeed(seed, style);
        }
        public ulong Hash { get; private set; }
        public int Length { get; private set; }
        public int Material { get; private set; }
        public int BaseDecoration { get; private set; }
        public int Details { get; private set; }

        public ulong GetNormalizedHash(string seed)
        {
            var _hash = xxHash3.ComputeHash(seed);
            Hash = _hash % ulong.MaxValue;
            return Hash;
        }
        public OutfityTypeCharacteristics ExtractFromSeed(string seed, OutfitStyle style)
        {
            GetNormalizedHash(seed);

            Length = (int)(Hash % 10);
            Material = (int)((Hash / 10) % 10);

            BaseDecoration = (int)((Hash / 100) % 10);
            Details = (int)((Hash / 1000) % 10);

            //style dependand characteristics
            switch (style)
            {

                case OutfitStyle.WINTER:
                    Length = Length % 2;
                    break;

                case OutfitStyle.SUMMER:
                    Length = 2 + (Length % 3);
                    break;
                case OutfitStyle.CASUAL:
                    Length = Length % 4;
                    break;
            }
            return this;
        }
    }
}
