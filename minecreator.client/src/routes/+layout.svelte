<script lang="ts">
  import favicon from "$lib/assets/favicon.svg";
  import { Setup } from "$data/global";
  import { onMount } from "svelte";
  let { children } = $props();

  onMount(() => {
	Setup();
  });
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

{@render children()}
