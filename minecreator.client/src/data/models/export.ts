import type { Outfit } from "../outfit";

export class ExportModel {
  model: string;
  gameVersion: string;
  outfits: Outfit[];

  constructor(model: string, gameVersion: string, outfits: Outfit[]) {
    this.model = model;
    this.gameVersion = gameVersion;
    this.outfits = outfits;
  }
}
