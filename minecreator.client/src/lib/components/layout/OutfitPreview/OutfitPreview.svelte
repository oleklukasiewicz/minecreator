<script lang="ts">
  import {
    OUTFIT_ACCESSORY_DATA,
    OUTFIT_STYLE_DATA,
    OUTFIT_TYPE_DATA,
    type Outfit,
  } from "$src/data/outfit";
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

  let {
    outfit = null,
    onUpdate,
  }: {
    outfit?: Outfit | null;
    onUpdate?: (value: Outfit) => void;
  } = $props();

  let colorPickerCaller = $state<HTMLElement | null>(null);
  let colorPickerOpened = $state(false);
  let colorPickerSelected = $state("#C6C6C6");

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
      <SectionTitle label="Name" />
      <TextBox
        value={outfit.name}
        placeholder="Outfit Name"
        clearable
        oninput={(value: string) =>
          onUpdate?.({ ...outfit, name: value } as Outfit)}
      />
      <div class="section">
        <div class="sub-section">
          <SectionTitle label="Type" />
          <Select
            items={OUTFIT_TYPE_DATA}
            bind:selectedItem={outfit.type}
            itemText="label"
            itemValue="value"
            placeholder="Select outfit type"
            onselect={(payload: { item: ValueData }) =>
              onUpdate?.({ ...outfit, type: payload.item.value } as Outfit)}
          />
        </div>
        <div class="sub-section">
          <SectionTitle label="Style" />
          <Select
            items={OUTFIT_STYLE_DATA}
            bind:selectedItem={outfit.style}
            itemText="label"
            itemValue="value"
            placeholder="Select outfit style"
            onselect={(payload: { item: ValueData }) =>
              onUpdate?.({ ...outfit, style: payload.item.value } as Outfit)}
          />
        </div>
      </div>
    </div>
    <div class="category">
      <SectionTitle label="Accessory" />
      <Select
        items={OUTFIT_ACCESSORY_DATA}
        bind:selectedItem={outfit.accessories}
        itemText="label"
        multiple
        itemValue="value"
        placeholder="Select outfit accessory"
        onselect={(payload: { item: ValueData[] }) =>
          onUpdate?.({
            ...outfit,
            accessories: [...payload.item.map((i) => i.value)],
          } as Outfit)}
      />
    </div>
    <div class="category">
      <SectionTitle label="Colors" />

      <div class="color-section">
        <div bind:this={colorPickerCaller} class="color-picker-caller">
          <Button
            icon={AddIcon}
            label="Add Color"
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
        <div class="colors">
          {#if outfit?.colors.length === 0}
            <span class="empty-colors">No colors added yet.</span>
          {:else}
            <div class="separator vertical"></div>
            {#each outfit?.colors as color}
              <!-- svelte-ignore a11y_missing_attribute -->
              <a class="color">
                <span style={`background:${color};`}></span>
                <Button
                  onclick={() => removeColorFromOutfit(color)}
                  icon={CancelIcon}
                  label="Remove"
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
      <SectionTitle label="Seed" />
      <TextBox
        value={outfit.seed}
        placeholder="Outfit Seed"
        clearable
        oninput={(value: string) =>
          onUpdate?.({ ...outfit, seed: value } as Outfit)}
      />
      <SectionTitle label="Samples" />
      <TextBox
        value={outfit.samples?.toString() ?? ""}
        placeholder="Number of samples to generate"
        clearable
        oninput={(value: string) =>
          onUpdate?.({
            ...outfit,
            samples: parseInt(value) || undefined,
          } as Outfit)}
      />
    </div>
  {:else}
    <span id="no-outfit">No outfit selected</span>
  {/if}
</div>

<style lang="scss">
  #outfit-preview {
    display: flex;
    gap: 8px;
    flex-direction: column;
    text-align: left;
    #no-outfit {
      color: var(--color-theme-D5);
      font-family: minecraft;
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
      margin: 16px 0;
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
      width: min(320px, calc(100vw - 32px));
      max-width: calc(100vw - 32px);
      max-height: min(560px, calc(100vh - 120px));
      overflow: auto;
    }
    .color-section {
      grid-template-columns: auto 1fr;
      display: grid;
      gap: 8px;
    }
    .colors {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;

      .empty-colors {
        font-family: minecraft-simple;
        font-size: var(--size-font-caption);
        color: var(--color-theme-D5);
        margin-top: 12px;
      }

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
