// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

const resx = require("resx");
import { readFileSync } from "fs";

export class ResourceParser {

    public static getResource(resxPath: string): Promise<Map<string, string>> {
        const result = ResourceParser.cache.get(resxPath);
        if (result) {
            return Promise.resolve(result);
        }

        return new Promise((resolve, reject) => {
            resx.resx2js(readFileSync(resxPath, { encoding: "utf8"}), (err: Error, res: Map<string, string>) => {
                if (err) {
                    reject(err);
                } else {
                    const result = new Map(Object.entries(res));
                    ResourceParser.cache.set(resxPath, result);
                    resolve(result);
                }
            });
        });
    }

    public static async get(resxPath: string, name: string): Promise<string> {
        const resources: Map<string, string> = await ResourceParser.getResource(resxPath);
        if (resources.has(name)) {
            return resources.get(name) || "";
        }
        return Promise.reject(new Error(`Key '${name}' not found in ${resxPath}`));
    }
    private static cache: Map<string, Map<string, string>> = new Map();

    private readonly _resxPath: string;

    constructor(resxPath: string) {
        this._resxPath = resxPath;
    }

    public get(name: string): Promise<string> {
        return ResourceParser.get(this._resxPath, name);
    }
}
