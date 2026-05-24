<script lang="ts">
  import type { Outfit } from "$src/data/outfit";
  import Button from "../base/Button/Button.svelte";
  import CancelIcon from "$icons/close.svg?raw";
  import Label from "../base/Label/Label.svelte";
  import { _, t } from "svelte-i18n";
  //main imports

  let {
    outfit,
    onclick,
    onremove,
    selected = false,
  }: {
    outfit?: Outfit;
    onclick?: () => void;
    onremove?: () => void;
    selected?: boolean;
  } = $props();
</script>

<!-- svelte-ignore a11y_missing_attribute -->
<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<a class="outfit-list-item" class:selected {onclick}>
  <div class="data">
    <b>{outfit?.name?.length == 0 ? "Unnamed Outfit" : outfit?.name}</b>
    <div>
      <Label
        variant="common"
        dense
        text={outfit ? $t(`options.outfitType.${outfit.type}`) : ""}
      />
    </div>
  </div>
  <div class="actions" onclick={(e) => e.stopPropagation()}>
    <Button
      type="quaternary"
      onlyIcon
      icon={CancelIcon}
      whiteText={selected}
      onclick={onremove}
    />
  </div>
</a>

<style lang="scss">
  .outfit-list-item {
    color: var(--color---color-font);
    padding: 8px;
    box-sizing: border-box;
    display: grid;
    gap: 12px;
    grid-template-columns: minmax(0, 1fr) auto;
    cursor: pointer;
    user-select: none;
    overflow: hidden;
    background-color: var(--color-theme-D2);
    &:hover {
      background-color: var(--color-theme-D3);
    }
    &:active {
      background-color: var(--color-active);
    }
    &.selected {
      background-color: var(--color-accent);
      color: var(--color-accent-font);
    }
    .data {
      display: flex;
      flex-direction: column;
      text-align: left;
      gap: 4px;
      min-width: 0;
      b {
        font-size: var(--size-font-subtitle);
        font-family: minecraft;
        text-overflow: ellipsis;
        overflow: hidden;
        max-width: 100%;
        white-space: nowrap;
      }
    }
  }
</style>
