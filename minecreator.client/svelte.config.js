import adapter from "@sveltejs/adapter-node";
import { relative, sep } from "node:path";
import { vitePreprocess } from "@sveltejs/vite-plugin-svelte";

/** @type {import('@sveltejs/kit').Config} */
const config = {
  preprocess: vitePreprocess(),
  compilerOptions: {
    // defaults to rune mode for the project, execept for `node_modules`. Can be removed in svelte 6.
    runes: ({ filename }) => {
      const relativePath = relative(import.meta.dirname, filename);
      const pathSegments = relativePath.toLowerCase().split(sep);
      const isExternalLibrary = pathSegments.includes("node_modules");

      return isExternalLibrary ? undefined : true;
    },
  },
  kit: {
    adapter: adapter(),
    alias: {
      $lib: "src/lib",
      $src: "src",
      $public: "static",
      $data: "src/data",
      $icons: "src/icons",
    },
  },
};

export default config;
