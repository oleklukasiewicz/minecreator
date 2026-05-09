using Standart.Hash.xxHash;

namespace minecreator.api.Model
{
    public class OutfityTypeCharacteristics
    {
        public OutfityTypeCharacteristics(string seed)
        {
            ExtractFromSeed(seed);
        }
        public ulong Hash { get; private set; }
        public int Length { get; private set; }
        public int Material { get; private set; }
        public int BaseDecoration { get; private set; }

        public ulong GetNormalizedHash(string seed)
        {
            var _hash = xxHash3.ComputeHash(seed);
            Hash = _hash % ulong.MaxValue;
            return Hash;
        }
        public OutfityTypeCharacteristics ExtractFromSeed(string seed)
        {
            GetNormalizedHash(seed);

            Length = (int)(Hash % 100);
            Material = (int)((Hash / 100) % 100);
            BaseDecoration = (int)((Hash / 10000) % 100);

            return this;
        }
    }
}
