<script lang="ts">
  import { goto } from "$app/navigation";
  import OutfitPackageRender from "$lib/components/render/OutfitPackageRender.svelte";
  import { GenerateOutfits } from "$src/data/api";
  import { MODEL_TYPE } from "$src/data/enums/model";
  import {
    currentGenerateSets,
    currentOutfits,
    currentSkinModel,
    currentVersion,
  } from "$src/data/global";
  import type { ExportModel } from "$src/data/models/export";
  import { onMount } from "svelte";

  type GeneratedOutfit = {
    id?: string;
    name?: string;
    image: string;
  };

  let generatedOutfits = $state<GeneratedOutfit[]>([]);
  let selectedRenderIndex = $state(0);
  let isLoading = $state(true);
  let loadError = $state<string | null>(null);

  const selectedRender = $derived(
    generatedOutfits[selectedRenderIndex] ?? null,
  );

  const getRenderLabel = (outfit: GeneratedOutfit, index: number) => {
    if (outfit.name && outfit.name.trim().length > 0) return outfit.name;
    if (outfit.id && outfit.id.trim().length > 0) return `Render ${outfit.id}`;
    return `Render ${index + 1}`;
  };

  onMount(async () => {
    if ($currentOutfits.length == 0) {
      goto("/");
      return;
    }
    const json = {
      model: $currentSkinModel,
      generateSets: $currentGenerateSets,
      gameVersion: $currentVersion,
      outfits: $currentOutfits.map((o) => o.ToExportModel()),
    } as ExportModel;

    try {
      isLoading = true;
      loadError = null;

      const generated = await GenerateOutfits(json);
      generatedOutfits = (generated?.outfits ?? []) as GeneratedOutfit[];
      const generatedSets =
        generated?.sets ?? ([] as GeneratedOutfit[]);
      generatedOutfits = generatedOutfits.concat(generatedSets);
      selectedRenderIndex = 0;
    } catch (error) {
      loadError =
        error instanceof Error ? error.message : "Failed to generate outfits.";
      generatedOutfits = [];
      selectedRenderIndex = 0;
    } finally {
      isLoading = false;
    }
  });
</script>

<div id="generated-view">
  {#if isLoading}
    <p class="state">Generating outfits, please wait...</p>
  {:else if loadError}
    <p class="state error">{loadError}</p>
  {:else if generatedOutfits.length === 0}
    <p class="state">No generated renders found.</p>
  {:else}
    <div id="generated-layout">
      <aside id="generated-list">
        {#each generatedOutfits as outfit, index}
          <button
            type="button"
            class="render-list-item"
            class:selected={index === selectedRenderIndex}
            onclick={() => (selectedRenderIndex = index)}
          >
            <img
              src={"data:image/png;base64," + outfit.image}
              alt={getRenderLabel(outfit, index)}
            />
            <span>{getRenderLabel(outfit, index)}</span>
          </button>
        {/each}
      </aside>

      <section id="single-render-window">
        {#if selectedRender}
          <h2>{getRenderLabel(selectedRender, selectedRenderIndex)}</h2>
          <OutfitPackageRender
            source={"data:image/png;base64," + selectedRender.image}
            model={$currentSkinModel === "slim"
              ? MODEL_TYPE.ALEX
              : MODEL_TYPE.STEVE}
            isDynamic
          />
        {/if}
      </section>
    </div>
  {/if}
</div>

<style lang="scss">
  @use "style.scss";
</style>
