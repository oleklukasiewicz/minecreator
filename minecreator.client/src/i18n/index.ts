import { browser } from "$app/environment";
import {
  addMessages,
  getLocaleFromNavigator,
  init,
  locale as i18nLocale,
} from "svelte-i18n";
import en from "./locales/en.json";
import pl from "./locales/pl.json";

export const SUPPORTED_LOCALES = ["en", "pl"] as const;
export type SupportedLocale = (typeof SUPPORTED_LOCALES)[number];

const FALLBACK_LOCALE: SupportedLocale = "en";
const STORAGE_KEY = "minecreator.locale";
let initialized = false;

const resolveLocale = (value?: string | null): SupportedLocale => {
  const normalized = (value ?? "").toLowerCase().split("-")[0];
  return SUPPORTED_LOCALES.includes(normalized as SupportedLocale)
    ? (normalized as SupportedLocale)
    : FALLBACK_LOCALE;
};

export const initializeI18n = () => {
  if (initialized) return;

  addMessages("en", en);
  addMessages("pl", pl);

  const initialLocale = browser
    ? resolveLocale(
        window.localStorage.getItem(STORAGE_KEY) ?? getLocaleFromNavigator(),
      )
    : FALLBACK_LOCALE;

  init({
    fallbackLocale: FALLBACK_LOCALE,
    initialLocale,
  });

  initialized = true;
};

export const setAppLocale = (nextLocale: string) => {
  const resolved = resolveLocale(nextLocale);
  i18nLocale.set(resolved);
  if (browser) window.localStorage.setItem(STORAGE_KEY, resolved);
};

// Ensure locale is initialized for SSR before any component tries to format messages.
initializeI18n();
