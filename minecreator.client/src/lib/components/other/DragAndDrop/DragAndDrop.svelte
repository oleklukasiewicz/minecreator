<script lang="ts">
  //main imports
  let isDragging = $state(false);
  let { children, ondrop } = $props();

  const hasFiles = function (e: DragEvent) {
    return e.dataTransfer?.types?.includes("Files") ?? false;
  };

  const handleDragEnter = function (e: DragEvent) {
    if (!hasFiles(e)) return;
    isDragging = true;
  };
  const handleDragLeave = function (e: DragEvent) {
    if (!hasFiles(e)) return;

    const container = e.currentTarget as HTMLElement | null;
    if (!container) {
      isDragging = false;
      return;
    }

    const nextTarget = e.relatedTarget as Node | null;
    if (nextTarget && container.contains(nextTarget)) return;

    isDragging = false;
  };
  const handleDragOver = function (e: DragEvent) {
    e.preventDefault();
  };
  const handleDrop = function (e: DragEvent) {
    e.preventDefault();
    const items = (Array.from(e.dataTransfer?.items ?? []) as DataTransferItem[])
      .filter((item) => item.kind === "file")
      .map((item) => item.getAsFile());

    ondrop?.(items);
    isDragging = false;
  };
</script>

<!-- svelte-ignore a11y_interactive_supports_focus -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
  class="drag-and-drop"
  class:isDragging
  ondrop={handleDrop}
  ondragover={handleDragOver}
  ondragenter={handleDragEnter}
  ondragleave={handleDragLeave}
>
  {@render children?.()}
</div>

<style lang="scss">
  .drag-and-drop {
    outline: 3px solid transparent;
    outline-offset: 2px;
    user-select: none;

    &.isDragging {
      outline-color: var(--color-accent);
    }
  }
</style>
