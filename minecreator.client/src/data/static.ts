import { writable, readonly, type Readable, type Writable } from "svelte/store";
import { ModelScene } from "./render";
import { ALEX_MODEL, STEVE_MODEL } from "./consts/model";
import { THREE } from "$lib/three";

// steve modelscene
const steveModelSceneWritable: Writable<ModelScene | null> = writable(null);
export const STEVE_MODELSCENE: Readable<ModelScene | null> = readonly(steveModelSceneWritable);
// alex modelscene
const alexModelSceneWritable: Writable<ModelScene | null> = writable(null);
export const ALEX_MODELSCENE: Readable<ModelScene | null> = readonly(alexModelSceneWritable);

const steveModelSceneBaseWritable: Writable<ModelScene | null> = writable(null);
export const STEVE_MODELSCENE_BASE: Readable<ModelScene | null> = readonly(steveModelSceneBaseWritable);
const alexModelSceneBaseWritable: Writable<ModelScene | null> = writable(null);
export const ALEX_MODELSCENE_BASE: Readable<ModelScene | null> = readonly(alexModelSceneBaseWritable);

// default renderer
const defaultRendererWritable: Writable<any | null> = writable(null);
export const DEFAULT_RENDERER: Readable<any | null> = readonly(defaultRendererWritable);

// initialize only what's required for the render component
export const Initialize = async function () {
  alexModelSceneWritable.set(await new ModelScene(ALEX_MODEL.model, ALEX_MODEL.name).Create());
  steveModelSceneWritable.set(await new ModelScene(STEVE_MODEL.model, STEVE_MODEL.name).Create());
  alexModelSceneBaseWritable.set(
    await new ModelScene(ALEX_MODEL.model, ALEX_MODEL.name).Create().then((x) => x.ResetPosition())
  );
  steveModelSceneBaseWritable.set(
    await new ModelScene(STEVE_MODEL.model, STEVE_MODEL.name).Create().then((x) => x.ResetPosition())
  );

  const threeModule = await THREE.getThree();
  defaultRendererWritable.update(() => {
    const renderer = new threeModule.WebGLRenderer({ alpha: true });
    // set color space if available
    try {
      renderer.outputColorSpace = threeModule.LinearSRGBColorSpace;
    } catch {}
    return renderer;
  });
};
