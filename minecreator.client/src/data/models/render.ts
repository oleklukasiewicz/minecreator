import type { MODEL_TYPE } from "../enums/model";
import type { OutfitPackage } from "./package";

export interface OutfitPackageRenderConfig {
  item: OutfitPackage;
  baseTexture?: string | Record<string, any> | null;
  selectedLayerId?: string | null;
  isFlatten?: boolean;
  excludedPartsFromFlat?: string[];
}

export class ModelTextureArea {
  public constructor(
    public x: number,
    public y: number,
    public width: number,
    public height: number
  ) {}
}
export class ModelPart {
  public constructor(
    public name: string,
    public textureArea: ModelTextureArea,
    public outerTextureArea: ModelTextureArea
  ) {}
}
export class ModelMap {
  public constructor(
    public name: string,
    public model: string,
    public head: ModelPart,
    public body: ModelPart,
    public leftLeg: ModelPart,
    public rightLeg: ModelPart,
    public leftArm: ModelPart,
    public rightArm: ModelPart
  ) {}
}
