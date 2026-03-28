<script lang="ts">
  let { label }: { label: string } = $props();
</script>

<span class="section-title">
  {label}
</span>

<style lang="scss">
  .section-title {
    font-size: small;
    color: var(--color-theme-D6);
    font-family: minecraft;
    margin: 0;
    display: table;
    user-select: none;
  }
</style>
