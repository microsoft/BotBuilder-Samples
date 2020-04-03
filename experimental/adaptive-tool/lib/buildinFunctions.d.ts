/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { ReturnType } from 'adaptive-expressions';
export declare class FunctionEntity {
    constructor(params: string[], returntype: ReturnType, introduction: string);
    params: string[];
    returntype: ReturnType;
    introduction: string;
}
export declare const buildInfunctionsMap: Map<string, FunctionEntity>;
//# sourceMappingURL=buildinFunctions.d.ts.map