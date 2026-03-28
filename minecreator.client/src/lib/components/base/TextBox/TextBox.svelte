<script lang="ts">
  //components
  import Button from "../Button/Button.svelte";
  //icons
  import CloseIcon from "$icons/close.svg?raw";

  let { value = $bindable(""), clearable = false, placeholder = "", oninput } = $props();

  const handleInput = (event: Event) => {
    if (oninput) oninput((event.target as HTMLInputElement).value);
  };
  const clear = () => {
    value = "";
    oninput?.("");
  };
</script>

<div class="text-box">
  <!-- svelte-ignore event_directive_deprecated -->
  <input bind:value oninput={handleInput} {placeholder} />
  {#if clearable && value?.length > 0}
    <Button
      onlyIcon
      style="height: 32px;border-left:2px solid var(--color-theme-D6);"
      icon={CloseIcon}
      type="secondary"
      iconSize="auto"
      noBorder
      onclick={clear}
    ></Button>
  {/if}
</div>

<style lang="scss">
  @use "TextBox.scss";
</style>
