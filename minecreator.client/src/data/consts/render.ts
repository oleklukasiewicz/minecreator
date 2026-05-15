import { Vector3Min } from "$lib/three";
import { CameraConfig } from "$src/data/render";
const angle = 0.5958;

const cameraMap: Record<string, CameraConfig> = {
  set: new CameraConfig(
    new Vector3Min(-1, 0 + angle, -1),
    new Vector3Min(0, 0, 0),
    0.905
  ),
  top: new CameraConfig(
    new Vector3Min(-1, 0.1 + angle, -1),
    new Vector3Min(0, 0.1, 0),
    1.5
  ),
  bottom: new CameraConfig(
    new Vector3Min(-1, -0.54 + angle, -1),
    new Vector3Min(0, -0.54, 0),
    1.5
  ),
  shoes: new CameraConfig(
    new Vector3Min(-1, -0.9 + angle, -1),
    new Vector3Min(0, -0.9, 0),
    2
  ),
  hat: new CameraConfig(
    new Vector3Min(-1, 0.8 + angle, -1),
    new Vector3Min(0, 0.8, 0),
    1.7
  ),
  hoodie: new CameraConfig(
    new Vector3Min(-1, 0.45 + angle, -1),
    new Vector3Min(0, 0.45, 0),
    1.3
  ),
  suit: new CameraConfig(
    new Vector3Min(-1, 0 + angle, -1),
    new Vector3Min(0, 0, 0),
    0.85
  ),
};

export const CAMERA_CONFIG = {
  getForOutfit: (outfitType: string): CameraConfig => {
    return cameraMap[outfitType] ?? new CameraConfig();
  },
  ...cameraMap,
};
