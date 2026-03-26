<script lang="ts">
  import type { Snippet } from "svelte";
  import Resize from "$lib/components/other/Resize/Resize.svelte";
  import { IS_MOBILE_VIEW } from "$data/global";

  //services
  import { clickOutside } from "$src/helpers/componentHelper";
  import { on } from "svelte/events";
  
  let {
    opened = $bindable(false),
    caller = null,
    position = "auto",
    align = "right",
    preventClickOutsideClose = false,
    autoWidth = true,
    resizable = false,
    children,
  } = $props<{
    opened?: boolean;
    caller?: HTMLElement | null;
    position?: "top" | "left" | "right" | "bottom" | "auto";
    align?: "left" | "right" | "center";
    preventClickOutsideClose?: boolean;
    autoWidth?: boolean;
    resizable?: boolean;
    children?: Snippet<[{ position: string }]>;
  }>();

  let actualPosition = $state<"top" | "left" | "right" | "bottom" | "auto">("auto");

  let component: HTMLDivElement | null = null;
  let componentContent = $state<HTMLDivElement | null>(null);

  const onClose = () => {
    if (opened && !preventClickOutsideClose) opened = false;
  };
  const onStateChanged = (v: any) => {
    let parentNode = document.body;
    if (!component || !parentNode) return;
    if (caller) {
      parentNode = caller;
    }
    onResize();
  };
  const calculatePosition = () => {
    if(!component) return;
    const flyoutRect = component.getBoundingClientRect();
    const callerRect = caller?.getBoundingClientRect();
    if (autoWidth && !$IS_MOBILE_VIEW)
      component.style.minWidth = callerRect?.width + "px";
    else component.style.minWidth = "";
    //component.style.maxWidth = callerRect?.width + "px";
    component.style.left = "";
    component.style.right = "";
    component.style.top = "";
    component.style.bottom = "";
    component.style.maxHeight = "";
    if ($IS_MOBILE_VIEW) {
      return;
    }
    //calculate needed space
    if (position == "auto") {
      if (
        flyoutRect.height + callerRect.top + callerRect.height >
        window.innerHeight
      )
        actualPosition = "top";
      else actualPosition = "bottom";
    }
    if (actualPosition == "top") {
      component.style.bottom = "100%";
      //set maxheight
      component.style.maxHeight = callerRect.top - 50 + "px";
    }
    if (actualPosition == "bottom") {
      component.style.top = "100%";
      //set maxheight
      component.style.maxHeight =
        window.innerHeight - (callerRect.top + callerRect.height) - 50 + "px";
    }
    //align
    if (align == "left") {
      component.style.left = "";
      component.style.right = "";
    }
    if (align == "right") {
      component.style.left = "";
      component.style.right = 0 + "px";
    }
    if (align == "center") {
      component.style.left =
        callerRect.left +
        callerRect.width / 2 -
        (flyoutRect.left + flyoutRect.width / 2) +
        "px";
      component.style.right = "";
    }
  };
  const onResize = () => {
    if (!resizable) return;
    if(!component) return;
    const callerRect = caller?.getBoundingClientRect();
    if (autoWidth && !$IS_MOBILE_VIEW) {
      component.style.minWidth = callerRect?.width + "px";
      component.style.maxWidth = callerRect?.width + "px";
    } else {
      component.style.minWidth = "";
      component.style.maxWidth = "";
    }
  };
  const onComponentResize = () => {
    if (!opened) return;
    requestAnimationFrame(() => {
      calculatePosition();
    });
  };
  $effect(() => {
    onStateChanged(opened);
  });

  $effect(() => {
    actualPosition = position;
  });
</script>

<div
  bind:this={component}
  use:clickOutside={onClose}
  class:opened
  class="flyout"
  class:closed={!opened}
  class:mobile={$IS_MOBILE_VIEW}
>
  <Resize targetNode={caller} onresize={onResize} debounce={100}></Resize>
  <Resize targetNode={componentContent} onresize={onComponentResize}></Resize>
  <div bind:this={componentContent} class="flyout-content">
    {@render children?.({ position: actualPosition })}
  </div>
  <!-- svelte-ignore a11y_click_events_have_key_events -->
  <!-- svelte-ignore a11y_no_static_element_interactions -->
  <div class="flyout-mobile-bg" onclick={() => (opened = false)}></div>
</div>

<style lang="scss">
  .flyout {
    position: absolute;
    z-index: 20;
    &.opened {
      display: flex;
    }
    &.closed {
      display: none;
    }
    .flyout-content {
      flex: 1;
    }
    &.mobile {
      /* stick to bottom of the viewport on mobile */
      position: fixed;
      left: 0;
      right: 0;
      bottom: 0;
      width: 100%;
      justify-content: center;
      box-sizing: border-box;
      align-items: flex-end;
      height: 100vh;
      .flyout-content {
        position: relative;
        width: 100%;
        max-width: 100%;
        max-height: 75vh;
        overflow: auto;
      }
      &.opened .flyout-mobile-bg {
        display: block;
      }
      .flyout-mobile-bg {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: -1;
        display: none;
        background-color: var(--color-dialog);
      }
    }
  }
</style>
