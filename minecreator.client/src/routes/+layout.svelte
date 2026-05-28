<script lang="ts">
  import { onMount } from "svelte";
  import { IS_MOBILE_VIEW, Setup } from "$data/global";
  import { Initialize } from "$src/data/static";
  import { initializeI18n, setAppLocale, SUPPORTED_LOCALES } from "$src/i18n";
  import favicon from "$lib/assets/favicon.svg";
  import Select from "$lib/components/base/Select/Select.svelte";
  import { ValueData } from "$src/helpers/dataHelper";
  import { _, locale } from "svelte-i18n";
  let { children } = $props();

  let currentLocale = $state<string>("en");
  const languageOptions = $derived(
    SUPPORTED_LOCALES.map(
      (code) => new ValueData(code, $_(`options.language.${code}`)),
    ),
  );
  onMount(() => {
    initializeI18n();
    Setup();
    Initialize();
  });

  const changeLocale = (payload: { item: ValueData }) => {
    currentLocale = payload.item.value;
    setAppLocale(payload.item.value);
  };
  $effect(() => {
    const activeLocale = $locale ?? "en";
    if (currentLocale !== activeLocale) currentLocale = activeLocale;
  });
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>
<div id="container" class:mobile={$IS_MOBILE_VIEW}>
  <div id="lang-select">
    <div>
      <Select
        items={languageOptions}
        selectedItem={currentLocale}
        itemText="label"
        itemValue="value"
        placeholder={$_("common.select")}
        onselect={changeLocale}
      />
    </div>
  </div>
  <div>
    {@render children()}
  </div>
</div>

<style lang="scss">
  #container {
    margin: 0 auto;
    padding: 12px;
    box-sizing: border-box;
    max-width: 1200px;
    width: 90%;
    &.mobile {
      margin: 0px;
      width: 100%;
    }
    #lang-select {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      width: 100%;
      gap: 12px;

    }
  }
</style>
