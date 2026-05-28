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
  if (typeof window === "undefined") return;

  const viewMatcher = window.matchMedia("(max-width: 760px)");
  isMobileView.set(viewMatcher.matches);

  const handler = (event: any) => {
    const matches = event && typeof event.matches === "boolean" ? event.matches : viewMatcher.matches;
    isMobileView.set(matches);
  };

  if (typeof (viewMatcher as any).addEventListener === "function") {
    (viewMatcher as any).addEventListener("change", handler);
  } else if (typeof (viewMatcher as any).addListener === "function") {
    (viewMatcher as any).addListener(handler);
  }

  const resizeHandler = debounce(() => {
    isMobileView.set(viewMatcher.matches);
  }, 100);

  window.addEventListener("resize", resizeHandler);
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

