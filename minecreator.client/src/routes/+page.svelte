<script lang="ts">
  import { ExportConfig } from "$data/export";
  import {
    currentOutfits,
    currentSkinModel,
    currentVersion,
    IS_MOBILE_VIEW,
  } from "$data/global";
  import { ImportConfig, ImportConfigFromFile } from "$data/import";
  import { GAME_VERSION, Outfit, SKIN_MODEL } from "$data/outfit";
  import { ValueData } from "$src/helpers/dataHelper";
  import { SUPPORTED_LOCALES, setAppLocale } from "$src/i18n";
  import { _, locale } from "svelte-i18n";
  import Button from "$lib/components/base/Button/Button.svelte";
  import SectionTitle from "$lib/components/base/SectionTitle/SectionTitle.svelte";
  import Select from "$lib/components/base/Select/Select.svelte";
  import OutfitPreview from "$lib/components/layout/OutfitPreview/OutfitPreview.svelte";
  import ClothListItem from "$lib/components/OutfitListItem/OutfitListItem.svelte";
  import DragAndDrop from "$lib/components/other/DragAndDrop/DragAndDrop.svelte";
  import ImportIcon from "$icons/download.svg?raw";
  import AddIcon from "$icons/plus.svg?raw";
  import ExportIcon from "$icons/upload.svg?raw";
  import GenerateIcon from "$icons/zap.svg?raw";
  import Dialog from "$lib/components/base/Dialog/Dialog.svelte";
  import { onMount, tick } from "svelte";
  import OutfitPackageRender from "$lib/components/render/OutfitPackageRender.svelte";
  import { MODEL_TYPE } from "$src/data/enums/model";
  import DefaultAnimation from "$src/animation/default";
  import {
    GenerateOutfits,
    GetConfiguration,
    PreviewOutfits,
  } from "$src/data/api";
  import { Configuration } from "$src/data/config";
  import { ExportModel } from "$src/data/models/export";
  import { goto } from "$app/navigation";

  let currentLocale = $state<string>("en");
  let outfitDialogOpen = $state(false);

  let selectedOutfit = $state<Outfit | null>(null);
  const previewDebounceTimers = new Map<
    string,
    ReturnType<typeof setTimeout>
  >();
  const previewRequestVersions = new Map<string, number>();

  const translatedGameVersions = $derived(
    GAME_VERSION.map(
      (item) =>
        new ValueData(item.value, $_(`options.gameVersion.${item.value}`)),
    ),
  );
  const translatedSkinModels = $derived(
    SKIN_MODEL.map(
      (item) =>
        new ValueData(item.value, $_(`options.skinModel.${item.value}`)),
    ),
  );
  const languageOptions = $derived(
    SUPPORTED_LOCALES.map(
      (code) => new ValueData(code, $_(`options.language.${code}`)),
    ),
  );
  let configuration: Configuration = $state(new Configuration());
  //mount
  onMount(async () => {
    //getconfig
    var config = await GetConfiguration();

    configuration = config;
  });

  const addDefaultOutfit = function () {
    const timestamp = Date.now();

    const newOutfit: Outfit = new Outfit();

    newOutfit.id = timestamp.toString();
    newOutfit.name = `Outfit ${$currentOutfits.length + 1}`;
    newOutfit.seed = timestamp.toString();
    newOutfit.samples = 1;
    newOutfit.type = configuration.modulesConfig[0]?.name.toLowerCase();
    newOutfit.style = configuration.modulesConfig[0]?.styles[0].toLowerCase();

    currentOutfits.update((outfits) => [newOutfit, ...outfits]);
    selectedOutfit = newOutfit;
    if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
  };
  const setSelectedOutfit = async function (outfit: Outfit) {
    selectedOutfit = outfit;
    if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
    queuePreviewGeneration(outfit);
  };
  const updateSelectedOutfit = async function (updatedOutfit: Outfit) {
    if (!selectedOutfit) return;

    const updated = Object.assign(new Outfit(), selectedOutfit, updatedOutfit);
    currentOutfits.update((outfits) =>
      outfits.map((o) => (o.id === updated.id ? updated : o)),
    );
    selectedOutfit = updated;
    queuePreviewGeneration(updated);
  };
  const queuePreviewGeneration = function (outfit: Outfit) {
    if (!outfit?.id) return;

    const timer = previewDebounceTimers.get(outfit.id);
    if (timer) clearTimeout(timer);

    previewDebounceTimers.set(
      outfit.id,
      setTimeout(() => {
        void generatePreview(outfit);
      }, 200),
    );
  };
  const generatePreview = async function (outfit: Outfit) {
    if (!outfit?.id) return;

    const requestVersion = (previewRequestVersions.get(outfit.id) ?? 0) + 1;
    previewRequestVersions.set(outfit.id, requestVersion);

    const outfitSnapshot = Object.assign(new Outfit(), outfit);

    try {
      const preview = await PreviewOutfits(
        new ExportModel($currentSkinModel, $currentVersion, [outfitSnapshot]),
      );

      // Ignore stale responses when a newer request for this outfit already exists.
      if (previewRequestVersions.get(outfit.id) !== requestVersion) return;

      const previewImage = preview?.outfits?.[0]?.image;

      currentOutfits.update((outfits) =>
        outfits.map((o) =>
          o.id === outfit.id
            ? Object.assign(new Outfit(), o, outfitSnapshot, {
                preview: previewImage,
              })
            : o,
        ),
      );

      if (selectedOutfit?.id === outfit.id) {
        selectedOutfit = Object.assign(
          new Outfit(),
          selectedOutfit,
          outfitSnapshot,
          {
            preview: previewImage,
          },
        );
      }
    } catch (err) {
      console.error("Preview generation failed", err);
    }
  };
  const ExportData = async function () {
    await ExportConfig($currentOutfits, $currentSkinModel);
  };
  const ImportData = async function () {
    const data = await ImportConfig();
    if (data) {
      currentOutfits.update((outfits) => [...data, ...outfits]);
      if (data.length > 0) {
        selectedOutfit = $currentOutfits[0];
        queuePreviewGeneration(selectedOutfit);
        if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
      }
    }
  };
  const HandleDrop = async function (e: File[]) {
    const file = e[0];
    if (file) {
      const data = await ImportConfigFromFile(file);
      currentOutfits.update((outfits) => [...data, ...outfits]);
      if (data.length > 0) {
        selectedOutfit = $currentOutfits[0];
        queuePreviewGeneration(selectedOutfit);
        if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
      }
    }
  };
  const RemoveOutfit = function (outfit: Outfit) {
    currentOutfits.update((outfits) =>
      outfits.filter((o) => o.id !== outfit.id),
    );

    if (selectedOutfit?.id === outfit.id) {
      selectedOutfit = $currentOutfits.length > 0 ? $currentOutfits[0] : null;
      if (!selectedOutfit) outfitDialogOpen = false;
    }
  };

  const changeLocale = (payload: { item: ValueData }) => {
    currentLocale = payload.item.value;
    setAppLocale(payload.item.value);
  };
  let previewComponent = $state<any>(null);
  let generatedOutfits = $state<string>("");

  const generateOutfits = async function () {
    goto("/generated");
  };

  $effect(() => {
    const activeLocale = $locale ?? "en";
    if (currentLocale !== activeLocale) currentLocale = activeLocale;
  });
</script>

<div id="container" class:mobile={$IS_MOBILE_VIEW}>
  <div id="lang-select">
    <div>
      <Select
        items={languageOptions}
        selectedItem={currentLocale}
        itemText="label"
        itemValue="value"
        placeholder={$_("common.select")}
        onselect={changeLocale}
      />
    </div>
  </div>
  <h1>{$_("page.title")}</h1>
  <div id="toolbox">
    <div class="option-select">
      <SectionTitle label={$_("page.gameVersion")} />
      <Select
        items={translatedGameVersions}
        bind:selectedItem={$currentVersion}
        itemText="label"
        itemValue="value"
        placeholder={$_("page.gameVersionPlaceholder")}
      />
    </div>
    <div class="option-select">
      <SectionTitle label={$_("page.skinModel")} />
      <Select
        items={translatedSkinModels}
        bind:selectedItem={$currentSkinModel}
        itemText="label"
        itemValue="value"
        placeholder={$_("page.skinModelPlaceholder")}
      />
    </div>
    <div></div>
    <div id="generate">
      <Button
        disabled={$currentOutfits.length === 0 ||
          $currentVersion === null ||
          $currentSkinModel === null ||
          $currentSkinModel.length === 0}
        label={$_("page.generate")}
        icon={GenerateIcon}
        style="height:64px;"
        size="large"
        onclick={() => generateOutfits()}
      />
    </div>
  </div>
  <DragAndDrop ondrop={HandleDrop}>
    <div id="items">
      <div id="items-tools">
        <div></div>
        <Button
          onlyIcon={$IS_MOBILE_VIEW}
          label={$_("page.export")}
          type="tertiary"
          icon={ExportIcon}
          onclick={ExportData}
        />
        <Button
          onlyIcon={$IS_MOBILE_VIEW}
          label={$_("page.import")}
          type="tertiary"
          icon={ImportIcon}
          onclick={ImportData}
        />
        <div class="separator vertical"></div>
        <Button
          label={$_("page.newOutfit")}
          icon={AddIcon}
          onclick={addDefaultOutfit}
          disabled={$currentVersion === null ||
            $currentSkinModel === null ||
            $currentSkinModel.length === 0}
        />
      </div>
      <div class="separator horizontal"></div>
      {#if $currentOutfits.length === 0}
        <span id="no-outfits">{$_("page.noOutfits")}</span>
      {:else}
        <div id="items-content">
          <div id="items-list">
            {#each $currentOutfits as outfit (outfit.id)}
              <ClothListItem
                {outfit}
                onclick={() => setSelectedOutfit(outfit)}
                selected={selectedOutfit?.id === outfit.id && !$IS_MOBILE_VIEW}
                onremove={() => RemoveOutfit(outfit)}
              />
            {/each}
          </div>
          <div id="items-preview">
            {#if $IS_MOBILE_VIEW}
              <Dialog
                open={outfitDialogOpen && selectedOutfit !== null}
                label={selectedOutfit?.name ??
                  $_("outfitPreview.noOutfitSelected")}
                onclose={() => (outfitDialogOpen = false)}
              >
                <OutfitPreview
                  outfit={selectedOutfit}
                  onUpdate={updateSelectedOutfit}
                  {configuration}
                />
              </Dialog>
            {:else}
              <OutfitPreview
                outfit={selectedOutfit}
                onUpdate={updateSelectedOutfit}
                {configuration}
              />
              {#if selectedOutfit?.preview != null}
                <OutfitPackageRender
                  resizable={true}
                  isDynamic={true}
                  source={"data:image/png;base64," + selectedOutfit.preview}
                  model={$currentSkinModel === "classic"
                    ? MODEL_TYPE.STEVE
                    : MODEL_TYPE.ALEX}
                />
              {/if}
            {/if}
          </div>
        </div>
      {/if}
    </div></DragAndDrop
  >
</div>

<style lang="scss">
  @use "style.scss";
</style>
