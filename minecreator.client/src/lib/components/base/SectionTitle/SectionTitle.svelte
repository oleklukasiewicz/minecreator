<script lang="ts">
  let { label,style }: { label: string; style?: string } = $props();
</script>

<span class="section-title" style={style}>
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
