<script lang="ts">
  import { type Outfit } from "$src/data/outfit";
  import { ValueData } from "$src/helpers/dataHelper";
  import Button from "$lib/components/base/Button/Button.svelte";
  import ColorPicker from "$lib/components/base/ColorPicker/ColorPicker.svelte";
  import Flyout from "$lib/components/base/Flyout/Flyout.svelte";
  import SectionTitle from "$lib/components/base/SectionTitle/SectionTitle.svelte";
  import Select from "$lib/components/base/Select/Select.svelte";
  import TextBox from "$lib/components/base/TextBox/TextBox.svelte";
  import CancelIcon from "$icons/close.svg?raw";
  import AddIcon from "$icons/plus.svg?raw";
  import { IS_MOBILE_VIEW } from "$src/data/global";
  import { _ } from "svelte-i18n";
  import { Configuration } from "$src/data/config";

  let {
    configuration,
    outfit = null,
    onUpdate,
  }: {
    configuration: Configuration;
    outfit?: Outfit | null;
    onUpdate?: (value: Outfit) => void;
  } = $props();

  let colorPickerCaller = $state<HTMLElement | null>(null);
  let colorPickerOpened = $state(false);
  let colorPickerSelected = $state("#C6C6C6");

  const translatedOutfitTypes = $derived(
    configuration.modulesConfig.map(
      (m) =>
        new ValueData(
          m.name.toLowerCase(),
          $_(`options.outfitType.${m.name.toLowerCase()}`),
        ),
    ),
  );
  const translatedOutfitStyles = $derived(
    configuration.modulesConfig
      .filter((m) => m.name.toUpperCase() === outfit?.type.toUpperCase())[0]
      ?.styles.map(
        (style) =>
          new ValueData(
            style,
            $_(`options.outfitStyle.${style.toLowerCase()}`),
          ),
      ) || [],
  );
  const translatedAccessories = $derived(
    configuration.modulesConfig
      .filter((m) => m.name.toUpperCase() === outfit?.type.toUpperCase())[0]
      ?.accessory.map(
        (acc) =>
          new ValueData(
            acc,
            $_(`options.outfitAccessory.${acc.toLowerCase()}`),
          ),
      ),
  );
  const sampleOptions = $derived.by(() =>
    Array.from(
      { length: configuration?.appConfig.maxSamplesCount || 0 },
      (_, index) => ({
        label: String(index + 1),
        value: index + 1,
      }),
    ),
  );

  const normalizeHex = (value: string) => {
    const normalized = value.trim().toUpperCase();
    if (!/^#([A-F0-9]{3}|[A-F0-9]{6})$/.test(normalized)) return null;
    if (normalized.length === 4) {
      const [r, g, b] = normalized.slice(1).split("");
      return `#${r}${r}${g}${g}${b}${b}`;
    }
    return normalized;
  };

  const addColorToOutfit = (color: string) => {
    if (!outfit) return;

    const normalized = normalizeHex(color);
    if (!normalized) return;

    const currentColors = (outfit.colors ?? []).map(
      (c) => normalizeHex(c) ?? c,
    );
    if (currentColors.includes(normalized)) {
      colorPickerOpened = false;
      return;
    }

    onUpdate?.({
      ...outfit,
      colors: [...currentColors, normalized],
    } as Outfit);

    colorPickerOpened = false;
  };

  const removeColorFromOutfit = (colorToRemove: string) => {
    if (!outfit) return;
    const normalizedToRemove = normalizeHex(colorToRemove) ?? colorToRemove;
    onUpdate?.({
      ...outfit,
      colors: (outfit.colors ?? []).filter(
        (c) => (normalizeHex(c) ?? c) !== normalizedToRemove,
      ),
    } as Outfit);
  };
</script>

<div id="outfit-preview" class:mobile={$IS_MOBILE_VIEW}>
  {#if outfit}
    <div class="category">
      <SectionTitle label={$_("outfitPreview.name")} />
      <TextBox
        value={outfit.name}
        placeholder={$_("outfitPreview.namePlaceholder")}
        clearable
        oninput={(value: string) =>
          onUpdate?.({ ...outfit, name: value } as Outfit)}
      />
      <div class="section">
        <div class="sub-section">
          <SectionTitle label={$_("outfitPreview.type")} />
          <Select
            items={translatedOutfitTypes}
            selectedItem={outfit.type}
            itemText="label"
            itemValue="value"
            placeholder={$_("outfitPreview.typePlaceholder")}
            onselect={(payload: { item: ValueData }) =>
              onUpdate?.({ ...outfit, type: payload.item.value } as Outfit)}
          />
        </div>
        <div class="sub-section">
          <SectionTitle label={$_("outfitPreview.style")} />
          <Select
            disabled={!outfit.type}
            items={translatedOutfitStyles}
            selectedItem={outfit.style}
            itemText="label"
            itemValue="value"
            placeholder={$_("outfitPreview.stylePlaceholder")}
            onselect={(payload: { item: ValueData }) =>
              onUpdate?.({ ...outfit, style: payload.item.value } as Outfit)}
          />
        </div>
      </div>
      <SectionTitle label={$_("outfitPreview.colors")} />

      <div class="color-section">
        <div class="color-actions">
          <div bind:this={colorPickerCaller} class="color-picker-caller">
            <Button
              disabled={outfit?.colors.length >
                configuration.appConfig.maxColorCount - 1}
              icon={AddIcon}
              label={$_("outfitPreview.addColor")}
              size="medium"
              onclick={() => (colorPickerOpened = !colorPickerOpened)}
            />
            <Flyout
              bind:opened={colorPickerOpened}
              caller={colorPickerCaller}
              align="left"
              autoWidth={false}
            >
              <div id="color-picker-container">
                <ColorPicker
                  bind:selectedColor={colorPickerSelected}
                  onselect={addColorToOutfit}
                />
              </div></Flyout
            >
          </div>
          {#if outfit?.colors.length > configuration.appConfig.maxColorCount - 1}
            <span class="preview-note">{$_("outfitPreview.maxColors")}</span>
          {/if}
          {#if outfit?.colors.length === 0}
            <span class="preview-note">{$_("outfitPreview.noColors")}</span>
          {/if}
        </div>
        <div class="colors">
          {#if outfit?.colors.length > 0}
            {#each outfit?.colors as color}
              <!-- svelte-ignore a11y_missing_attribute -->
              <a class="color" title={color}>
                <span style={`background:${color};`}></span>
                <Button
                  onclick={() => removeColorFromOutfit(color)}
                  icon={CancelIcon}
                  label={$_("common.remove")}
                  size="medium"
                  type="quaternary"
                  onlyIcon
                />
              </a>
            {/each}
          {/if}
        </div>
      </div>
    </div>
    <div class="category">
      <SectionTitle label={$_("outfitPreview.accessory")} />
      <Select
        items={translatedAccessories}
        selectedItem={outfit.accessories}
        itemText="label"
        multiple
        itemValue="value"
        placeholder={$_("outfitPreview.accessoryPlaceholder")}
        onselect={(payload: { item: ValueData[] }) =>
          onUpdate?.({
            ...outfit,
            accessories: [...payload.item.map((i) => i.value)],
          } as Outfit)}
      />
    </div>
    <div class="category">
      <div class="section">
        <div class="sub-section">
          <SectionTitle label={$_("outfitPreview.seed")} />
          <TextBox
            value={outfit.seed}
            placeholder={$_("outfitPreview.seedPlaceholder")}
            clearable
            maxLength={32}
            oninput={(value: string) =>
              onUpdate?.({ ...outfit, seed: value } as Outfit)}
          />
        </div>
        <div class="sub-section">
          <SectionTitle label={$_("outfitPreview.samples")} />
          <Select
            items={sampleOptions}
            selectedItem={outfit.samples}
            itemText="label"
            itemValue="value"
            placeholder={$_("outfitPreview.samplesPlaceholder")}
            onselect={(payload: { item: { label: string; value: number } }) =>
              onUpdate?.({
                ...outfit,
                samples: payload.item.value,
              } as Outfit)}
          />
        </div>
      </div>
    </div>
  {:else}
    <span class="preview-note">{$_("outfitPreview.noOutfitSelected")}</span>
  {/if}
</div>

<style lang="scss">
  #outfit-preview {
    display: flex;
    width: 100%;
    gap: 8px;
    flex-direction: column;
    text-align: left;
    .preview-note {
      color: var(--color-theme-D5);
      font-family: minecraft-simple;
      font-size: var(--size-font-caption);
    }
    .section {
      display: flex;
      gap: 8px;
      & > * {
        flex: 1;
      }
    }
    .category {
      display: flex;
      gap: 8px;
      flex-direction: column;
      margin: 12px 0;
    }
    .sub-section {
      display: flex;
      gap: 8px;
      flex-direction: column;
    }
    & .color-picker-caller {
      position: relative;
      width: fit-content;
    }
    & #color-picker-container {
      padding: 8px;
      background-color: var(--color-theme);
      border: 2px solid var(--color-theme-D1);
      box-sizing: border-box;
      width: min(360px, calc(100vw - 32px));
      max-width: calc(100vw - 32px);
      max-height: min(560px, calc(100vh - 120px));
      overflow: auto;
    }
    .color-actions {
      display: flex;
      gap: 8px;
      .preview-note {
        margin: 12px 0px;
      }
    }
    .colors {
      margin-top: 8px;
      display: flex;
      flex-wrap: wrap;
      gap: 8px;

      .color {
        min-width: 72px;
        height: 36px;
        display: inline-flex;
        box-sizing: border-box;
        background-color: var(--color-theme-D2);
        cursor: pointer;

        span {
          box-shadow: var(--shadow-button);
          aspect-ratio: 1;
          height: 100%;
        }
      }
    }
    &.mobile {
      & #color-picker-container {
        width: initial;
        max-width: initial;
        max-height: initial;
      }
    }
  }
</style>
