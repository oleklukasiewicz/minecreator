<script lang="ts">
  import { onMount } from "svelte";
  import { Setup } from "$data/global";
  import favicon from "$lib/assets/favicon.svg";
  let { children } = $props();

  onMount(() => {
	Setup();
  });
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

{@render children()}
