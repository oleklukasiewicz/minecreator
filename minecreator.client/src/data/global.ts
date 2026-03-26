import { readonly, writable, type Readable, type Writable } from "svelte/store";

const isMobileView: Writable<boolean> = writable(false);
export const IS_MOBILE_VIEW: Readable<boolean> = readonly(isMobileView);

export function Setup() {
  const viewMatcher = window.matchMedia("(max-width: 768px)");
  isMobileView.set(viewMatcher.matches);
  viewMatcher.addEventListener("change", (event) => {
    isMobileView.set(event.matches);
  });
}
