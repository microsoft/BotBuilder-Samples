/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { TextDocument, Position } from "vscode";
import { Templates } from "botbuilder-lg";
import * as vscode from 'vscode';
import { ReturnType } from "adaptive-expressions";
import { FunctionEntity } from './buildinFunctions';
export declare function isLgFile(fileName: string): boolean;
export declare function isLuFile(fileName: string): boolean;
export declare function isInFencedCodeBlock(doc: TextDocument, position: Position): boolean;
export declare function getAllTemplatesFromCurrentLGFile(lgFileUri: vscode.Uri): Templates;
export declare function getreturnTypeStrFromReturnType(returnType: ReturnType): string;
export declare function getAllFunctions(lgFileUri: vscode.Uri): Map<string, FunctionEntity>;
export declare function getFunctionEntity(lgFileUri: vscode.Uri, name: string): FunctionEntity | undefined;
//# sourceMappingURL=util.d.ts.map