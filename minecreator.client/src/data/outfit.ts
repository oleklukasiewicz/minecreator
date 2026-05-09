import { ValueData } from "$src/helpers/dataHelper";

const DEFAULT_SAMPLES = 4;

const enumToValueData = <T extends string>(entry: Record<string, T>) =>
  Object.entries(entry).map(
    ([key, value]) =>
      new ValueData(value, key.charAt(0) + key.slice(1).toLowerCase()),
  );

export enum OUTFIT_TYPE {
  TOP = "top",
  HOODIE = "hoodie",
  HAT = "hat",
  BOTTOM = "bottom",
  SHOES = "shoes",
  SUIT = "suit",
}
export const OUTFIT_TYPE_DATA = enumToValueData(OUTFIT_TYPE);
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
  SPORTS = "sport",
}
export const OUTFIT_STYLE_DATA = enumToValueData(OUTFIT_STYLE);
export enum OUTFIT_ACCESSORY {
  PATCHES = "patches",
  LOGO = "logo",
  PRINT = "print",
  PINS = "pins",
  BUTTONS = "buttons",
}
export const OUTFIT_ACCESSORY_DATA = enumToValueData(OUTFIT_ACCESSORY);

type OutfitLike = Partial<Outfit>;

const isEnumValue = <T extends string>(
  value: unknown,
  enumValues: Record<string, T>,
): value is T => Object.values(enumValues).includes(value as T);

const toStringArray = (value: unknown): string[] =>
  Array.isArray(value) ? value.filter((item): item is string => typeof item === "string") : [];

const toAccessories = (value: unknown): OUTFIT_ACCESSORY[] =>
  toStringArray(value).filter((item): item is OUTFIT_ACCESSORY =>
    isEnumValue(item, OUTFIT_ACCESSORY),
  );
export class Outfit {
  id: string;
  name: string;
  type: OUTFIT_TYPE;
  style: OUTFIT_STYLE;
  colors: string[];
  seed: string;
  accessories: OUTFIT_ACCESSORY[];
  samples: number = DEFAULT_SAMPLES;

  constructor(
    id: string = "",
    name: string = "",
    type: OUTFIT_TYPE = OUTFIT_TYPE.TOP,
    colors: string[] = [],
    seed: string = "",
    accessories: OUTFIT_ACCESSORY[] = [],
    style: OUTFIT_STYLE = OUTFIT_STYLE.CASUAL,
    samples: number = DEFAULT_SAMPLES,
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

export const createOutfit = (input: OutfitLike = {}): Outfit => {
  const outfit = new Outfit();

  outfit.id = typeof input.id === "string" ? input.id : "";
  outfit.name = typeof input.name === "string" ? input.name : "";
  outfit.type = isEnumValue(input.type, OUTFIT_TYPE)
    ? input.type
    : OUTFIT_TYPE.TOP;
  outfit.style = isEnumValue(input.style, OUTFIT_STYLE)
    ? input.style
    : OUTFIT_STYLE.CASUAL;
  outfit.colors = toStringArray(input.colors);
  outfit.seed = typeof input.seed === "string" ? input.seed : "";
  outfit.accessories = toAccessories(input.accessories);
  outfit.samples =
    typeof input.samples === "number" && Number.isFinite(input.samples)
      ? input.samples
      : DEFAULT_SAMPLES;

  return outfit;
};
