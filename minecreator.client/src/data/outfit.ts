import { ValueData } from "$src/helpers/dataHelper";

const enumToValueData = <T extends string>(entry: Record<string, T>) =>
  Object.entries(entry).map(
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

export enum OUTFIT_TYPE {
  SUIT = "suit",
  SHOES = "shoes",
  BOTTOM = "bottom",
  HAT = "hat",
  TOP = "top",
  HOODIE = "hoodie",
}

type OutfitLike = Partial<Outfit>;

const isEnumValue = <T extends string>(
  value: unknown,
  enumValues: Record<string, T>,
): value is T => Object.values(enumValues).includes(value as T);

const toStringArray = (value: unknown): string[] =>
  Array.isArray(value) ? value.filter((item): item is string => typeof item === "string") : [];

const randomId = (): string => {
  const maybeCrypto = (globalThis as any).crypto;
  if (maybeCrypto && typeof maybeCrypto.randomUUID === "function") {
    return maybeCrypto.randomUUID();
  }
  return Date.now().toString(36) + Math.random().toString(36).slice(2, 8);
};

export class Outfit {
  id: string;
  name: string;
  type: string;
  style: string;
  colors: string[];
  seed: string;
  accessories: string[];
  samples: number = 1;
  preview?: string;

  constructor(
    id: string = "",
    name: string = "",
    type: string = "",
    colors: string[] = [],
    seed: string = "",
    accessories: string[] = [],
    style: string = "",
    samples: number = 1,
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
  ToExportModel(): any {
    return {
      name: this.name,
      type: this.type,
      style: this.style,
      colors: this.colors,
      seed: this.seed,
      accessories: this.accessories,
      samples: this.samples
    };
  }
}

export const createOutfit = (input: OutfitLike = {}): Outfit => {
  const outfit = new Outfit();

  outfit.id = typeof input.id === "string" && input.id !== "" ? input.id : randomId();
  outfit.name = typeof input.name === "string" ? input.name : "";
  outfit.type = input.type ?? "";
  outfit.style = input.style ?? "";
  outfit.colors = toStringArray(input.colors);
  outfit.seed = typeof input.seed === "string" ? input.seed : "";
  outfit.accessories = input.accessories ? toStringArray(input.accessories) : [];
  outfit.samples =
    typeof input.samples === "number" && Number.isFinite(input.samples)
      ? input.samples
      : 1;

  return outfit;
};
