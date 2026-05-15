let three: any | null = null;
let orbitalControls: any | null = null;
let gltfLoader: any | null = null;

export const THREE = {
  getThree: async (): Promise<any> => {
    if (three == null) {
      three = await import("three");
    }
    return three;
  },
  getOrbitalControls: async (): Promise<any> => {
    if (orbitalControls == null) {
      orbitalControls = await import(
        "three/examples/jsm/controls/OrbitControls.js"
      );
    }
    return orbitalControls;
  },
  getGLTFLoader: async (): Promise<any> => {
    if (gltfLoader == null) {
      gltfLoader = await import("three/examples/jsm/loaders/GLTFLoader.js");
    }
    return gltfLoader;
  },
};
export class Vector3Min {
  x: number;
  y: number;
  z: number;
  constructor(x: number, y: number, z: number) {
    this.x = x;
    this.y = y;
    this.z = z;
  }
}