import { THREE, Vector3Min } from "$lib/three";
import { isNextStepReady, lerp, lerpOutCubic, type RenderAnimation } from "$src/data/animation";
import DefaultAnimation from "$src/animation/default";
import { CHANGE_TYPE } from "$src/data/enums/app";
import { MODEL_TYPE } from "$src/data/enums/model";
import { OUTFIT_TYPE } from "$src/data/outfit";

const LAYER_CHANGE_TYPES = [
  CHANGE_TYPE.LAYER_ADD,
  CHANGE_TYPE.LAYER_DOWN,
  CHANGE_TYPE.LAYER_UP,
  CHANGE_TYPE.LAYER_REMOVE,
];

const CreatePropertyStep = function (
  data: any,
  part: any,
  property: "position" | "rotation",
  value: "x" | "y" | "z",
  targetValue: any,
  duration: any,
  ease: "direct" | "ease" = "ease",
  clock: number
) {
  const partData = data[part];
  if (partData == undefined) return;

  const targetCache = (data.__stepTargetCache ??= {} as any);
  const targetKey = `${part}.${property}.${value}`;
  const currentSmoothedTarget = targetCache[targetKey] ?? partData[property][value];
  const targetBlend = Math.min(1, Math.max(0, clock * 18));
  const smoothedTarget = lerp(currentSmoothedTarget, targetValue, targetBlend);
  targetCache[targetKey] = smoothedTarget;

  if (ease == "ease") {
    partData[property][value] = lerpOutCubic(clock, partData[property][value], smoothedTarget, duration);
  } else {
    partData[property][value] = smoothedTarget;
  }
};

export class AnimationPropertyStep {
  part: string;
  property: "position" | "rotation";
  value: "x" | "y" | "z";
  targetValue: number;
  duration: number;
  ease: "direct" | "ease" = "ease";
  constructor(
    part: string,
    property: "position" | "rotation",
    value: "x" | "y" | "z",
    targetValue: number,
    duration: number,
    ease: "direct" | "ease" = "ease"
  ) {
    this.part = part;
    this.property = property;
    this.value = value;
    this.targetValue = targetValue;
    this.duration = duration;
    this.ease = ease;
  }
}
export class AnimationStepState {
  name: string;
  step: AnimationPropertyStep[];
  onFinished: any;
  epsilon = 0.003;
  constructor(name: string, step: AnimationPropertyStep[], onFinishedMth: any, epsilon = 0.003) {
    this.name = name;
    this.step = step;
    this.onFinished = onFinishedMth;
    this.epsilon = epsilon;
  }
}

export const GetAnimationForPackageChange = function (type: CHANGE_TYPE, outfitType: OUTFIT_TYPE | null = null): RenderAnimation | null {
  if (type == CHANGE_TYPE.MODEL_TYPE_CHANGE) return DefaultAnimation;
  if (LAYER_CHANGE_TYPES.includes(type)) return GetAnimationForType(outfitType) ?? null;
  if (type == CHANGE_TYPE.PACKAGE_IMPORT) return DefaultAnimation;
  if (type == CHANGE_TYPE.SHARE) return DefaultAnimation;
  if (type == CHANGE_TYPE.DOWNLOAD) return DefaultAnimation;
  if (type == CHANGE_TYPE.SKIN_SET) return DefaultAnimation;
  return null;
};

export const GetAnimationForType = function (type: string | null): RenderAnimation | null {
  const random = Math.random();
  switch (type) {
    case OUTFIT_TYPE.HAT:
      return DefaultAnimation;
    case OUTFIT_TYPE.TOP:
    case OUTFIT_TYPE.HOODIE:
      return DefaultAnimation;
    case OUTFIT_TYPE.SHOES:
      return random < 0.5 ? DefaultAnimation : DefaultAnimation;
    case OUTFIT_TYPE.BOTTOM:
      return random < 0.5 ? DefaultAnimation : DefaultAnimation;
    default:
      return null;
  }
};

export const AnimationStep = function (data: any, props: AnimationPropertyStep[], clock: number, epsilon = 0.003) {
  props.forEach((prop) => {
    CreatePropertyStep(data, prop.part, prop.property, prop.value, prop.targetValue, prop.duration, prop.ease, clock);
  });
  return isNextStepReady(
    props.map((prop) => {
      if (data[prop.part] == undefined) return { value: 0, target: 0 };
      return { value: data[prop.part][prop.property][prop.value], target: prop.targetValue };
    }),
    epsilon
  );
};

export const AnimationStepManager = function (data: any, steps: AnimationStepState[], startState: string) {
  const findStep = (name: string) => steps.find((step) => step.name == name) as AnimationStepState | undefined;
  let currentStep = findStep(startState);

  return {
    currentStep,
    run: (clock: number) => {
      if (currentStep == undefined) return true;
      if (AnimationStep(data, currentStep.step, clock, currentStep.epsilon)) {
        currentStep = findStep(currentStep.onFinished());
      }
      return currentStep == undefined;
    },
  };
};
