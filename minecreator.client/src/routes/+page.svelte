<script lang="ts">
  import { IS_MOBILE_VIEW } from "$data/global";
  import Button from "$lib/components/Button/Button.svelte";
  import AddIcon from "$icons/plus.svg?raw";
  import Select from "$lib/components/Select/Select.svelte";

  let selectedVersion = $state<string | null>("modern");
  let selectedSkinModel = $state<string[] | null>(["classic"]);
  const newSkin = function () {
    alert("New item created!");
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
    <div></div>
  </div>
  <div id="items">
    <Button
      label="Create new item"
      icon={AddIcon}
      onclick={newSkin}
      disabled={selectedVersion === null}
    />
  </div>
</div>

<style lang="scss">
  @use "style.scss";
</style>
