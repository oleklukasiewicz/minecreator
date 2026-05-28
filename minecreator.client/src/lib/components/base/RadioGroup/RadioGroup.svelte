<script lang="ts">
  import type { ValueData } from "$src/helpers/dataHelper";

  //models

  import RadioButton from "../RadioButton/RadioButton.svelte";

  interface RadioGroupProps {
    options?: ValueData[];
    selectedValue?: any;
    onselect?: (event?: any) => void;
  }

  let {
    options = [],
    selectedValue = $bindable(null),
    onselect = () => null,
  }: RadioGroupProps = $props();

  const onSelect = (item: any) => {
    if (selectedValue.value === item.value) return;
    selectedValue = item.value;
    onselect?.({ value: item });
  };
</script>

<div class="radio-group">
  {#each options as value}
    <RadioButton
      value={value.value}
      label={value.label}
      selected={value.value === selectedValue}
      onselect={() => onSelect(value)}
    />
  {/each}
</div>

<style lang="scss">
  .radio-group {
    display: flex;
    flex-direction: row;
  }
</style>
