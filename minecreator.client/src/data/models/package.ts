export type OutfitLayerPart = {
  content?: string | null;
  contentSnapshot?: string | null;
};

export type OutfitLayer = {
  id: string;
  outfitType?: string | null;
  alex?: OutfitLayerPart | null;
  steve?: OutfitLayerPart | null;
};

export type OutfitPackage = {
  id?: string;
  model: string;
  outfitType?: string | null;
  layers: OutfitLayer[];
};
