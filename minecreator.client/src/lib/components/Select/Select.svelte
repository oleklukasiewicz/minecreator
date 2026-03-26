<script lang="ts">
  //main imports
  //services
  import { clickOutside } from "$src/helpers/componentHelper";
  //components
  import Button from "../Button/Button.svelte";
  //consts
  import { IS_MOBILE_VIEW } from "$src/data/global";
  //icons
  import ChevronUpIcon from "$icons/chevron-up.svg?raw";
  import ChevronDownIcon from "$icons/chevron-down.svg?raw";
  import CloseIcon from "$icons/close.svg?raw";
  import CheckBoxIcon from "$icons/checkbox.svg?raw";
  import CheckBoxOffIcon from "$icons/checkbox-off.svg?raw";
  import Flyout from "../Flyout/Flyout.svelte";

  interface Props {
    items?: any[];
    placeholder?: string;
    multiple?: boolean;
    selectedItem?: any;
    clickable?: boolean;
    opened?: boolean;
    itemText?: string | null;
    itemValue?: string | null;
    clearable?: boolean;
    dropDownStyle?: any;
    disabled?: boolean;
    defaultValue?: any;
    sorter?: (a: any, b: any) => number;
    comparer?: (
      selectedItemValue: any,
      item: any,
      isMultiple?: boolean,
    ) => boolean;
    actions?: any;
    itemSnippet?: (props: any) => any;
    onselect?: (payload: { item: any }) => void;
    onselectedclick?: (payload: { item: any }) => void;
    onclear?: (payload: { item: any }) => void;
  }

  let {
    items = [],
    placeholder = "Select",
    multiple = false,
    selectedItem = $bindable(null),
    clickable = false,
    opened = false,
    itemText = null,
    itemValue = null,
    clearable = false,
    dropDownStyle = null,
    disabled = false,
    defaultValue = null,
    sorter = function (a, b) {
      if (a < b) return -1;
      if (a > b) return 1;
      return 0;
    },
    comparer = function (selectedItemValue, item, isMultiple = false) {
      const isSameItem = (a: any, b: any) => {
        if (itemValue) {
          const aValue =
            a && typeof a === "object" && !Array.isArray(a) ? a[itemValue] : a;
          const bValue =
            b && typeof b === "object" && !Array.isArray(b) ? b[itemValue] : b;
          return aValue == bValue;
        }
        return a == b;
      };

      if (isMultiple) {
        if (!Array.isArray(selectedItemValue)) return false;
        return selectedItemValue.some((selected: any) => isSameItem(selected, item));
      }
      return isSameItem(selectedItemValue, item);
    },
    actions,
    itemSnippet,
    onselect,
    onselectedclick,
    onclear,
  }: Props = $props();

  let selectedItemValue = $state<any>(null);
  let menuWidth = 0;
  let menu = $state<HTMLDivElement | null>(null);
  let itemsContainer: HTMLDivElement | null = null;
  let inputComponent = $state<HTMLInputElement | null>(null);
  let filteredItems = $state<any[]>([]);
  let sortedFilteredItems = $derived([...filteredItems].sort(sorter));
  let focusedIndex = $state(-1);

  const isSameItem = (a: any, b: any) => {
    if (itemValue) {
      const aValue =
        a && typeof a === "object" && !Array.isArray(a) ? a[itemValue] : a;
      const bValue =
        b && typeof b === "object" && !Array.isArray(b) ? b[itemValue] : b;
      return aValue == bValue;
    }
    return a == b;
  };

  const select = (item: any) => {
    if (multiple) {
      const currentSelection = Array.isArray(selectedItemValue)
        ? selectedItemValue
        : selectedItemValue == null
          ? []
          : [selectedItemValue];

      if (currentSelection.some((selected: any) => isSameItem(selected, item))) {
        selectedItemValue = currentSelection.filter(
          (selected: any) => !isSameItem(selected, item),
        );
      } else {
        selectedItemValue = [...currentSelection, item];
      }
    } else {
      selectedItemValue = item;
    }

    if (itemValue) {
      if (multiple)
        selectedItem = selectedItemValue.map(
          (i: { [x: string]: any }) => i[itemValue],
        );
      else selectedItem = selectedItemValue[itemValue];
    } else selectedItem = selectedItemValue;
    if (!multiple) opened = false;
    onselect?.({ item: selectedItemValue });
  };
  const selectedClick = () => {
    if (clickable) {
      onselectedclick?.({ item: selectedItemValue });
    } else {
      opened = true;
    }
  };
  const clear = () => {
    if (multiple) selectedItemValue = [];
    else selectedItemValue = defaultValue;

    if (itemValue) {
      selectedItem = selectedItemValue;
    } else selectedItem = selectedItemValue;
    filteredItems = items;
    onclear?.({ item: selectedItemValue });
  };
  let setSelectedItemValue = (value: any) => {
    if (multiple) {
      const selectedValues = Array.isArray(value)
        ? value
        : value == null
          ? []
          : [value];

      if (itemValue) {
        selectedItemValue = items.filter((i) =>
          selectedValues.some((v) =>
            v && typeof v === "object" && !Array.isArray(v)
              ? i[itemValue] == v[itemValue]
              : i[itemValue] == v,
          ),
        );
      } else {
        selectedItemValue = selectedValues;
      }
      return;
    }

    if (itemValue) {
      selectedItemValue =
        items.find(
          (i) =>
            i[itemValue] == value ||
            (value && typeof value === "object" && !Array.isArray(value)
              ? i[itemValue] == value[itemValue]
              : false),
        ) ?? null;
    } else selectedItemValue = value;
  };

  const handleKeyDown = (event: { key: any; preventDefault: () => void }) => {
    if (!opened) return;

    switch (event.key) {
      case "ArrowDown":
        event.preventDefault();
        focusedIndex = (focusedIndex + 1) % filteredItems.length;
        scrollToFocusedItem();
        break;
      case "ArrowUp":
        event.preventDefault();
        focusedIndex =
          (focusedIndex - 1 + filteredItems.length) % filteredItems.length;
        scrollToFocusedItem();
        break;
      case "Enter":
        event.preventDefault();
        if (focusedIndex >= 0 && focusedIndex < filteredItems.length) {
          select(filteredItems[focusedIndex]);
        }
        break;
      case "Escape":
        event.preventDefault();
        opened = false;
        break;
    }
  };
  const scrollToFocusedItem = () => {
    const focusedItem = itemsContainer?.querySelectorAll(".item")[focusedIndex];
    focusedItem?.scrollIntoView({ block: "nearest" });
  };

  $effect(() => {
    setSelectedItemValue(selectedItem);
  });

  $effect(() => {
    filteredItems = items;
  });
</script>

<!-- svelte-ignore a11y_no_noninteractive_tabindex -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
  class="select"
  class:opened
  class:disabled
  class:mobile={$IS_MOBILE_VIEW}
  bind:this={menu}
  use:clickOutside={() => (opened = false)}
  tabindex="0"
  onkeydown={handleKeyDown}
>
  <!-- svelte-ignore a11y_click_events_have_key_events -->
  <div class="selected-item-container">
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <!-- svelte-ignore a11y_no_static_element_interactions -->
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <div class="selected-item" onclick={selectedClick}>
      {#if selectedItemValue != null && (multiple ? selectedItemValue?.length > 0 : true)}
        {#if clickable && selectedItemValue != null}
          <Button
            textAlign="left"
            size="small"
            type={clickable ? "primary" : "quaternary"}
          >
            {itemText == null
              ? selectedItemValue
              : selectedItemValue[itemText]}</Button
          >
        {:else}
          <div class="selected-item-default">
            {#if selectedItemValue != null || selectedItemValue.length > 0}
              {multiple == false
                ? itemText == null
                  ? selectedItemValue
                  : selectedItemValue[itemText]
                : itemText == null
                  ? selectedItemValue
                  : selectedItemValue
                      .map((i: { [x: string]: any }) => i[itemText])
                      .join(", ")}
            {/if}
          </div>
        {/if}
      {:else}
        <div class="select-placeholder">{placeholder}</div>
      {/if}
    </div>
    {#if clearable && selectedItemValue != null && (multiple ? selectedItemValue.length > 0 : true)}
      <Button
        onlyIcon
        style="height: 30px;"
        icon={CloseIcon}
        size="small"
        type="secondary"
        iconSize="auto"
        noBorder
        onclick={clear}
      ></Button>
    {/if}
    <Button
      onlyIcon
      style="height: 30px;width:30px;"
      iconSize="auto"
      size="small"
      icon={opened ? ChevronUpIcon : ChevronDownIcon}
      type="primary"
      noBorder
      onclick={() => (opened = !opened)}
    ></Button>
    {#if actions}{@render actions()}{/if}
  </div>
  <!-- svelte-ignore a11y_click_events_have_key_events -->
  <!-- svelte-ignore a11y_no_static_element_interactions -->
  <Flyout bind:opened caller={menu} preventClickOutsideClose resizable>
    {#snippet children({ position })}
      <div
        class:pos-bottom={position == "bottom"}
        class:pos-top={position == "top"}
        class="items"
        style={dropDownStyle}
        class:opened
        class:hidden={!opened}
        bind:this={itemsContainer}
      >
        {#each sortedFilteredItems as item, index}
          <div class="selected item" onclick={() => select(item)}>
            {#if itemSnippet}
              {@render itemSnippet({
                item,
                multiple,
                itemText,
                selectedItemValue,
                comparer,
                index,
                focusedIndex,
              })}
            {:else}
              <Button
                size="small"
                flat
                noBorder
                type={comparer(selectedItemValue, item, multiple) ||
                index == focusedIndex
                  ? "primary"
                  : "quaternary"}
                icon={multiple
                  ? comparer(selectedItemValue, item, multiple)
                    ? CheckBoxIcon
                    : CheckBoxOffIcon
                  : null}
                focused={index == focusedIndex}
                label={itemText == null ? item : item[itemText]}
                textAlign="left"
              ></Button>
            {/if}
          </div>
        {/each}
      </div>
    {/snippet}
  </Flyout>
</div>

<style lang="scss">
  @use "Select.scss";
</style>
