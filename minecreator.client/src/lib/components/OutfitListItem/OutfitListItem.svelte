<script lang="ts">
  import type { Outfit } from "$src/data/outfit";
  import Button from "../base/Button/Button.svelte";
  import CancelIcon from "$icons/close.svg?raw";
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
<a class="outfit-list-item" class:selected={selected} {onclick}>
  <div class="preview"></div>
  <div class="data">
    <b>{outfit?.name.length==0 ? "Unnamed Outfit" : outfit?.name}</b>
    <span>{outfit?.type}</span>
  </div>
  <div class="actions" onclick={(e) => e.stopPropagation()}>
    <Button type="quaternary" onlyIcon icon={CancelIcon} onclick={onremove} />
  </div>
</a>

<style lang="scss">
  .outfit-list-item {
    color: var(--color---color-font);
    padding: 8px 12px;
    box-sizing: border-box;
    display: grid;
    gap: 8px;
    grid-template-columns: 64px minmax(0, 1fr) auto;
    cursor: pointer;
    user-select: none;
    overflow: hidden;
    background-color: var(--color-theme-D2);
    &:hover {
      background-color: var(--color-hover);
      color: var(--color-accent-font);
    }
    &:active {
      background-color: var(--color-active);
    }
    &.selected {
      background-color: var(--color-accent);
      color: var(--color-font-accent);
    }
    .preview {
      width: 64px;
      height: 64px;
      background-color: var(--color-theme-D1);
    }
    .data {
      display: flex;
      flex-direction: column;
      text-align: left;
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
