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

  let timeout: ReturnType<typeof setTimeout> | null = null;
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
    if (timeout) clearTimeout(timeout);
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
        if (timeout) clearTimeout(timeout);
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
        if (timeout) clearTimeout(timeout);
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
