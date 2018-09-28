const resx = require('resx');
import { readFileSync } from 'fs';

export class ResourceParser {
    private static cache: Map<string, Map<string, string>> = new Map();

    public static getResource(resxPath: string): Promise<Map<string, string>> {
        if (ResourceParser.cache.has(resxPath)) {
            return Promise.resolve(ResourceParser.cache.get(resxPath) || new Map());
        }

        return new Promise((resolve, reject) => {
            resx.resx2js(readFileSync(resxPath, { encoding: 'utf8'}), (err: Error, res: Map<string, string>) => {
                if (err) {
                    reject(err);
                } else {
                    ResourceParser.cache.set(resxPath, res);
                    resolve(res);
                }
            });
        });
    }

    public static async get(resxPath: string, name: string): Promise<string> {
        const resources = await ResourceParser.getResource(resxPath);
        if (resources.has(name)) {
            return resources.get(name) || '';
        }
        return Promise.reject(new Error(`Key '${name}' not found in ${resxPath}`));
    }

    private readonly _resxPath: string;

    constructor(resxPath: string) {
        this._resxPath = resxPath;
    }

    public get(name: string): Promise<string> {
        return ResourceParser.get(this._resxPath, name);
    }
}