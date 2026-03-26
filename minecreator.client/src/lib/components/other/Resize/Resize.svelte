<script lang="ts">
  //main imports
  import { onDestroy, onMount } from "svelte";

  let {
    debounce = 0,
    targetNode = null,
    onresize,
  }: {
    debounce?: number;
    targetNode?: any;
    onresize?: () => void;
  } = $props();

  let timeout: number | undefined;
  let resizeObserver: ResizeObserver | null = null;
  let _targetNode: Element | null = null;
  let initialized = false;

  const updateTargetNode = (node: any) => {
    if (!node) return;
    if (_targetNode) {
      resizeObserver?.unobserve(_targetNode);
    }
    _targetNode = node;
    observe();
  };
  onMount(() => {
    if (targetNode) {
      updateTargetNode(targetNode);
    }
  });
  onDestroy(() => {
    clearTimeout(timeout);
    if (_targetNode) {
      resizeObserver?.unobserve(_targetNode);
    }
  });

  $effect(() => {
    updateTargetNode(targetNode);
  });

  function observe() {
    if (!resizeObserver)
      resizeObserver = new ResizeObserver(() => {
        clearTimeout(timeout);
        if (!initialized) {
          initialized = true;
          return;
        }
        if (debounce == -1) {
          onresize?.();
        }
        timeout = setTimeout(() => {
          onresize?.();
        }, debounce);
      });
    if (_targetNode) {
      resizeObserver.observe(_targetNode);
    }

    return {
      destroy() {
        clearTimeout(timeout);
        if (_targetNode) {
          resizeObserver?.unobserve(_targetNode);
        }
      },
    };
  }
</script>

<div></div>

<style lang="scss">
  div {
    display: none;
  }
</style>
