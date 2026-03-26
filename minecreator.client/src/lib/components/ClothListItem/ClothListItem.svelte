<script lang="ts">
  //main imports
  import { onMount } from "svelte";

  let {
    onclick,
    selected = $bindable(false),
  }: { onclick?: () => void; selected?: boolean } = $props();
</script>

<!-- svelte-ignore a11y_missing_attribute -->
<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<a class="cloth-list-item" class:selected {onclick}> Cloth List Item </a>

<style lang="scss">
  .cloth-list-item {
    color: var(--color---color-font);
    padding: 8px 12px;
    box-sizing: border-box;
    display: flex;
    cursor: pointer;
    user-select: none;
    background-color: var(--color-theme-D2);
    &:hover {
      background-color: var(--color-hover);
    }
    &:active {
      background-color: var(--color-active-L2);
    }
    &.selected {
      background-color: var(--color-accent);
      color: var(--color-font-accent);
    }
  }
</style>
