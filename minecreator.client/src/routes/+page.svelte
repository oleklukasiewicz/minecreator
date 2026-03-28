<script lang="ts">
  import { ExportConfig } from "$src/data/export";
  import { IS_MOBILE_VIEW } from "$data/global";
  import { ImportConfig, ImportConfigFromFile } from "$src/data/import";
  import { GAME_VERSION, Outfit, SKIN_MODEL } from "$data/outfit";
  import Button from "$lib/components/base/Button/Button.svelte";
  import SectionTitle from "$lib/components/base/SectionTitle/SectionTitle.svelte";
  import Select from "$lib/components/base/Select/Select.svelte";
  import OutfitPreview from "$lib/components/layout/OutfitPreview/OutfitPreview.svelte";
  import ClothListItem from "$lib/components/OutfitListItem/OutfitListItem.svelte";
  import DragAndDrop from "$lib/components/other/DragAndDrop/DragAndDrop.svelte";
  import ImportIcon from "$icons/download.svg?raw";
  import AddIcon from "$icons/plus.svg?raw";
  import ExportIcon from "$icons/upload.svg?raw";

  let selectedVersion = $state<string | null>("modern");
  let selectedSkinModel = $state<string[] | null>(["classic"]);

  let outfitList: Outfit[] = $state<Outfit[]>([]);
  let selectedOutfit = $state<Outfit | null>(null);

  const addDefaultOutfit = function () {
    const timestamp = Date.now();

    const newOutfit: Outfit = new Outfit();

    newOutfit.id = timestamp.toString();
    newOutfit.name = `Outfit ${outfitList.length + 1}`;

    outfitList = [newOutfit, ...outfitList];
    selectedOutfit = newOutfit;
  };
  const setSelectedOutfit = function (outfit: Outfit) {
    selectedOutfit = outfit;
  };
  const updateSelectedOutfit = function (updatedOutfit: Outfit) {
    if (!selectedOutfit) return;

    const updated = Object.assign(new Outfit(), selectedOutfit, updatedOutfit);
    outfitList = outfitList.map((o) => (o.id === updated.id ? updated : o));
    selectedOutfit = updated;
  };
  const ExportData = async function () {
    await ExportConfig(outfitList);
  };
  const ImportData = async function () {
    const data = await ImportConfig();
    if (data) {
      outfitList = [...data, ...outfitList];
      selectedOutfit = null;
    }
  };
  const HandleDrop = async function (e: File[]) {
    const file = e[0];
    if (file) {
      const data = await ImportConfigFromFile(file);
      outfitList = [...data, ...outfitList];
      selectedOutfit = outfitList[0];
    }
  };
  const RemoveOutfit = function (outfit: Outfit) {
    outfitList = outfitList.filter((o) => o.id !== outfit.id);

    if (selectedOutfit?.id === outfit.id) {
      selectedOutfit = outfitList.length > 0 ? outfitList[0] : null;
    }
  };
</script>

<div id="container" class:mobile={$IS_MOBILE_VIEW}>
  <h1>MineCreator</h1>
  <div id="toolbox">
    <div class="option-select">
      <SectionTitle label="Game Version" />
      <Select
        items={GAME_VERSION}
        bind:selectedItem={selectedVersion}
        itemText="label"
        itemValue="value"
        placeholder="Select game version"
      />
    </div>
    <div class="option-select">
      <SectionTitle label="Skin Model" />
      <Select
        items={SKIN_MODEL}
        bind:selectedItem={selectedSkinModel}
        itemText="label"
        itemValue="value"
        multiple
        placeholder="Select skin model"
      />
    </div>
    <div></div>
    <div id="generate">
      <Button
        disabled={outfitList.length === 0 ||
          selectedVersion === null ||
          selectedSkinModel === null ||
          selectedSkinModel.length === 0}
        label="Generate"
        style="height:64px;"
        size="large"
        onclick={() => alert("Generate clicked!")}
      />
    </div>
  </div>
  <DragAndDrop ondrop={HandleDrop}>
    <div id="items">
      <div id="items-tools">
        <div></div>
        <Button
          onlyIcon={$IS_MOBILE_VIEW}
          label="Export"
          type="tertiary"
          icon={ExportIcon}
          onclick={ExportData}
        />
        <Button
          onlyIcon={$IS_MOBILE_VIEW}
          label="Import"
          type="tertiary"
          icon={ImportIcon}
          onclick={ImportData}
        />
        <div class="separator vertical"></div>
        <Button
          label="new outfit"
          icon={AddIcon}
          onclick={addDefaultOutfit}
          disabled={selectedVersion === null}
        />
      </div>
      <div class="separator horizontal"></div>
      {#if outfitList.length === 0}
        <span id="no-outfits">No outfits</span>
      {:else}
        <div id="items-content">
          <div id="items-list">
            {#each outfitList as outfit (outfit.id)}
              <ClothListItem
                {outfit}
                onclick={() => setSelectedOutfit(outfit)}
                selected={selectedOutfit?.id === outfit.id}
                onremove={() => RemoveOutfit(outfit)}
              />
            {/each}
          </div>
          <div id="items-preview">
            <OutfitPreview
              outfit={selectedOutfit}
              onUpdate={updateSelectedOutfit}
            />
          </div>
        </div>
      {/if}
    </div></DragAndDrop
  >
</div>

<style lang="scss">
  @use "style.scss";
</style>
