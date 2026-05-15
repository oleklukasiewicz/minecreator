<script lang="ts">
  //consts
  import { CAMERA_CONFIG } from "$src/data/consts/render";

  //main imports
  import { onDestroy, onMount } from "svelte";
  import { get, type Readable } from "svelte/store";
  import IntersectionObserver from "svelte-intersection-observer";
  //services
  import {
    CameraConfig,
    ModelScene,
    OutfitPackageToTextureConverter,
    TextureRender,
  } from "$src/data/render";
  import type { RenderAnimation } from "$src/data/animation";
  //consts
  import {
    ALEX_MODELSCENE,
    ALEX_MODELSCENE_BASE,
    DEFAULT_RENDERER,
    STEVE_MODELSCENE,
    STEVE_MODELSCENE_BASE,
  } from "$src/data/static";
  //models
  //components
  import Resize from "../other/Resize/Resize.svelte";
  //types/values
  import { MODEL_TYPE } from "$src/data/enums/model";
  import { OUTFIT_TYPE } from "$src/data/outfit";
  type OutfitLayer = any;
  //icons
  import floorTexture from "$texture/floor.webp?url";

  export const addAnimation = function (animation: RenderAnimation, force = false): void {
    if (textureRenderer == null) return;
    textureRenderer.AddAnimation(animation, force);
  };
  export const resize = async function () {
    if (textureRenderer == null) return;
    await textureRenderer.Resize();
  };
  export const getCurrentTexture = function (): string | null | undefined {
    return textureRenderer?.GetTexture();
  };
  interface OutfitPackageRenderProps {
    source: string;
    model?: MODEL_TYPE | "source";
    outfitType?: string | null;
    isDynamic?: boolean;
    isFlatten?: boolean;
    layerId?: string;
    cameraOptions?: CameraConfig | "auto";
    renderer?: Readable<any> | any | null;
    cape?: string | null;
    baseTexture?: OutfitLayer | string | null;
    pauseOnIntersection?: boolean;
    useTextureLighting?: boolean;
    resizable?: boolean;
    resizeDebounce?: number;
    ontextureUpdate?: (event?: { detail: { texture: string | null | undefined } }) => void | null;
  }

  let {
    source,
    model = MODEL_TYPE.STEVE,
    outfitType = null,
    isDynamic = false,
    isFlatten = false,
    layerId = "",
    cameraOptions = "auto",
    renderer = $DEFAULT_RENDERER,
    cape = null,
    baseTexture = null,
    pauseOnIntersection = false,
    useTextureLighting = false,
    resizable = false,
    resizeDebounce = 300,
    ontextureUpdate = undefined
  }: OutfitPackageRenderProps = $props();

  let _component: HTMLElement | null = $state<HTMLElement | null>(null);

  let _source: string | null = null;
  let _model: MODEL_TYPE | "source" = MODEL_TYPE.STEVE;
  let _isFlatten: boolean = false;
  let _baseTexture: OutfitLayer | string | null = null;
  let _layerId: string = "";
  let _cape: string | null = null;
  let renderReady = $state<boolean>(false);
  let cachedtexture: string | null = null;
  let renderNode: HTMLImageElement | HTMLDivElement | null = $state<HTMLImageElement | HTMLDivElement | null>(null);
  let merger: OutfitPackageToTextureConverter = new OutfitPackageToTextureConverter();
  let textureRenderer: TextureRender;
  let initialized = false;

  const safeClone = <T>(value: T): T => {
    if (value == null) return value;
    if (typeof value !== "object") return value;

    try {
      return structuredClone(value);
    } catch {
      // Fallback for non-cloneable values (e.g. functions/proxies inside objects).
      try {
        return JSON.parse(JSON.stringify(value)) as T;
      } catch {
        return value;
      }
    }
  };

  onMount(async () => {
    _source = safeClone(source);
    _model = model;
    _isFlatten = isFlatten;
    _baseTexture = baseTexture;
    _layerId = layerId;
    _cape = cape;
    const initialRenderer =
      renderer ?? (await waitForStoreValue(DEFAULT_RENDERER as Readable<any>));
    textureRenderer = new TextureRender(initialRenderer);

    textureRenderer.SetNode(renderNode);
    if (!isDynamic) textureRenderer.SetTemporaryRenderNode(_component);

    await loadInitialParams();
    await setRenderMode(isDynamic);
    initialized = true;
    renderReady = isDynamic;
    if (_cape != null) await textureRenderer.SetCapeAsync(_cape);
  });
  onDestroy(() => {
    textureRenderer.RemoveTemporaryRenderNode();
    textureRenderer.StopRendering();
  });

  const onTextureUpdate = function () {
    ontextureUpdate?.({ detail: { texture: textureRenderer.GetTexture() } });
  };

  const setRenderMode = async (v: boolean) => {
    textureRenderer.RemoveAmbientLight().RemoveDirectionalLight();
    if (useTextureLighting) {
      await textureRenderer.AddTextureAmbientLighting();
    } else {
      await textureRenderer.AddAmbientLight();
      await textureRenderer.AddDirectionalLight();
    }
    if (v) {
      await textureRenderer.AddFloor(floorTexture);
      await textureRenderer.AddShadow();
      await textureRenderer.SetBackground(0x202020);
      await textureRenderer.RenderDynamic();
    } else {
      textureRenderer.RemoveFloor().RemoveShadow();

      await textureRenderer.RemoveBackground();
      textureRenderer.StopRendering();
      await textureRenderer.RenderStatic();
    }
  };

  const loadInitialParams = async function () {
    if (_source == null || _source == "") return;
    // Working with string-only source now
    await syncModel(_model == "source" ? MODEL_TYPE.STEVE : (_model as MODEL_TYPE));

    cachedtexture = _source as string;

    if (cameraOptions == "auto") {
      const options = CAMERA_CONFIG.getForOutfit(String(outfitType ?? ""));
      await textureRenderer.SetCameraOptions(options);
    }
    if (cachedtexture != null) await textureRenderer.SetTextureAsync(cachedtexture);
  };
  const setSource = async (
    v: string,
    oldModel: MODEL_TYPE | null,
    newModel: MODEL_TYPE,
    oldLayerId: string | null,
    newLayerId: string | null,
    oldCape: string | null,
    newCape: string | null
  ) => {
    if (!initialized) return;
    if (v == null || v == "") return;

    _source = safeClone(v);
    cachedtexture = _source as string;

    await textureRenderer.SetCameraOptions(CAMERA_CONFIG.getForOutfit(String(outfitType ?? "")));

    if (cachedtexture != null) await textureRenderer.SetTextureAsync(cachedtexture);
    if (!isDynamic) await textureRenderer.RenderStatic();
    renderReady = true;
    onTextureUpdate();
  };
  const setModel = async (v: MODEL_TYPE | "source") => {
    if (!initialized) return false;
    if (_model == v && v != "source") return false;
    _model = v;

    await syncModel(v);
    return true;
  };
  const setOutfitType = async (v: string) => {
    if (!initialized) return;
    await setCameraOptions(v);
  };
  const setFlatten = async (v: boolean) => {
    if (!initialized) return;
    if (v == _isFlatten) return;
    _isFlatten = v;
    // Flattening not supported when source is string-only; no-op
  };
  const setLayerId = async (v: string) => {
    if (!initialized) return;
    if (v == _layerId) return false;
    _layerId = v;
    return true;
  };
  const setCameraOptions = async (v: string | CameraConfig) => {
    if (!initialized) return;

    let targetCameraOptions = cameraOptions;
    if (cameraOptions == "auto") {
      targetCameraOptions = CAMERA_CONFIG.getForOutfit(String(outfitType ?? ""));
    }
    await textureRenderer.SetCameraOptions(targetCameraOptions);

    if (!isDynamic) await textureRenderer.RenderStatic();
  };
  const setBaseTexture = async (v: string | OutfitLayer | null) => {
    if (!initialized) return;
    if (v == null) return;
    // Base texture not supported for string-only source; no-op
  };
  const setCape = async (v: string | null) => {
    if (!initialized) return;
    if (v == _cape) return;
    _cape = v;
    if (v != null) await textureRenderer.SetCapeAsync(v);
    else textureRenderer.RemoveCape();
  };
  const baseModelTypesList = [
    OUTFIT_TYPE.SUIT,
    OUTFIT_TYPE.SHOES,
    OUTFIT_TYPE.BOTTOM,
  ];

  async function waitForStoreValue<T>(
    store: Readable<T>,
    timeoutMs = 5000
  ): Promise<T> {
    const value = get(store);
    if (value != null) return value;

    return new Promise<T>((resolve) => {
      let timeout: ReturnType<typeof setTimeout> | null = null;
      const unsubscribe = store.subscribe((nextValue) => {
        if (nextValue == null) return;
        if (timeout != null) clearTimeout(timeout);
        unsubscribe();
        resolve(nextValue);
      });

      timeout = setTimeout(() => {
        unsubscribe();
        resolve(get(store));
      }, timeoutMs);
    });
  }

  const resolveModelScene = async (
    modelToSync: MODEL_TYPE,
    useBaseScene: boolean
  ): Promise<ModelScene> => {
    const targetStore = useBaseScene
      ? modelToSync === MODEL_TYPE.ALEX
        ? ALEX_MODELSCENE_BASE
        : STEVE_MODELSCENE_BASE
      : modelToSync === MODEL_TYPE.ALEX
        ? ALEX_MODELSCENE
        : STEVE_MODELSCENE;

    return await waitForStoreValue(targetStore as Readable<ModelScene>);
  };

  const syncModel = async (modelToSync: MODEL_TYPE | "source") => {
    if (modelToSync == "source") modelToSync = MODEL_TYPE.STEVE;
    merger.SetModel(modelToSync as MODEL_TYPE);

    const useBaseScene = baseModelTypesList.includes(outfitType as OUTFIT_TYPE);

    const modelScene = await resolveModelScene(modelToSync as MODEL_TYPE, useBaseScene);
    if (modelScene == null) {
      console.warn("Unable to resolve model scene in syncModel", {
        modelToSync,
        useBaseScene,
      });
      return;
    }

    await textureRenderer.SetModelScene(modelScene.Clone());
  };
  const syncModelSource = async function (vModel: MODEL_TYPE | "source", vSource: string, vLayerId: string, vCape: string | null) {
    const oldModel = merger.GetModel() as MODEL_TYPE;
    await setModel(vModel);
    const newModel = merger.GetModel() as MODEL_TYPE;
    const oldLayerId = _layerId;
    await setLayerId(vLayerId);
    const newLayerId = _layerId;
    const oldCape = _cape;
    await setCape(vCape);
    const newCape = vCape;
    await setSource(
      vSource,
      oldModel,
      newModel,
      oldLayerId,
      newLayerId,
      oldCape,
      newCape
    );
  };

  const isReRenderNeeded = function (
    aSource: string,
    bSource: string,
    oldModel: MODEL_TYPE,
    newModel: MODEL_TYPE,
    oldLayerId: string | null,
    newLayerId: string | null
  ) {
    if (oldLayerId != newLayerId) return true;
    return aSource !== bSource;
  };
  const isLayersChanged = function (aSource: string, bSource: string) {
    return aSource !== bSource;
  };

  $effect(() => {
    syncModelSource(model, source, layerId, cape);
  });
  $effect(() => {
    setBaseTexture(baseTexture);
  });
  $effect(() => {
    setOutfitType(String(outfitType ?? ""));
  });

  $effect(() => {
    setFlatten(isFlatten);
  });
  $effect(() => {
    setCameraOptions(cameraOptions);
  });

  const onResize = async function () {
    if (!initialized) return;
    await textureRenderer.Resize();
    if (isDynamic) renderReady = true;
  };
  const onObserve = function (e: { detail: { isIntersecting: any; }; }) {
    if (pauseOnIntersection) {
      if (!e.detail.isIntersecting) textureRenderer.PauseRendering();
      else textureRenderer.ResumeRendering();
    } else textureRenderer.ResumeRendering();
  };
</script>

<div class="outfit-render" bind:this={_component}>
  {#if !isDynamic}
    <!-- svelte-ignore a11y_missing_attribute -->
    <img
      bind:this={renderNode}
      class:renderReady
      onload={() => (renderReady = true)}
      onerror={() => (renderReady = false)}
      draggable="false"
      fetchpriority="low"
      loading="lazy"
    />
  {:else}
    <IntersectionObserver element={_component} on:observe={onObserve}>
      <div bind:this={renderNode}></div></IntersectionObserver
    >
  {/if}
  {#if resizable}
    <Resize
      onresize={onResize}
      debounce={resizeDebounce}
      targetNode={_component}
    ></Resize>
  {/if}
</div>

<style lang="scss">
  .outfit-render {
    aspect-ratio: 1;
    width: 100%;
    height: 100%;
    display: flex;
    div {
      width: 100%;
      height: 100%;
    }
    img {
      width: 100%;
      height: 100%;
      opacity: 0;
      &.renderReady {
        opacity: 1;
      }
    }
  }
</style>
