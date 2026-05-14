import { createOutfit, type Outfit } from "$data/outfit";

const parseOutfits = (value: any): Outfit[] => {
  const valueOutfits = value.outfits ?? value;
  const normalizedSource = Array.isArray(valueOutfits) ? valueOutfits : [valueOutfits];
  return normalizedSource
    .filter((item): item is Record<string, unknown> =>
      item !== null && typeof item === "object",
    )
    .map((item) => createOutfit(item as Partial<Outfit>));
};

const readOutfitsFromFile = async (file: File): Promise<Outfit[]> => {
  const text = await file.text();
  const parsed = JSON.parse(text) as unknown;
  return parseOutfits(parsed);
};

export const ImportConfig = async function (): Promise<Outfit[]> {
  const input = document.createElement("input");
  input.type = "file";
  input.accept = ".json";
  input.multiple = false;
  input.click();

  const inputPromise = new Promise<File[]>((resolve, reject) => {
    input.onchange = (event) => {
      const files = (event.target as HTMLInputElement).files;
      if (files) {
        resolve(Array.from(files));
      } else {
        reject(new Error("No file selected"));
      }
    };
  });
  const files = await inputPromise;
  const file = files[0];
  return readOutfitsFromFile(file);
};

export const ImportConfigFromFile = async function (
  file: File,
): Promise<Outfit[]> {
  return readOutfitsFromFile(file);
};