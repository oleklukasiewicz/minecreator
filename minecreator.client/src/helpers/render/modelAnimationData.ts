import { THREE, Vector3Min } from "$lib/three";
import { MODEL_TYPE } from "$src/data/enums/model";

export const CreatePivotPart = async function (
  basePart: any,
  targetPart: any,
  pivotPosition: Vector3Min,
  partPosition: Vector3Min = new Vector3Min(0, 0, 0),
  showAxis = false
) {
  const threeModule = await THREE.getThree();
  // Preserve world transform: compute world position/quaternion/scale of target,
  // create pivot at that world position projected into basePart local space,
  // add pivot to basePart, then attach target to pivot preserving world transform.
  const worldPos = new threeModule.Vector3();
  targetPart.getWorldPosition(worldPos);
  const worldQuat = new threeModule.Quaternion();
  targetPart.getWorldQuaternion(worldQuat);
  const worldScale = new threeModule.Vector3();
  targetPart.getWorldScale(worldScale);

  const pivot = new threeModule.Object3D();
  // position pivot relative to basePart (at target's world position)
  const pivotLocal = worldPos.clone();
  basePart.worldToLocal(pivotLocal);
  pivot.position.copy(pivotLocal);

  // add pivot to basePart (now pivot has correct world transform)
  basePart.add(pivot);

  // attach target to pivot while preserving world transform
  try {
    pivot.attach(targetPart);
  } catch (e) {
    // fallback: manual reparent keeping local transform approximation
    try {
      if (targetPart.parent) targetPart.parent.remove(targetPart);
    } catch {}
    const localPos = worldPos.clone();
    pivot.worldToLocal(localPos);
    targetPart.position.copy(localPos);
    targetPart.quaternion.copy(worldQuat);
    targetPart.scale.copy(worldScale);
    pivot.add(targetPart);
  }

  // optional partPosition is intentionally ignored to preserve original world placement

  if (showAxis) {
    const axisHelper = new threeModule.AxesHelper(5);
    pivot.add(axisHelper);
  }

  return { part: targetPart, pivot };
};

export const CreateModelAnimationData = async function (
  scene: any,
  modelName: any,
  debug = false
) {
  const data = {
    body: scene.getObjectByName("Body"),
    head: scene.getObjectByName("Head"),
    leftarm: scene.getObjectByName("LeftArm"),
    rightarm: scene.getObjectByName("RightArm"),
    leftleg: scene.getObjectByName("LeftLeg"),
    rightleg: scene.getObjectByName("RightLeg"),
    leftArmPivot: null,
    rightArmPivot: null,
    leftLegPivot: null,
    rightLegPivot: null,
    headPivot: null,
    bodyPivot: null,
    cape: scene.getObjectByName("Cape"),
  } as any;

  const threeModule = await THREE.getThree();
  const la = await CreatePivotPart(
    data.body,
    data.leftarm,
    modelName == MODEL_TYPE.STEVE
      ? new threeModule.Vector3(-0.31, -0.125, 0)
      : new threeModule.Vector3(-0.31, -0.16, 0),
    undefined,
    debug
  );
  data.leftArmPivot = la.pivot;

  const ra = await CreatePivotPart(
    data.body,
    data.rightarm,
    modelName == MODEL_TYPE.STEVE
      ? new threeModule.Vector3(0.31, -0.125, 0)
      : new threeModule.Vector3(0.31, -0.16, 0),
    undefined,
    debug
  );
  data.rightArmPivot = ra.pivot;

  const ll = await CreatePivotPart(
    data.body,
    data.leftleg,
    new threeModule.Vector3(-0.125, -0.75, 0),
    new threeModule.Vector3(0, 0, 0),
    debug
  );
  data.leftLegPivot = ll.pivot;

  const rl = await CreatePivotPart(
    data.body,
    data.rightleg,
    new threeModule.Vector3(0.125, -0.75, 0),
    new threeModule.Vector3(0, 0, 0),
    debug
  );
  data.rightLegPivot = rl.pivot;

  const hp = await CreatePivotPart(
    data.body,
    data.head,
    new threeModule.Vector3(0, 0.75, 0),
    new threeModule.Vector3(0, 0.75, 0),
    debug
  );
  data.headPivot = hp.pivot;
  // Use body object directly as bodyPivot to avoid nesting body inside itself
  data.bodyPivot = data.body;

  return data;
};
