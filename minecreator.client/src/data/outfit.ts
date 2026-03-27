import { ValueData } from "$src/helpers/dataHelper";

export enum OUTFIT_TYPE {
  TOP = "top",
  HOODIE = "hoodie",
  HAT = "hat",
  BOTTOM = "bottom",
  SHOES = "shoes",
  SUIT = "suit",
}
export const OUTFIT_TYPE_DATA = Array.from(Object.entries(OUTFIT_TYPE)).map(
  ([key, value]) =>
    new ValueData(value, key.charAt(0) + key.slice(1).toLowerCase()),
);
export const GAME_VERSION = [
  new ValueData("modern", "Modern"),
  new ValueData("beta", "Beta and Lower"),
];
export const SKIN_MODEL = [
  new ValueData("classic", "Classic"),
  new ValueData("slim", "Slim"),
];
export enum OUTFIT_STYLE {
  CASUAL = "casual",
  FORMAL = "formal",
  SPORTS = "sports",
}
export const OUTFIT_STYLE_DATA = Array.from(Object.entries(OUTFIT_STYLE)).map(
  ([key, value]) =>
    new ValueData(value, key.charAt(0) + key.slice(1).toLowerCase()),
);
export enum OUTFIT_ACCESSORY {
  GLASSES = "glasses",
  MASK = "mask",
}
export const OUTFIT_ACCESSORY_DATA = Array.from(
  Object.entries(OUTFIT_ACCESSORY),
).map(
  ([key, value]) =>
    new ValueData(value, key.charAt(0) + key.slice(1).toLowerCase()),
);
export class Outfit {
  id: string;
  name: string;
  type: OUTFIT_TYPE;
  style: OUTFIT_STYLE;
  colors: string[];
  seed: string;
  accessories: OUTFIT_ACCESSORY[];
  samples: number = 4;

  constructor(
    id: string = "",
    name: string = "",
    type: OUTFIT_TYPE = OUTFIT_TYPE.TOP,
    colors: string[] = [],
    seed: string = "",
    accessories: OUTFIT_ACCESSORY[] = [],
    style: OUTFIT_STYLE = OUTFIT_STYLE.CASUAL,
    samples: number = 4,
  ) {
    this.id = id;
    this.name = name;
    this.type = type;
    this.colors = colors;
    this.seed = seed;
    this.accessories = accessories;
    this.style = style;
    this.samples = samples;
  }
}
