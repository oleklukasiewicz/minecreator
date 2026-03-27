<script lang="ts">
  import SectionTitle from "$lib/components/SectionTitle/SectionTitle.svelte";
  import TextBox from "$lib/components/TextBox/TextBox.svelte";
  import {
    OUTFIT_ACCESSORY_DATA,
    OUTFIT_STYLE_DATA,
    OUTFIT_TYPE_DATA,
    type Outfit,
  } from "$src/data/outfit";
  import Select from "$lib/components/Select/Select.svelte";
  import { ValueData } from "$src/helpers/dataHelper";

  let {
    outfit = null,
    onUpdate,
  }: {
    outfit?: Outfit | null;
    onUpdate?: (value: Outfit) => void;
  } = $props();
</script>

<div id="outfit-preview">
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
  }
</style>
