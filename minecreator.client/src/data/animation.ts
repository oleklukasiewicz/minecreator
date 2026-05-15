export class RenderAnimation {
  prepare: (sceneData: any, keepData: boolean, modelName: string) => Promise<any>;
  render: (animationData: any, arg: any, delta: number, modelName?: string, elapsedTime?: number) => void;
  stop: (animationData: any, arg: any, delta: number, modelName?: string, elapsedTime?: number) => boolean;
  constructor(
    prepare: (sceneData: any, keepData: boolean, modelName: string) => Promise<any>,
    render: (animationData: any, arg: any, delta: number, modelName?: string, elapsedTime?: number) => void,
    stop: (animationData: any, arg: any, delta: number, modelName?: string, elapsedTime?: number) => boolean
  ) {
    this.prepare = prepare;
    this.render = render;
    this.stop = stop;
  }
}
export function lerp(start: number, end: number, factor: number): number {
  return (1 - factor) * start + factor * end;
}
export function easeOutCubic(t: number): number {
  return (1 - Math.pow(1 - t, 3));
}
const clamp01 = (value: number) => Math.min(1, Math.max(0, value));
export function lerpOutCubic(clock: number, prop: number, target: number, speed: number): number {
  // Clamp interpolation to avoid overshoot/jitter on unstable frame deltas.
  const normalizedClock = Math.max(0, clock);
  const interpolationFactor = clamp01(speed * (normalizedClock * 130));
  return lerp(prop, target, easeOutCubic(interpolationFactor));
}
export function isPoseReady(poses: { value: number; target: number }[], epsilon = 0.003): boolean {
  let isPoseReady = true;
  poses.forEach((pose) => {
    if (pose.value + pose.target * -1 > epsilon) {
      isPoseReady = false;
    }
  });
  return isPoseReady;
}
export function isNextStepReady(poses: { value: number; target: number }[], epsilon = 0.003): boolean {
  let isPoseReady = true;
  poses.forEach((pose) => {
    if (Math.abs(pose.value - pose.target) > epsilon) {
      isPoseReady = false;
    }
  });
  return isPoseReady;
}