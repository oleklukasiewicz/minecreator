<script lang="ts">
  import { onMount } from "svelte";
  import { Setup } from "$data/global";
  import { initializeI18n } from "$src/i18n";
  import favicon from "$lib/assets/favicon.svg";
  let { children } = $props();

  onMount(() => {
    initializeI18n();
    Setup();
  });
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

{@render children()}
