export class AppConfig {
    maxColorCount: number;
    maxPalleteSize: number;
    maxSamplesCount: number;

    constructor(maxColorCount = 0, maxPalleteSize = 0, maxSamplesCount = 0) {
        this.maxColorCount = maxColorCount;
        this.maxPalleteSize = maxPalleteSize;
        this.maxSamplesCount = maxSamplesCount;
    }
}

export class ModuleConfig {
    name: string;
    accessory: string[];
    styles: string[];
    constructor(name = "", accessory: string[] = [], styles: string[] = []) {
        this.name = name;
        this.accessory = accessory;
        this.styles = styles;
    }
}

export class Configuration {
    appConfig: AppConfig;
    modulesConfig: ModuleConfig[];

    constructor() {
        this.appConfig = new AppConfig();
        this.modulesConfig = [];
    }
}