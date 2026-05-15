<script lang="ts">
  import { ExportConfig } from "$data/export";
  import { IS_MOBILE_VIEW } from "$data/global";
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
  import { GenerateOutfits, GetConfiguration } from "$src/data/api";
  import { Configuration } from "$src/data/config";

  let selectedVersion = $state<string | null>("modern");
  let selectedSkinModel = $state<string>("classic");
  let currentLocale = $state<string>("en");
  let outfitDialogOpen = $state(false);

  let outfitList: Outfit[] = $state<Outfit[]>([]);
  let selectedOutfit = $state<Outfit | null>(null);

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
    newOutfit.name = `Outfit ${outfitList.length + 1}`;
    newOutfit.seed = timestamp.toString();
    newOutfit.samples = 1;
    newOutfit.type = configuration.modulesConfig[0]?.name.toLowerCase();
    newOutfit.style = configuration.modulesConfig[0]?.styles[0].toLowerCase();

    outfitList = [newOutfit, ...outfitList];
    selectedOutfit = newOutfit;
    if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
  };
  const setSelectedOutfit = function (outfit: Outfit) {
    selectedOutfit = outfit;
    if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
  };
  const updateSelectedOutfit = function (updatedOutfit: Outfit) {
    if (!selectedOutfit) return;

    const updated = Object.assign(new Outfit(), selectedOutfit, updatedOutfit);
    outfitList = outfitList.map((o) => (o.id === updated.id ? updated : o));
    selectedOutfit = updated;
  };
  const ExportData = async function () {
    await ExportConfig(outfitList, selectedSkinModel);
  };
  const ImportData = async function () {
    const data = await ImportConfig();
    if (data) {
      outfitList = [...data, ...outfitList];
      if (data.length > 0) {
        selectedOutfit = outfitList[0];
        if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
      }
    }
  };
  const HandleDrop = async function (e: File[]) {
    const file = e[0];
    if (file) {
      const data = await ImportConfigFromFile(file);
      outfitList = [...data, ...outfitList];
      if (data.length > 0) {
        selectedOutfit = outfitList[0];
        if ($IS_MOBILE_VIEW) outfitDialogOpen = true;
      }
    }
  };
  const RemoveOutfit = function (outfit: Outfit) {
    outfitList = outfitList.filter((o) => o.id !== outfit.id);

    if (selectedOutfit?.id === outfit.id) {
      selectedOutfit = outfitList.length > 0 ? outfitList[0] : null;
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
    const json = JSON.stringify(
      {
        model: selectedSkinModel,
        outfits: outfitList.map((o) => o.ToExportModel()),
      },
      null,
      2,
    );

    var generated = await GenerateOutfits(json);
    generatedOutfits = "data:image/png;base64," + generated;
    await tick();
    if (previewComponent && typeof previewComponent.addAnimation === "function") {
      previewComponent.addAnimation(DefaultAnimation);
    }
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
        bind:selectedItem={selectedVersion}
        itemText="label"
        itemValue="value"
        placeholder={$_("page.gameVersionPlaceholder")}
      />
    </div>
    <div class="option-select">
      <SectionTitle label={$_("page.skinModel")} />
      <Select
        items={translatedSkinModels}
        bind:selectedItem={selectedSkinModel}
        itemText="label"
        itemValue="value"
        placeholder={$_("page.skinModelPlaceholder")}
      />
    </div>
    <div></div>
    <div id="generate">
      <Button
        disabled={outfitList.length === 0 ||
          selectedVersion === null ||
          selectedSkinModel === null ||
          selectedSkinModel.length === 0}
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
          disabled={selectedVersion === null ||
            selectedSkinModel === null ||
            selectedSkinModel.length === 0}
        />
      </div>
      <div class="separator horizontal"></div>
      {#if outfitList.length === 0}
        <span id="no-outfits">{$_("page.noOutfits")}</span>
      {:else}
        <div id="items-content">
          <div id="items-list">
            {#each outfitList as outfit (outfit.id)}
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
                {#if generatedOutfits.length > 0}
                  <OutfitPackageRender
                    source={generatedOutfits}
                    model={MODEL_TYPE.STEVE}
                    isDynamic
                    bind:this={previewComponent}
                  />
                {/if}
              </Dialog>
            {:else}
              <OutfitPreview
                outfit={selectedOutfit}
                onUpdate={updateSelectedOutfit}
                {configuration}
              />
              {#if generatedOutfits.length > 0}
                <OutfitPackageRender
                  source={generatedOutfits}
                  model={MODEL_TYPE.STEVE}
                  isDynamic
                  bind:this={previewComponent}
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
