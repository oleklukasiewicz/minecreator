import type { Outfit } from "./outfit";

export const ImportConfig = async function (): Promise<Outfit[]> {
  const input = document.createElement("input");
  input.type = "file";
  input.accept = ".json";
  input.multiple = false;
  input.click();

  const inputPromise = new Promise<any[]>((resolve, reject) => {
    input.onchange = (event) => {
      let files = (event.target as HTMLInputElement).files;
      if (files) {
        resolve(Array.from(files));
      } else {
        reject(new Error("No file selected"));
      }
    };
  });
  const files = await inputPromise;
  const file = files[0];
  const text = await file.text();
  const data = JSON.parse(text) as Outfit[];
  return data;
};
export const ImportConfigFromFile = async function (file: File): Promise<Outfit[]> {
  const text = await file.text();
  const data = JSON.parse(text) as Outfit[];
  return data;
};