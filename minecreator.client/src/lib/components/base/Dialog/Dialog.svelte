<script lang="ts">
  import { onDestroy } from "svelte";
  import Button from "../Button/Button.svelte";
  import CloseIcon from "$icons/close.svg?raw";
  import { IS_MOBILE_VIEW } from "$data/global";

  let {
    open = false,
    style = "",
    label = "",
    showTitleBar = true,
    className = "",
    onclose,
  }: {
    open?: boolean;
    style?: string;
    label?: string;
    showTitleBar?: boolean;
    className?: string;
    onclose?: () => void;
  } = $props();

  const onClose = () => {
    open = false;
    onclose?.();
  };

  const onOverlayKeydown = (event: KeyboardEvent) => {
    if (event.key === "Escape") {
      event.preventDefault();
      onClose();
    }
  };

  const onContentClick = (event: MouseEvent) => {
    event.stopPropagation();
  };

  $effect(() => {
    if (!open || !$IS_MOBILE_VIEW) return;

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.body.style.overflow = previousOverflow;
    };
  });

  onDestroy(() => {
    document.body.style.overflow = "";
  });
</script>

<div
  class="dialog {className}"
  class:open={open}
  role="button"
  tabindex="0"
  aria-label="Close dialog"
  onclick={onClose}
  onkeydown={onOverlayKeydown}
  class:mobile={$IS_MOBILE_VIEW}
>
  {#if open}
    <div class="dialog-content" role="dialog" aria-modal="true" onclick={onContentClick}>
      {#if showTitleBar}
        <div class="dialog-title-bar">
          <span>{label || ""}</span>
          <Button
            type="quaternary"
            icon={CloseIcon}
            label="Close"
            iconSize="large"
            onlyIcon
            onclick={onClose}
          />
        </div>
      {/if}
      <div class="dialog-content-container" style={style}>
        <slot isMobile={$IS_MOBILE_VIEW} />
      </div>
    </div>
  {/if}
</div>

<style lang="scss">
  @use "Dialog.scss";
</style>
