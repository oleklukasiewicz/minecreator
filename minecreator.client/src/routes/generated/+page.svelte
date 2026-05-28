<script lang="ts">
  import { goto } from "$app/navigation";
  import Button from "$lib/components/base/Button/Button.svelte";
  import Dialog from "$lib/components/base/Dialog/Dialog.svelte";
  import Select from "$lib/components/base/Select/Select.svelte";
  import OutfitPackageRender from "$lib/components/render/OutfitPackageRender.svelte";
  import { ExportConfig } from "$data/export";
  import { GenerateOutfits } from "$src/data/api";
  import { MODEL_TYPE } from "$src/data/enums/model";
  import {
    IS_MOBILE_VIEW,
    currentGenerateSets,
    currentOutfits,
    currentSkinModel,
    currentVersion,
  } from "$src/data/global";
  import type { ExportModel } from "$src/data/models/export";
  import { ValueData } from "$src/helpers/dataHelper";
  import { SUPPORTED_LOCALES, setAppLocale } from "$src/i18n";
  import { CameraConfig } from "$src/data/render";
  import { THREE, Vector3Min } from "$lib/three";
  import JSZip from "jszip";
  import { _, t } from "svelte-i18n";
  import { locale } from "svelte-i18n";
  import { onMount, tick } from "svelte";
  import DownloadIcon from "$icons/download.svg?raw";
  import ExportIcon from "$icons/upload.svg?raw";
  import RadioButton from "$lib/components/base/RadioButton/RadioButton.svelte";
  import RadioGroup from "$lib/components/base/RadioGroup/RadioGroup.svelte";
  import Label from "$lib/components/base/Label/Label.svelte";

  type GeneratedItem = {
    id?: string;
    name?: string;
    image: string;
    outfitType?: string | null;
    type?: string | null;
    config: any;
  };

  type GeneratedTab = "outfits" | "sets";

  let generatedOutfits = $state<GeneratedItem[]>([]);
  let generatedSets = $state<GeneratedItem[]>([]);
  let selectedTab = $state<string>("outfits");
  let selectedOutfitIndex = $state(-1);
  let selectedSetIndex = $state(-1);
  let isLoading = $state(true);
  let loadError = $state<string | null>(null);
  let renderer = $state<any>(null);
  let renderDialogOpen = $state(false);

  const hasGeneratedSets = $derived(generatedSets.length > 0);
  const tabs = $derived([
    {
      key: "outfits" as const,
      label: $_("page.generatedOutfits"),
      count: generatedOutfits.length,
    },
    ...(hasGeneratedSets
      ? [
          {
            key: "sets" as const,
            label: $_("page.sets"),
            count: generatedSets.length,
          },
        ]
      : []),
  ]);

  const activeItems = $derived(
    selectedTab === "outfits" ? generatedOutfits : generatedSets,
  );
  const sortedActiveItems = $derived(
    [...activeItems].sort((left, right) => {
      const leftId = String(left.id ?? left.config?.id ?? "");
      const rightId = String(right.id ?? right.config?.id ?? "");
      return leftId.localeCompare(rightId, undefined, {
        numeric: true,
        sensitivity: "base",
      });
    }),
  );
  const selectedRenderIndex = $derived(
    selectedTab === "outfits" ? selectedOutfitIndex : selectedSetIndex,
  );
  const selectedRender = $derived(
    sortedActiveItems[selectedRenderIndex] ?? null,
  );
  const allGeneratedItems = $derived([...generatedOutfits, ...generatedSets]);

  const getRenderLabel = (outfit: GeneratedItem, index: number) => {
    if (outfit.name && outfit.name.trim().length > 0) return outfit.name;
    if (outfit.id && outfit.id.trim().length > 0) return `#${outfit.id}`;
    return `${$_("common.select")} ${index + 1}`;
  };

  const sanitizeFileName = (value: string) =>
    value
      .trim()
      .replace(/\s+/g, "_")
      .replace(/[^a-zA-Z0-9._-]+/g, "_")
      .replace(/_+/g, "_")
      .replace(/^_+|_+$/g, "");

  const dataUriToBlob = async (dataUri: string) => {
    const response = await fetch(dataUri);
    return await response.blob();
  };

  const downloadBlob = (blob: Blob, fileName: string) => {
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    URL.revokeObjectURL(url);
  };

  const exportCurrentTexture = async () => {
    if (!selectedRender) return;
    const label = sanitizeFileName(
      getRenderLabel(selectedRender, selectedRenderIndex),
    );
    const blob = await dataUriToBlob(
      `data:image/png;base64,${selectedRender.image}`,
    );
    downloadBlob(blob, `${label || "texture"}.png`);
  };

  const exportConfiguration = async () => {
    await ExportConfig($currentOutfits, $currentSkinModel);
  };

  const exportAllTextures = async () => {
    if (allGeneratedItems.length === 0) return;

    const zip = new JSZip();
    const outfitsFolder = zip.folder("outfits");
    const setsFolder = zip.folder("sets");

    await Promise.all(
      generatedOutfits.map(async (item, index) => {
        const label =
          sanitizeFileName(getRenderLabel(item, index)) ||
          `outfit_${index + 1}`;
        const blob = await dataUriToBlob(`data:image/png;base64,${item.image}`);
        outfitsFolder?.file(`${label}.png`, blob);
      }),
    );

    await Promise.all(
      generatedSets.map(async (item, index) => {
        const label =
          sanitizeFileName(getRenderLabel(item, index)) || `set_${index + 1}`;
        const blob = await dataUriToBlob(`data:image/png;base64,${item.image}`);
        setsFolder?.file(`${label}.png`, blob);
      }),
    );

    const blob = await zip.generateAsync({ type: "blob" });
    downloadBlob(blob, "generated-textures.zip");
  };

  const setTab = (tab: any) => {
    selectedTab = tab.value;
    if (tab === "outfits" && selectedOutfitIndex >= generatedOutfits.length) {
      selectedOutfitIndex = 0;
    }
    if (tab === "sets" && selectedSetIndex >= generatedSets.length) {
      selectedSetIndex = 0;
    }
  };

  const selectItem = (tab: GeneratedTab, index: number) => {
    if (tab === "outfits") {
      selectedOutfitIndex = index;
      selectedTab = tab;
      if ($IS_MOBILE_VIEW) renderDialogOpen = true;
      return;
    }

    selectedSetIndex = index;
    selectedTab = tab;
    if ($IS_MOBILE_VIEW) renderDialogOpen = true;
  };

  const getItemId = (item: GeneratedItem) =>
    String(item.id ?? item.config?.id ?? "");

  const showGroupSeparator = (
    current: GeneratedItem,
    previous?: GeneratedItem,
  ) => previous != null && getItemId(current) !== getItemId(previous);

  onMount(async () => {
    const threeModule = await THREE.getThree();
    renderer = new threeModule.WebGLRenderer({
      alpha: true,
    });
    renderer.outputColorSpace = threeModule.LinearSRGBColorSpace;

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
      generatedOutfits = (generated?.outfits ?? []) as GeneratedItem[];
      generatedSets = (generated?.sets ?? []) as GeneratedItem[];
      selectedTab = generatedOutfits.length > 0 ? "outfits" : "sets";
      selectedOutfitIndex = 0;
      selectedSetIndex = 0;
      renderDialogOpen = false;
    } catch (error) {
      loadError =
        error instanceof Error ? error.message : "Failed to generate outfits.";
      generatedOutfits = [];
      generatedSets = [];
      selectedOutfitIndex = 0;
      selectedSetIndex = 0;
      renderDialogOpen = false;
    } finally {
      isLoading = false;
    }
  });
</script>

<div
  id="container"
  class:mobile={$IS_MOBILE_VIEW}
  style="height: calc(100dvh - 100px); display:flex; flex-direction:column;"
>
  <div id="toolbox">
    <div></div>
    <Button
      label={$_("page.export")}
      type="tertiary"
      size="large"
      icon={ExportIcon}
      onclick={exportConfiguration}
    />
    <div class="separator vertical"></div>
    <Button
      label={$_("generatedPage.exportCurrent")}
      icon={DownloadIcon}
      type="tertiary"
      onclick={exportCurrentTexture}
      size="large"
      disabled={selectedRender == null}
    />
    <Button
      label={$_("generatedPage.exportZip")}
      icon={DownloadIcon}
      size="large"
      type="primary"
      onclick={exportAllTextures}
      disabled={allGeneratedItems.length === 0}
    />
  </div>

  {#if isLoading}
    <p class="state">Generating outfits, please wait...</p>
  {:else if loadError}
    <p class="state error">{loadError}</p>
  {:else if generatedOutfits.length === 0 && generatedSets.length === 0}
    <p class="state">No generated renders found.</p>
  {:else}
    <div id="preview">
      <div id="preview-items">
        <div id="preview-items-categories">
          <RadioGroup
            options={tabs.map(
              (tab) => new ValueData(tab.key, `${tab.label} (${tab.count})`),
            )}
            selectedValue={selectedTab}
            onselect={(e) => setTab(e.value)}
          />
        </div>
        <div id="preview-items-list">
          {#each sortedActiveItems as item, index}
            {#if showGroupSeparator(item, sortedActiveItems[index - 1])}
              <div class="separator horizontal" style="margin: 8px 0;"></div>
            {/if}
            <button
              type="button"
              class="render-list-item"
              class:selected={item === selectedRender}
              onclick={() => selectItem(selectedTab as GeneratedTab, index)}
              aria-pressed={item === selectedRender}
            >
              <div class="render-thumb">
                <OutfitPackageRender
                  source={"data:image/png;base64," + item.image}
                  model={$currentSkinModel === "slim"
                    ? MODEL_TYPE.ALEX
                    : MODEL_TYPE.STEVE}
                  outfitType={"set"}
                  isDynamic={false}
                />
              </div>
              <div class="data">
                <span
                  >{item.config.name +
                    (item.config.samples > 0
                      ? ` (${item.config.samples})`
                      : "")}</span
                >
                <div>
                  <Label
                    variant="common"
                    dense
                    text={item?.config?.type
                      ? $t(
                          `options.outfitType.${item?.config?.type?.toLowerCase()}`,
                        )
                      : ""}
                  />
                </div>
              </div>
            </button>
          {/each}
        </div>
      </div>
      {#if selectedRender}
        {#if $IS_MOBILE_VIEW}
          <Dialog
            open={renderDialogOpen && selectedRender !== null}
            label={selectedRender?.config?.name || "Selected Render"}
            onclose={() => (renderDialogOpen = false)}
            className="generated-render-dialog"
          >
            <div class="mobile-render-content">
              <div>
                <Label
                  variant="common"
                  text={selectedRender?.config?.type
                    ? $t(
                        `options.outfitType.${selectedRender?.config?.type?.toLowerCase()}`,
                      )
                    : ""}
                />
              </div>
              <div class="render-container">
                <OutfitPackageRender
                  source={"data:image/png;base64," + selectedRender.image}
                  model={$currentSkinModel === "slim"
                    ? MODEL_TYPE.ALEX
                    : MODEL_TYPE.STEVE}
                  isDynamic
                  resizable
                  {renderer}
                />
              </div>
            </div>
          </Dialog>
        {:else}
          <div id="selected-preview">
            <span style="margin-bottom:12px;">
              <h2 style="margin-bottom: 4px;">
                {selectedRender?.config?.name || "Selected Render"}
              </h2>
              <Label
                variant="common"
                text={selectedRender?.config?.type
                  ? $t(
                      `options.outfitType.${selectedRender?.config?.type?.toLowerCase()}`,
                    )
                  : ""}
              />
            </span>
            <div class="render-container">
              <OutfitPackageRender
                source={"data:image/png;base64," + selectedRender.image}
                model={$currentSkinModel === "slim"
                  ? MODEL_TYPE.ALEX
                  : MODEL_TYPE.STEVE}
                isDynamic
                resizable
                {renderer}
              />
            </div>
          </div>
        {/if}
      {:else}
        <p class="no-selection">{$_("generatedPage.noSelection")}</p>
      {/if}
    </div>
  {/if}
</div>

<style lang="scss">
  @use "style.scss";
</style>
