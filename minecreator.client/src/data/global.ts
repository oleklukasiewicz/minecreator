import {
  derived,
  readonly,
  writable,
  type Readable,
  type Writable,
} from "svelte/store";
import { type Outfit } from "./outfit";
import { ExportModel } from "./models/export";
import { propertyStore } from "svelte-writable-derived";

const isMobileView: Writable<boolean> = writable(false);
export const IS_MOBILE_VIEW: Readable<boolean> = readonly(isMobileView);

export function Setup() {
  const viewMatcher = window.matchMedia("(max-width: 480px)");
  isMobileView.set(viewMatcher.matches);
  viewMatcher.addEventListener("change", (event) => {
    isMobileView.set(event.matches);
  });
}
export const currentExport: Writable<ExportModel> = writable(
  new ExportModel("classic", "modern", []),
);
export const currentOutfits: Writable<Outfit[]> = propertyStore(
  currentExport,
  "outfits",
);
export const currentSkinModel: Writable<string> = propertyStore(
  currentExport,
  "model",
);
export const currentVersion: Writable<string> = propertyStore(
  currentExport,
  "gameVersion",
);
export const currentGenerateSets: Writable<boolean> = propertyStore(
  currentExport,
  "generateSets",
);
export const debounce = function (
  callback: (...args: any[]) => void,
  timeout: number,
) {
  let timer: ReturnType<typeof setTimeout> | undefined;
  return (...args: any[]) => {
    if (timer !== undefined) clearTimeout(timer);
    timer = setTimeout(() => callback(...args), timeout);
  };
};

