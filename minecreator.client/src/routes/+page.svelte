<script lang="ts">
  import { IS_MOBILE_VIEW } from "$data/global";
  import Button from "$lib/components/Button/Button.svelte";
  import AddIcon from "$icons/plus.svg?raw";
  import ExportIcon from "$icons/upload.svg?raw";
  import ImportIcon from "$icons/download.svg?raw";
  import Select from "$lib/components/Select/Select.svelte";
  import ClothListItem from "$lib/components/ClothListItem/ClothListItem.svelte";

  let selectedVersion = $state<string | null>("modern");
  let selectedSkinModel = $state<string[] | null>(["classic"]);
  let clothesList: { name: string; type: string }[] = $state([]);
  let selectedCloth = $state<{ name: string; type: string } | null>(null);
  const newCloth = function () {
    const newCloth = { name: "New Cloth", type: "shirt" + Math.random() };
    clothesList = [newCloth, ...clothesList];
    selectedCloth = newCloth;
  };
  const updateCloth = function (cloth: { name: string; type: string }) {
    clothesList = clothesList.map((c) => (c === selectedCloth ? cloth : c));
    selectedCloth = cloth;
  };
</script>

<div id="container" class:mobile={$IS_MOBILE_VIEW}>
  <h1>MineCreator</h1>
  <div id="toolbox">
    <div class="option-select">
      <b class="label">Game Version</b>
      <Select
        items={[
          { label: "Modern", value: "modern" },
          { label: "Beta and Lower", value: "beta" },
        ]}
        bind:selectedItem={selectedVersion}
        itemText="label"
        itemValue="value"
        placeholder="Select game version"
      />
    </div>
    <div class="option-select">
      <b class="label">Skin Model</b>
      <Select
        items={[
          { label: "Classic", value: "classic" },
          { label: "Slim", value: "slim" },
        ]}
        bind:selectedItem={selectedSkinModel}
        itemText="label"
        itemValue="value"
        multiple
        placeholder="Select skin model"
      />
    </div>
  </div>
  <div id="items">
    <div id="items-tools">
      <div></div>
      <Button
        onlyIcon={$IS_MOBILE_VIEW}
        label="Export"
        type="tertiary"
        icon={ExportIcon}
        onclick={() => alert("Export config clicked!")}
      />
      <Button
        onlyIcon={$IS_MOBILE_VIEW}
        label="Import"
        type="tertiary"
        icon={ImportIcon}
        onclick={() => alert("Import config clicked!")}
      />
      <div class="separator vertical"></div>
      <Button
        label="new item"
        icon={AddIcon}
        onclick={newCloth}
        disabled={selectedVersion === null}
      />
    </div>
    <div class="separator horizontal"></div>
    <div id="items-content">
      <div id="items-list">
        {#each clothesList as cloth}
          <ClothListItem
            onclick={() => updateCloth({ ...cloth, name: cloth.name + "!" })}
            selected={selectedCloth?.type === cloth.type}
          />
        {/each}
      </div>
      <div id="items-preview">preview</div>
    </div>
  </div>
</div>

<style lang="scss">
  @use "style.scss";
</style>
