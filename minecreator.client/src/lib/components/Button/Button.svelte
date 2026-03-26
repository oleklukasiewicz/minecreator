<script lang="ts">
  import type { Snippet } from "svelte";

  let {
    onclick,
    href = null,
    label = null,
    icon = null,
    disabled = false,
    onlyIcon = false,
    noBorder = false,
    style = null,
    whiteText = false,
    flat = false,
    focused = false,
    type = "primary",
    size = "medium",
    iconSize = size,
    textAlign = "center",
    target = null,
    children,
  }: {
    onclick?: () => void;
    href?: string | null;
    label?: string | null;
    icon?: string | null;
    disabled?: boolean;
    onlyIcon?: boolean;
    noBorder?: boolean;
    style?: any;
    whiteText?: boolean;
    flat?: boolean;
    focused?: boolean;
    type?: "primary" | "secondary" | "tertiary" | "quaternary";
    size?: "small" | "medium" | "large" | "auto";
    iconSize?: "small" | "medium" | "large" | "auto";
    textAlign?: "left" | "center" | "right";
    target?: "_blank" | "_self" | null;
    children?: Snippet;
  } = $props();
</script>

<a
  {onclick}
  class="button"
  title={label}
  {style}
  {href}
  {target}
  class:focused={focused}
  class:flat
  class:white-text={whiteText}
  class:link={href != null}
  class:only-icon={onlyIcon}
  class:with-label={label != null && !onlyIcon}
  class:with-icon={icon != null}
  class:without-icon={!icon}
  class:no-border={noBorder}
  class:primary={type === "primary"}
  class:secondary={type === "secondary"}
  class:tertiary={type === "tertiary"}
  class:quaternary={type === "quaternary"}
  class:small={size === "small"}
  class:medium={size === "medium"}
  class:large={size === "large"}
  class:disabled
  class:text-left={textAlign === "left"}
  class:text-center={textAlign === "center"}
  class:text-right={textAlign === "right"}
>
  {#if icon != null}
    <div
      class="icon"
      class:b-icon-small={iconSize === "small"}
      class:b-icon-medium={iconSize === "medium"}
      class:b-icon-large={iconSize === "large"}
    >
      {@html icon}
    </div>
  {/if}
  {#if !onlyIcon}
    <div class="slot-container">
      {@render children?.()}
    </div>
  {/if}
  {#if label != null && !onlyIcon}
    <span>{label}</span>
  {/if}
</a>

<style lang="scss">
  @use "Button.scss";
</style>
