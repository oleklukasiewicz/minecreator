import { RenderAnimation } from "./animation";
import { ALEX_MODEL, STEVE_MODEL } from "./consts/model";
import { DEFAULT_RENDERER } from "./static";
import type { OutfitLayer, OutfitPackage } from "$data/models/package";
import type { OutfitPackageRenderConfig } from "$data/models/render";
import { MODEL_TYPE } from "./enums/model";
export class CameraConfig {
  rotation: any;
  position: any;
  lookAt: any;
  lookAtEnabled: boolean;
  fov: number;
  zoom: number;
  constructor(
    position: Vector3Min = new Vector3Min(0, 0, -1),
    lookAt: Vector3Min = new Vector3Min(0, 0, 0),
    zoom: number = 0.95,
    fov: number = 1,
    lookAtEnabled: boolean = false,
    rotation: Vector3Min = new Vector3Min(0, 0, 0)
  ) {
    this.fov = fov;
    this.position = position;
    this.lookAt = lookAt;
    this.lookAtEnabled = lookAtEnabled;
    this.rotation = rotation;
    this.zoom = zoom;
  }
}
import capeModelData from "$src/playerModel/cape.gltf?raw";
import { THREE, Vector3Min } from "$lib/three";
export class TextureRender {
  private renderer: any;
  private model: any;
  private texture: string | null = null;
  private node: any;
  private cameraOptions: any;
  private modelScene: ModelScene | null = null;
  private textureLoader: any | null = null;
  private loadedTexture: any | null = null;
  private renderingActive: boolean = false;
  private shadowsEnabled: boolean = false;
  private shadowScene: any | null = null;
  private floorScene: boolean | null = null;
  private temporaryRenderNode: any | null = null;
  private capeTexture: string | null = null;
  private capeScene: any | null = null;
  private capePivot: any | null = null;
  private renderingPaused: boolean = false;
  private ambientLight: any | null = null;
  private directionalLight: any | null = null;
  //for dynamic render
  private animationEngine: RenderAnimationEngine | null = null;
  private orbitalControls: any | null = null;
  private _loadTexture = async (targetTexture: string | null = null) => {
    return new Promise<any>(async (resolve) => {
      const threeModule = await THREE.getThree();
      if (this.textureLoader == null) {
        this.textureLoader = new threeModule.ImageBitmapLoader();
        this.textureLoader.setOptions({ imageOrientation: "flipY" });
      }
      this.textureLoader.load(
        targetTexture,
        (texture: any) => {
          const canvasTexture = new threeModule.CanvasTexture(texture);
          resolve(canvasTexture);
        },
        undefined,
        (error: any) => {
          // On error, resolve with null to prevent hanging promise
          console.warn("Texture load failed:", error);
          resolve(null);
        }
      );
    });
  };
  private _attachCapeToModel = async () => {
    if (this.modelScene == null) return;
    const bodyPart = this.modelScene.renderScene.getObjectByName("Body");
    if (this.capePivot != null) {
      bodyPart.remove(this.capePivot);
      this.capePivot = null;
    }
    const threeModule = await THREE.getThree();
    const pivot = new threeModule.Object3D();
    pivot.position.set(0, -0.02, 0);
    this.capePivot = pivot;
    if (this.capeScene != null) pivot.add(this.capeScene);
    bodyPart.add(pivot);
  };
  private _loadModelToTempScene = async (model: string) => {
    return new Promise<any>(async (resolve) => {
      const module = await THREE.getGLTFLoader();
      const modelLoader = new module.GLTFLoader();
      modelLoader.load(model, (gltf: any) => {
        resolve(gltf.scene);
      });
    });
  };
  private _applyTextureToModel = async () => {
    if (this.loadedTexture == null || this.modelScene == null) return;
    const threeModule = await THREE.getThree();
    this.loadedTexture.magFilter = threeModule.NearestFilter;
    this.loadedTexture.minFilter = threeModule.LinearMipmapLinearFilter;
    this.loadedTexture.wrapS = threeModule.RepeatWrapping;
    this.loadedTexture.wrapT = threeModule.RepeatWrapping;

    this.modelScene.renderScene.traverse((child: any) => {
      if (child.isMesh && child.name != "Cape") {
        if (this.shadowsEnabled) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
        const mat = child.material;
        mat.map = this.loadedTexture;
        mat.roughness = 1;
      }
    });
  };
  private _loadCameraOptions = async () => {
    if (this.modelScene == null) return;
    let options = new CameraConfig();
    const threeModule = await THREE.getThree();
    if (!this.renderingActive) {
      options = this.cameraOptions || new CameraConfig();
    } else {
      options = new CameraConfig(
        new threeModule.Vector3(0, 0, -2),
        undefined,
        undefined,
        75
      );
    }
    options.position = new threeModule.Vector3(
      options.position.x,
      options.position.y,
      options.position.z
    );
    options.lookAt = new threeModule.Vector3(
      options.lookAt.x,
      options.lookAt.y,
      options.lookAt.z
    );
    options.rotation = new threeModule.Vector3(
      options.rotation.x,
      options.rotation.y,
      options.rotation.z
    );
    this.modelScene.camera.position.x = options.position.x;
    this.modelScene.camera.position.y = options.position.y;
    this.modelScene.camera.position.z = options.position.z;
    this.modelScene.camera.rotation.x = options.rotation.x;
    this.modelScene.camera.rotation.y = options.rotation.y;
    this.modelScene.camera.rotation.z = options.rotation.z;
    this.modelScene.camera.lookAt(options.lookAt);
    this.modelScene.camera.fov = options.fov;
    this.modelScene.camera.zoom = options.zoom;
  };
  private _render = async (_self = this) => {
    if (!_self.renderingActive) return;
    if (this.animationEngine == null) {
      this.animationEngine = new RenderAnimationEngine();
      await this.animationEngine.Create();
    }
    if (!this.renderingPaused) {
      //frame rendering
      await this.animationEngine!.PrepareAnimation(
        this.modelScene!.renderScene,
        this.modelScene!.name,
        false
      );
      await this.animationEngine!.RenderAnimationFrame();

      this.renderer.render(this.modelScene!.scene, this.modelScene!.camera);
      (this.node as any).appendChild(this.renderer.domElement);
    }
    requestAnimationFrame(async () => await this._render(this));
  };
  private _updateRenderSize = () => {
    if (this.modelScene?.camera == null) return;
    const canvas = this.node;
    const width = canvas.clientWidth != 0 ? canvas.clientWidth : 300;
    const height = canvas.clientHeight != 0 ? canvas.clientHeight : 300;
    const canvasSizeMultiplier = 1;
    const fov = 1;
    if (this.renderer == null) return;

    this.renderer.setPixelRatio(window.devicePixelRatio);
    this.renderer.setSize(
      width * fov * canvasSizeMultiplier,
      height * fov * canvasSizeMultiplier
    );
    this.modelScene.camera.aspect = width / height;
    this.modelScene.camera.updateProjectionMatrix();
  };
  private _renderInNode = () => {
    if (this.loadedTexture == null) return;
    const renderNode = this.temporaryRenderNode || this.node;
    if (this.renderer == null || this.node == null || renderNode == null) return;

    const canAttachCanvas =
      renderNode.nodeType === Node.ELEMENT_NODE && renderNode.tagName !== "IMG";
    const shouldAttach =
      canAttachCanvas && this.renderer.domElement.parentNode !== renderNode;

    if (shouldAttach) {
      renderNode.appendChild(this.renderer.domElement);
    }

    this.renderer.render(this.modelScene!.scene, this.modelScene!.camera);

    const dataUrl = this.renderer.domElement.toDataURL();
    if ("src" in (this.node as any)) (this.node as any).src = dataUrl;

    if (shouldAttach && this.renderer.domElement.parentNode === renderNode) {
      renderNode.removeChild(this.renderer.domElement);
    }
  };

  constructor(renderer: any = DEFAULT_RENDERER) {
    this.renderer = renderer;
  }
  SetModelScene = async (newModelScene: ModelScene): Promise<TextureRender> => {
    const targetSceneModel = newModelScene;
    const newRenderScene = targetSceneModel.scene.children.find(
      (x: any) => x.name == "model"
    );
    if (this.modelScene == null) {
      this.modelScene = targetSceneModel;
      this.modelScene.scene = targetSceneModel.scene;
      this.modelScene.renderScene = newRenderScene;
    } else {
      this.modelScene!.scene.remove(this.modelScene!.renderScene);
      this.modelScene!.scene.add(newRenderScene);
      this.modelScene!.renderScene = newRenderScene;
    }
    this.modelScene.name = targetSceneModel.name;
    if (this.renderingActive) await this._applyTextureToModel();
    if (this.capeTexture != null) {
      await this._attachCapeToModel();
    }
    if (this.renderingActive && this.animationEngine != null)
      await this.animationEngine.RefreshAnimationData(
        this.modelScene!.renderScene,
        this.modelScene!.name
      );
    return this;
  };
  PauseRendering = (): TextureRender => {
    this.renderingPaused = true;
    return this;
  };
  ResumeRendering = (): TextureRender => {
    this.renderingPaused = false;
    return this;
  };
  SetTextureAsync = async (texture: string | null): Promise<TextureRender> => {
    this.texture = texture;
    if (texture != null) {
      this.loadedTexture = await this._loadTexture(texture);
    }
    if (this.renderingActive) {
      await this._applyTextureToModel();
    }
    if (this.renderingActive === false) {
      // For static rendering, refresh the frame
      await this.RenderStatic();
    }
    return this;
  };
  GetTexture = (): string | null => {
    return this.texture;
  };
  SetRenderer = (renderer: any): TextureRender => {
    this.renderer = renderer;
    return this;
  };
  SetNode = (node: any): TextureRender => {
    this.node = node;
    return this;
  };
  SetTemporaryRenderNode = (node: any): TextureRender => {
    this.temporaryRenderNode = node;
    return this;
  };
  RemoveTemporaryRenderNode = (): TextureRender => {
    this.temporaryRenderNode = null;
    return this;
  };
  SetCameraOptions = async (
    cameraOptions: CameraConfig | string
  ): Promise<TextureRender> => {
    if (typeof cameraOptions != "string") {
      const threeModule = await THREE.getThree();
      cameraOptions.position = new threeModule.Vector3(
        cameraOptions.position.x,
        cameraOptions.position.y,
        cameraOptions.position.z
      );
      cameraOptions.lookAt = new threeModule.Vector3(
        cameraOptions.lookAt.x,
        cameraOptions.lookAt.y,
        cameraOptions.lookAt.z
      );
    }
    this.cameraOptions = cameraOptions;
    return this;
  };
  AddAnimation = (animation: RenderAnimation, force: boolean): TextureRender => {
    if (this.animationEngine == null) {
      this.animationEngine = new RenderAnimationEngine();
      this.animationEngine.Create();
    }
    this.animationEngine.AddAnimation(animation, force);
    return this;
  };
  SetAnimationsLimit = (limit: number): TextureRender => {
    //this.animationEngine.animationQueueLimit = limit;
    return this;
  };
  RenderStatic = async (): Promise<TextureRender> => {
    //do it in one paint call
    requestAnimationFrame(async () => {
      if (this.renderer == null) return this;

      await this._loadCameraOptions();
      this._updateRenderSize();
      await this._applyTextureToModel();

      this._renderInNode();
    });
    return this;
  };
  RenderDynamic = async (): Promise<TextureRender> => {
    this.StopRendering();
    //initial configuration
    const threeModule = await THREE.getThree();
    this.modelScene!.camera = new threeModule.PerspectiveCamera();

    this.renderingActive = true;

    await this._loadCameraOptions();
    this._updateRenderSize();
    await this._applyTextureToModel();

    const module = await THREE.getOrbitalControls();
    this.orbitalControls = new module.OrbitControls(
      this.modelScene!.camera,
      this.renderer.domElement
    );
    this.orbitalControls.enablePan = false;
    this.orbitalControls.maxDistance = 3.0;
    this.orbitalControls.minDistance = 0.75;

    this._render();
    return this;
  };
  StopRendering = (): TextureRender => {
    this.renderingActive = false;
    return this;
  };
  AddShadow = async (): Promise<TextureRender> => {
    if (this.shadowScene != null) return this;
    this.shadowsEnabled = true;

    const threeModule = await THREE.getThree();
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = threeModule.PCFShadowMap;

    const pointLight = new threeModule.DirectionalLight(0xffffff, 0.78);

    pointLight.castShadow = true;
    pointLight.shadow.camera.near = 0.5;
    pointLight.shadow.camera.far = 500;
    pointLight.shadow.camera.left = -0.8;
    pointLight.shadow.camera.right = 0.8;
    pointLight.shadow.camera.top = 0.8;
    pointLight.shadow.camera.bottom = -0.8;
    pointLight.shadow.bias = -0.00001;
    pointLight.shadow.radius = 1;
    const mapSize = 1024;
    pointLight.shadow.mapSize.width = mapSize;
    pointLight.shadow.mapSize.height = mapSize;

    pointLight.target.position.set(0, 1, 0);
    pointLight.position.set(0, 10, -10);
    pointLight.shadowCameraVisible = true;
    this.modelScene!.scene.add(pointLight);
    this.shadowScene = pointLight;

    return this;
  };
  RemoveShadow = (): TextureRender => {
    this.shadowsEnabled = false;
    if (this.shadowScene != null && this.modelScene != null) this.modelScene.scene.remove(this.shadowScene);
    this.shadowScene = null;
    return this;
  };
  AddFloor = async (texture: string): Promise<TextureRender> => {
    if (this.floorScene != null) return this;

    const threeModule = await THREE.getThree();
    const floorTexture = new threeModule.TextureLoader().load(texture);
    const floorGeometry = new threeModule.PlaneGeometry(3, 3, 3, 3);
    const floorMaterial = new threeModule.MeshStandardMaterial({
      map: floorTexture,
      side: threeModule.DoubleSide,
      roughness: 1,
    });
    const floor = new threeModule.Mesh(floorGeometry, floorMaterial);
    floor.rotation.x = Math.PI / 2;
    floor.position.y = 0;
    floor.receiveShadow = true;
    this.modelScene!.scene.add(floor);
    this.floorScene = floor;
    return this;
  };
  RemoveFloor = (): TextureRender => {
    if (this.floorScene != null && this.modelScene != null) this.modelScene.scene.remove(this.floorScene as any);
    this.floorScene = null;
    return this;
  };
  SetBackground = (color: number): TextureRender => {
    if (this.renderer == null) return this;
    this.renderer.setClearColor(color, 1);
    return this;
  };
  RemoveBackground = async (): Promise<TextureRender> => {
    if (this.renderer == null) return this;

    const threeModule = await THREE.getThree();
    this.renderer.setClearColor(new threeModule.Color(0x000000), 0);
    return this;
  };
  SetCapeAsync = async (capeTexture: string): Promise<TextureRender> => {
    this.capeTexture = capeTexture;
    if (this.capeScene != null && this.modelScene != null) this.modelScene.scene.remove(this.capeScene);
    const threeModule = await THREE.getThree();
    const loaderPromise: Promise<any> = new Promise((resolve) => {
      const capeLoader = new threeModule.ImageBitmapLoader();
      capeLoader.load(capeTexture, (texture: any) => {
        const canvasTexture = new threeModule.CanvasTexture(texture);
        canvasTexture.magFilter = threeModule.NearestFilter;
        canvasTexture.minFilter = threeModule.NearestFilter;
        resolve(canvasTexture);
      });
    });

    const capeTxt = await loaderPromise;

    const capeModel = await this._loadModelToTempScene(
      "data:model/gltf+json;base64," + btoa(capeModelData)
    );
    const material = new threeModule.MeshStandardMaterial({
      map: capeTxt,
    });
    capeModel.traverse((child: any) => {
      child.name = "Cape";
      if (child.isMesh) {
        if (this.shadowsEnabled) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
        child.material = material;
        child.material.roughness = 1;
      }
    });
    this.capeScene = capeModel;
    await this._attachCapeToModel();
    if (this.renderingActive && this.animationEngine != null)
      await this.animationEngine.RefreshAnimationData(
        this.modelScene!.renderScene,
        this.modelScene!.name
      );

    return this;
  };
  RemoveCape = (): TextureRender => {
    this.capeTexture = null;
    if (this.modelScene != null) {
      const bodypart = this.modelScene.renderScene.getObjectByName("Body");
      if (bodypart && this.capePivot) bodypart.remove(this.capePivot);
    }
    this.capePivot = null;
    this.capeScene = null;
    return this;
  };
  AddAmbientLight = async (): Promise<void> => {
    const threeModule = await THREE.getThree();
    const brightness = 1.2;
    const light = new threeModule.AmbientLight(0xffffff, brightness * 1.8, 10);

    //configure light
    this.modelScene!.scene.add(light);
    this.ambientLight = light;
  };
  AddDirectionalLight = async (): Promise<TextureRender> => {
    const threeModule = await THREE.getThree();
    const brightness = 1.2;
    const pointLight = new threeModule.DirectionalLight(
      0xffffff,
      brightness * 0.65,
      10
    );
    pointLight.position.set(0, 50, -50);
    this.modelScene!.scene.add(pointLight);
    this.directionalLight = pointLight;
    return this;
  };
  RemoveDirectionalLight = (): TextureRender => {
    if (this.directionalLight != null) this.modelScene!.scene.remove(this.directionalLight);
    this.directionalLight = null;
    return this;
  };
  RemoveAmbientLight = (): TextureRender => {
    if (this.ambientLight != null) this.modelScene!.scene.remove(this.ambientLight);
    this.ambientLight = null;
    return this;
  };
  AddTextureAmbientLighting = async (): Promise<TextureRender> => {
    const threeModule = await THREE.getThree();
    const ambient = new threeModule.AmbientLight(0xffffff, 1.2);
    this.modelScene!.scene.add(ambient);
    this.ambientLight = ambient;
    return this;
  };
  async Resize() {
    if (this.renderer == null) return this;
    await this._loadCameraOptions();
    this._updateRenderSize();

    this._renderInNode();
    return this;
  }
}
export class ModelScene {
  model: string;
  name: string;
  camera: any;
  scene: any;
  renderScene: any;
  constructor(model: string, name: string) {
    this.model = model;
    this.name = name;
  }
  async Create() {
    //prepare scene

    const threeModule = await THREE.getThree();
    this.scene = new threeModule.Scene();
    this.camera = new threeModule.OrthographicCamera();
    const module = await THREE.getGLTFLoader();
    const modelLoader = new module.GLTFLoader();

    //set default values
    this.scene.position.y = -1;

    //load model
    this.renderScene = await new Promise((resolve) => {
      modelLoader.load(this.model, (gltf: any) => {
        gltf.scene.name = "model";
        this.scene.add(gltf.scene);
        resolve(gltf.scene);
      });
    });
    return this;
  }
  ResetPosition() {
    const renderScene: any = this.renderScene;
    renderScene.getObjectByName("RightArm").rotation.set(0, 0, 0);
    renderScene.getObjectByName("LeftArm").rotation.set(0, 0, 0);
    renderScene.getObjectByName("RightLeg").rotation.set(0, 0, 0);
    renderScene.getObjectByName("LeftLeg").rotation.set(0, 0, 0);
    renderScene.getObjectByName("Head").rotation.set(0, 0, 0);
    return this;
  }
  Clone() {
    const cloned = Object.assign({}, this);
    cloned.scene = this.scene.clone(true);
    cloned.renderScene = cloned.scene.children.find((x: any) => x.name == "model");
    cloned.renderScene?.traverse((child: any) => {
      if (child.isMesh) {
        const mat = child.material;
        child.material = mat.clone();
      }
    });
    return cloned;
  }
}
export class RenderAnimationEngine {
  private animationsList: RenderAnimation[] = [];
  private currentAnimation: RenderAnimation | null = null;
  private animationData: any | null = null;
  private animationDataModelName?: string;

  private isAnimationPrepared: boolean = false;
  private isAnimationQuiting: boolean = false;

  private animationQueueLimit: number = 2;

  private clock: any | null = null;

  constructor() {}
  Create = async () => {
    const threeModule = await THREE.getThree();
    this.clock = new threeModule.Clock();
    return this;
  };
  PrepareAnimation = async (sceneData: any, modelName: string, keepData: boolean, force = false) => {
    if (this.isAnimationPrepared && !force) return this;

    if (this.currentAnimation == null) {
      if (this.animationsList.length > 0) {
        this.currentAnimation = this.animationsList[0];
        this.animationsList.splice(0, 1);
      }
    }
    if (this.currentAnimation == null) return this;

    const localAnimationData = await this.currentAnimation.prepare(
      sceneData,
      keepData,
      modelName
    );

    this.animationDataModelName = modelName;
    this.animationData = Object.assign(
      this.animationData || {},
      localAnimationData
    );

    this.isAnimationPrepared = true;
    return this;
  };
  AddAnimation = (animation: RenderAnimation, force = false) => {
    if (this.animationsList.length >= this.animationQueueLimit) {
      if (!force) return this;
      this.animationsList.splice(this.animationsList.length - 1, 1, animation);
    }
    this.animationsList.push(animation);
    return this;
  };
  RenderAnimationFrame = async () => {
    if (this.clock == null) return this;
    const _clockActualDelta = this.clock.getDelta();
    const _clockDelta = _clockActualDelta > 1 ? 1 : _clockActualDelta;
    const _clockElapsedTime = this.clock.getElapsedTime();

    if (!this.isAnimationPrepared) {
      await this.PrepareAnimation(
        this.animationData,
        this.animationDataModelName || "",
        false
      );
    }
    if (this.currentAnimation == null) return this;

    if (this.animationsList.length > 0) this.isAnimationQuiting = true;
    if (this.isAnimationQuiting) {
      const isAnimationFinishedQuiting = this.currentAnimation.stop(
        this.animationData,
        null,
        _clockDelta,
        this.animationDataModelName,
        _clockElapsedTime
      );
      if (isAnimationFinishedQuiting) {
        this.isAnimationQuiting = false;
        this.isAnimationPrepared = false;
        this.animationData = null;
        this.currentAnimation = null;
      }
    } else {
      this.currentAnimation.render(
        this.animationData,
        null,
        _clockDelta,
        this.animationDataModelName,
        _clockElapsedTime
      );
    }
  };
  RefreshAnimationData = async (sceneData: any, modelName: string) => {
    if (this.currentAnimation == null) return this;
    const localAnimationData = await this.currentAnimation.prepare(
      sceneData,
      true,
      modelName
    );
    this.animationData = Object.assign(
      this.animationData || {},
      localAnimationData
    );
    this.animationDataModelName = modelName;
  };
}
export class OutfitPackageToTextureConverter {
  private outfitPackage: OutfitPackage | null = null;
  private model: string | null = null;
  private modelMap: any = null;
  private isMerging: boolean = false;
  private isFlatten: boolean = false;
  private layerId: string | null = null;
  private texture: string | null = null;
  private basetexture: string | null = null;
  private excludedPartsFromFlat: string[] = ["head"];

  constructor() {}
  SetOutfitPackage = (outfitPackage: OutfitPackage): OutfitPackageToTextureConverter => {
    this.outfitPackage = outfitPackage;
    return this;
  };
  SetBaseTexture = (basetexture: string): OutfitPackageToTextureConverter => {
    this.basetexture = basetexture;
    return this;
  };
  SetOptions = (options: OutfitPackageRenderConfig): OutfitPackageToTextureConverter => {
    this.outfitPackage = options.item;
    if (options.baseTexture == null) {
      this.basetexture = null;
    } else if (typeof options.baseTexture === "string") {
      this.basetexture = options.baseTexture;
    } else {
      const modelKey = options.item.model as any;
      const bt = (options.baseTexture as any)[modelKey];
      this.basetexture = bt?.content ?? null;
    }
    this.layerId = options.selectedLayerId ?? null;
    this.model = options.item.model;
    this.isFlatten = options.isFlatten ?? false;
    this.excludedPartsFromFlat = options.excludedPartsFromFlat ?? ["head"];
    this.modelMap = options.item.model == MODEL_TYPE.STEVE ? STEVE_MODEL : ALEX_MODEL;
    return this;
  };
  SetExcludedPartsFromFlat = (excludedPartsFromFlat: string[]): OutfitPackageToTextureConverter => {
    this.excludedPartsFromFlat = excludedPartsFromFlat;
    return this;
  };
  SetModel = (model: MODEL_TYPE): OutfitPackageToTextureConverter => {
    this.model = model;
    this.modelMap = model == MODEL_TYPE.STEVE ? STEVE_MODEL : ALEX_MODEL;
    return this;
  };
  SetLayerId = (layerId: string): OutfitPackageToTextureConverter => {
    this.layerId = layerId;
    return this;
  };
  SetAsFlatten = (): OutfitPackageToTextureConverter => {
    this.isFlatten = true;
    return this;
  };
  SetAsNotFlatten = (): OutfitPackageToTextureConverter => {
    this.isFlatten = false;
    return this;
  };
  AsFlattenAsync = async (): Promise<string | null> => {
    this.isFlatten = true;
    const modelMap = this.modelMap;
    const ctx = document.createElement("canvas").getContext("2d", {
      willReadFrequently: true,
    }) as CanvasRenderingContext2D;
    if (this.texture == null) return null;
    //create image
    var Image = window.Image;
    var img = new Image();
    //await for img to load in promise (handlers must be attached before src)
    const loadedImg: any = await new Promise((resolve) => {
      img.onload = () => {
        resolve({ img: img });
      };
      img.onerror = () => {
        resolve({ img: null });
      };
      img.src = this.texture as string;
    });
    if (loadedImg?.img == null) {
      return this.texture;
    }
    //get canvas size
    ctx.canvas.width = loadedImg.img.width;
    ctx.canvas.height = loadedImg.img.height;

    ctx.drawImage(loadedImg.img, 0, 0);
    //flattening
    try {
      const modelMapKeys = Object.keys(modelMap);
      for (let i = 0; i < modelMapKeys.length; i++) {
        let part = modelMap[modelMapKeys[i]];
        if (
          part.outerTextureArea != null &&
          part.textureArea != null &&
          !this.excludedPartsFromFlat.includes(part.name)
        )
          flatPart(ctx, part);
      }
      this.texture = ctx.canvas.toDataURL();
    } catch {
      // Keep original texture if flatten fails
    }
    return this.texture;
  };
  AsNotFlatten = (): string | null => {
    this.isFlatten = false;
    return this.texture;
  };
  GetTexture = (): string | null => {
    return this.texture;
  };
  GetModelMap = (): any => {
    return this.modelMap;
  };
  GetModel = (): string | null => {
    return this.model;
  };
  GetLayerId = (): string | null => {
    return this.layerId;
  };
  GetOutfitPackage = (): OutfitPackage | null => {
    return this.outfitPackage;
  };
  ConvertAsync = async (): Promise<string | null> => {
    //set modelMap
    const modelMap = this.modelMap;

    const getLayerContent = (layer: OutfitLayer, model: MODEL_TYPE) => {
      const primary = layer?.[model];
      if (primary?.content != null) return primary.content;
      if (primary?.contentSnapshot != null) return primary.contentSnapshot;

      const fallbackModel = model == MODEL_TYPE.ALEX
        ? MODEL_TYPE.STEVE
        : MODEL_TYPE.ALEX;
      const fallback = layer?.[fallbackModel];
      if (fallback?.content != null) return fallback.content;
      if (fallback?.contentSnapshot != null) return fallback.contentSnapshot;

      return null;
    };

    //load target layers
    const layers: string[] = [];
    if (this.basetexture != null) layers.push(this.basetexture);
    if (this.layerId == null || this.layerId == "") {
      //load all layers
      this.outfitPackage!.layers.forEach((layer: OutfitLayer) => {
        const content = getLayerContent(layer, this.model as MODEL_TYPE);
        if (content != null) layers.push(content);
      });
    } else {
      //load single
      const layer: OutfitLayer | undefined = this.outfitPackage!.layers.find(
          (x: any) => x.id == this.layerId
        );
      if (layer != null) {
        const content = getLayerContent(layer, this.model as MODEL_TYPE);
        if (content != null) layers.push(content);
      }
    }
    if (layers.length == 0) {
      this.texture = null;
      return null;
    }
    let texture: string | null = layers[0];
    if (layers.length > 1) {
      //merge all layers
      texture = await mergeTextures(layers, modelMap);
    }
    this.texture = texture;
    return texture;
  };
  ConvertAsyncWithFlattenSettingsAsync = async (): Promise<string | null> => {
    const texture = await this.ConvertAsync();
    if (texture == null) return null;
    if (this.isFlatten) await this.AsFlattenAsync();
    else await this.AsNotFlatten();
    return this.texture;
  };
  ConvertFromOptionsAsync = async (
    options: OutfitPackageRenderConfig
  ): Promise<string | null> => {
    this.SetOptions(options);
    return await this.ConvertAsyncWithFlattenSettingsAsync();
  };
}
const mergeTextures = async function (textures: string[], modelMap: any): Promise<string | null> {
  type LoadedTexture = {
    textureSrc: string;
    img: HTMLImageElement | null;
  };

  const canvas = document.createElement("canvas");
  const windowImage = window.Image;

  //load textures to canvas
  const layerPromises: Promise<LoadedTexture>[] = textures.map((texture) => {
    return new Promise<LoadedTexture>((resolve) => {
      const img = new windowImage();
      img.onload = () => {
        return resolve({ textureSrc: texture, img: img });
      };
      img.onerror = () => {
        return resolve({ textureSrc: texture, img: null });
      };
      img.src = texture;
    });
  });

  const ctx = canvas.getContext("2d", { willReadFrequently: true }) as CanvasRenderingContext2D | null;
  const tempCanvas = document.createElement("canvas");
  const tempCtx = tempCanvas.getContext("2d", { willReadFrequently: true }) as CanvasRenderingContext2D | null;

  const images = await Promise.all(layerPromises);
  const validImages = images.filter((image): image is LoadedTexture & { img: HTMLImageElement } => image.img != null);

  if (validImages.length === 0) return null;

  //get canvas size
    const getSize = (dim: "width" | "height") => {
      return Math.max(
        ...validImages.map((image) => image.img[dim])
      );
    };

  canvas.width = getSize("width");
  canvas.height = getSize("height");

  //draw images
  validImages.forEach((image) => {
    if (!ctx || !tempCtx) return;
    ctx.globalAlpha = 1;
    tempCtx.drawImage(image.img, 0, 0);
    //replace lower parts based on modelMap
    const modelMapKeys = Object.keys(modelMap);
      for (let i = 0; i < modelMapKeys.length; i++) {
      let part = modelMap[modelMapKeys[i]];
      if (part.outerTextureArea != null && part.textureArea != null)
        replaceLowerLayerPart(tempCtx, ctx, part);
    }

    tempCtx.clearRect(0, 0, tempCanvas.width, tempCanvas.height);

    ctx.drawImage(image.img, 0, 0);
  });

  try {
    return canvas.toDataURL();
  } catch {
    // If canvas fails, fallback to first valid texture
    return validImages[0]?.textureSrc ?? null;
  }
};
const replaceLowerLayerPart = function (imgContext: CanvasRenderingContext2D, lowerLayerContext: CanvasRenderingContext2D, part: any) {
  const imageData = imgContext.getImageData(
    part.textureArea.x,
    part.textureArea.y,
    part.textureArea.width,
    part.textureArea.height
  );
  const sourcePixels = imageData.data;
  const destData = lowerLayerContext.getImageData(
    part.outerTextureArea.x,
    part.outerTextureArea.y,
    part.outerTextureArea.width,
    part.outerTextureArea.height
  );
  const destPixels = destData.data;
  for (let i = 0; i < sourcePixels.length; i += 4) {
    const r = sourcePixels[i];
    const g = sourcePixels[i + 1];
    const b = sourcePixels[i + 2];
    //detect if is not empty
    if (r != 0 || g != 0 || b != 0) {
      destPixels[i] = 0;
      destPixels[i + 1] = 0;
      destPixels[i + 2] = 0;
      destPixels[i + 3] = 0;
    }
  }
  lowerLayerContext.putImageData(
    destData,
    part.outerTextureArea.x,
    part.outerTextureArea.y
  );
};
const flatPart = function (imgContext: CanvasRenderingContext2D, part: any) {
  const imageData = imgContext.getImageData(
    part.textureArea.x,
    part.textureArea.y,
    part.textureArea.width,
    part.textureArea.height
  );
  const outerImageData = imgContext.getImageData(
    part.outerTextureArea.x,
    part.outerTextureArea.y,
    part.outerTextureArea.width,
    part.outerTextureArea.height
  );
  const sourcePixels = outerImageData.data;
  const destPixels = imageData.data;
  for (let i = 0; i < sourcePixels.length; i += 4) {
    const r = sourcePixels[i];
    const g = sourcePixels[i + 1];
    const b = sourcePixels[i + 2];
    //detect if is not empty
    if (r != 0 || g != 0 || b != 0) {
      destPixels[i] = r;
      destPixels[i + 1] = g;
      destPixels[i + 2] = b;
      destPixels[i + 3] = 255;
      //clear outer
      sourcePixels[i] = 0;
      sourcePixels[i + 1] = 0;
      sourcePixels[i + 2] = 0;
      sourcePixels[i + 3] = 0;
    }
  }
  imgContext.putImageData(
    outerImageData,
    part.outerTextureArea.x,
    part.outerTextureArea.y
  );
  imgContext.putImageData(imageData, part.textureArea.x, part.textureArea.y);
};
