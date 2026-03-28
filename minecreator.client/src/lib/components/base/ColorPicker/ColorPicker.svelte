<script lang="ts">
  import { onDestroy } from "svelte";
  import Button from "../Button/Button.svelte";
  import TextBox from "../TextBox/TextBox.svelte";

  let { selectedColor = $bindable(""), onselect = null } = $props();

  const DEFAULT_COLOR = "#C6C6C6";
  const SWATCHES = [
    "#C6C6C6",
    "#FFFFFF",
    "#000000",
    "#E79836",
    "#F8604C",
    "#CA31E9",
    "#3B8526",
    "#1DE495",
    "#3B6DD8",
    "#8B5A2B",
  ];

  let hue = $state(0);
  let saturation = $state(0);
  let lightness = $state(0);
  let hexInput = $state("");
  let paletteElement = $state<HTMLDivElement | null>(null);
  let initialized = false;

  const browserWindow = typeof window === "undefined" ? null : window;

  const clamp = (value: number, min: number, max: number) =>
    Math.min(max, Math.max(min, value));

  const sanitizeHexInput = (raw: string) => {
    const trimmed = raw.trim().replace(/[^a-fA-F0-9#]/g, "");
    const hasPrefix = trimmed.startsWith("#");
    const value = trimmed.replace(/#/g, "").slice(0, 6).toUpperCase();
    return `${hasPrefix ? "#" : ""}${value}`;
  };

  const normalizeHex = (raw: string | null | undefined) => {
    if (!raw) return null;

    const compact = raw.trim().replace(/^#/, "");
    if (!/^[a-fA-F0-9]{3}$|^[a-fA-F0-9]{6}$/.test(compact)) return null;

    if (compact.length === 3) {
      return `#${compact
        .split("")
        .map((c) => c + c)
        .join("")
        .toUpperCase()}`;
    }

    return `#${compact.toUpperCase()}`;
  };

  const hexToRgb = (hex: string) => {
    const normalized = normalizeHex(hex);
    if (!normalized) return null;

    const parsed = normalized.replace("#", "");
    const r = Number.parseInt(parsed.slice(0, 2), 16);
    const g = Number.parseInt(parsed.slice(2, 4), 16);
    const b = Number.parseInt(parsed.slice(4, 6), 16);
    return { r, g, b };
  };

  const rgbToHex = (r: number, g: number, b: number) =>
    `#${[r, g, b]
      .map((channel) =>
        clamp(Math.round(channel), 0, 255).toString(16).padStart(2, "0"),
      )
      .join("")
      .toUpperCase()}`;

  const rgbToHsl = (r: number, g: number, b: number) => {
    const rn = r / 255;
    const gn = g / 255;
    const bn = b / 255;

    const max = Math.max(rn, gn, bn);
    const min = Math.min(rn, gn, bn);
    const delta = max - min;

    let h = 0;
    const l = (max + min) / 2;
    let s = 0;

    if (delta !== 0) {
      s = delta / (1 - Math.abs(2 * l - 1));
      switch (max) {
        case rn:
          h = ((gn - bn) / delta) % 6;
          break;
        case gn:
          h = (bn - rn) / delta + 2;
          break;
        default:
          h = (rn - gn) / delta + 4;
      }
      h *= 60;
      if (h < 0) h += 360;
    }

    return {
      h,
      s: s * 100,
      l: l * 100,
    };
  };

  const hslToRgb = (h: number, s: number, l: number) => {
    const normalizedHue = ((h % 360) + 360) % 360;
    const normalizedSaturation = clamp(s, 0, 100) / 100;
    const normalizedLightness = clamp(l, 0, 100) / 100;

    const chroma =
      (1 - Math.abs(2 * normalizedLightness - 1)) * normalizedSaturation;
    const section = normalizedHue / 60;
    const secondary = chroma * (1 - Math.abs((section % 2) - 1));
    const match = normalizedLightness - chroma / 2;

    let r1 = 0;
    let g1 = 0;
    let b1 = 0;

    if (section >= 0 && section < 1) {
      r1 = chroma;
      g1 = secondary;
    } else if (section < 2) {
      r1 = secondary;
      g1 = chroma;
    } else if (section < 3) {
      g1 = chroma;
      b1 = secondary;
    } else if (section < 4) {
      g1 = secondary;
      b1 = chroma;
    } else if (section < 5) {
      r1 = secondary;
      b1 = chroma;
    } else {
      r1 = chroma;
      b1 = secondary;
    }

    return {
      r: Math.round((r1 + match) * 255),
      g: Math.round((g1 + match) * 255),
      b: Math.round((b1 + match) * 255),
    };
  };

  const hslToHex = (h: number, s: number, l: number) => {
    const rgb = hslToRgb(h, s, l);
    return rgbToHex(rgb.r, rgb.g, rgb.b);
  };

  const syncFromHsl = () => {
    const currentHex = hslToHex(hue, saturation, lightness);
    selectedColor = currentHex;
    hexInput = currentHex;
  };

  const setFromHex = (hex: string) => {
    const rgb = hexToRgb(hex);
    if (!rgb) return;

    const hsl = rgbToHsl(rgb.r, rgb.g, rgb.b);
    hue = Math.round(hsl.h);
    saturation = Math.round(hsl.s);
    lightness = Math.round(hsl.l);
    selectedColor = normalizeHex(hex) ?? DEFAULT_COLOR;
    hexInput = selectedColor;
  };

  const updateFromPalettePosition = (clientX: number, clientY: number) => {
    if (!paletteElement) return;

    const rect = paletteElement.getBoundingClientRect();
    if (rect.width === 0 || rect.height === 0) return;

    const x = clamp(clientX - rect.left, 0, rect.width);
    const y = clamp(clientY - rect.top, 0, rect.height);

    saturation = Math.round((x / rect.width) * 100);
    lightness = Math.round(100 - (y / rect.height) * 100);
    syncFromHsl();
  };

  const stopPaletteDrag = () => {
    browserWindow?.removeEventListener("mousemove", handlePaletteMouseMove);
    browserWindow?.removeEventListener("mouseup", stopPaletteDrag);
  };

  const handlePaletteMouseMove = (event: MouseEvent) => {
    updateFromPalettePosition(event.clientX, event.clientY);
  };

  const onPaletteMouseDown = (event: MouseEvent) => {
    updateFromPalettePosition(event.clientX, event.clientY);
    browserWindow?.addEventListener("mousemove", handlePaletteMouseMove);
    browserWindow?.addEventListener("mouseup", stopPaletteDrag);
  };

  const handleHueInput = (event: Event) => {
    const value = Number((event.target as HTMLInputElement).value);
    hue = clamp(value, 0, 360);
    syncFromHsl();
  };

  const handleInput = (value: string) => {
    hexInput = sanitizeHexInput(value);
    const normalized = normalizeHex(hexInput);
    if (normalized) setFromHex(normalized);
  };

  const applySwatchColor = (hex: string) => {
    setFromHex(hex);
  };

  onDestroy(() => {
    stopPaletteDrag();
  });

  $effect(() => {
    const normalized = normalizeHex(selectedColor);

    if (!initialized) {
      initialized = true;
      setFromHex(normalized ?? DEFAULT_COLOR);
      return;
    }

    if (!normalized) return;
    if (selectedColor !== normalized) setFromHex(normalized);
  });

  const isHexValid = $derived(normalizeHex(hexInput) !== null);
  const previewColor = $derived(hslToHex(hue, saturation, lightness));
  const selectionMarkerStyle = $derived(
    `left:${clamp(saturation, 0, 100)}%;top:${clamp(100 - lightness, 0, 100)}%;`,
  );
  const activeHueStyle = $derived(`--picker-hue:${hue};`);

  const emitSelectedColor = () => {
    onselect?.(selectedColor);
  };
</script>

<div class="color-picker">
  <!-- svelte-ignore a11y_no_static_element_interactions -->
  <div
    id="color-palette"
    style={activeHueStyle}
    bind:this={paletteElement}
    onmousedown={onPaletteMouseDown}
  >
    <div id="selection-marker" style={selectionMarkerStyle}></div>
  </div>

  <div id="hue-control">
    <input
      type="range"
      min="0"
      max="360"
      value={hue}
      oninput={handleHueInput}
      aria-label="Hue"
    />
  </div>
  <div id="swatches">
    {#each SWATCHES as swatch}
      <button
        type="button"
        class="swatch"
        class:selected={swatch === selectedColor}
        style={`background:${swatch};`}
        onclick={() => applySwatchColor(swatch)}
        aria-label={`Select ${swatch}`}
      ></button>
    {/each}
  </div>
  <div id="color-actions">
    <div id="preview-area">
      <div id="color-preview" style={`background:${previewColor};`}></div>
    </div>

    <div id="controls-area">
      <TextBox
        placeholder="Hex color"
        oninput={handleInput}
        bind:value={hexInput}
      />
      {#if !isHexValid}
        <span id="input-error">Invalid color (#RGB or #RRGGBB).</span>
      {/if}
      <Button
        label="Select"
        size="medium"
        disabled={!isHexValid}
        onclick={emitSelectedColor}
      />
    </div>
  </div>
</div>

<style lang="scss">
  @use "ColorPicker.scss";
</style>
