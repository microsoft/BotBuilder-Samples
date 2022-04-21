/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

 /* eslint-disable @typescript-eslint/no-non-null-assertion */

import { WorkspaceFolder } from 'vscode-languageserver'
import { TextDocument, Range, Position } from "vscode-languageserver-textdocument";
import { LuFilesStatus } from './luFilesStatus';

import * as fs from 'fs';
import * as path from 'path';
import { URI } from 'vscode-uri'

export function isLuFile(fileName: string): boolean {
    if(fileName === undefined || !fileName.toLowerCase().endsWith('.lu')) {
        return false;
    }
    return true;
}

export function isInFencedCodeBlock(doc: TextDocument, position: Position): boolean {
    const textBefore = doc.getText({start: {line: 0, character: 0}, end: position});
    const matches = textBefore.match(/```[\w ]*$/gm);
    if (matches == null) {
        return false;
    } else {
        return matches.length % 2 != 0;
    }
}

export function getWordRangeAtPosition(document: TextDocument, position: Position): Range|undefined {
    const text = document.getText();
    const line = position.line;
    const pos = position.character;
    const lineText = text.split('\n')[line];
    let match: RegExpMatchArray | null;
    const wordDefinition = /[a-zA-Z0-9_/.-]+/g;
    while ((match = wordDefinition.exec(lineText))) {
        const matchIndex = match.index || 0;
        if (matchIndex > pos) {
            return undefined;
        } else if (wordDefinition.lastIndex >= pos) {
            return {start: {line: line, character: matchIndex}, end: {line: line, character: wordDefinition.lastIndex}};
        }  
    }
    return undefined;
}

export function triggerLGFileFinder(workspaceFolders: WorkspaceFolder[]) : void {
    LuFilesStatus.luFilesOfWorkspace = [];
    
    workspaceFolders?.forEach(workspaceFolder => fs.readdir(URI.parse(workspaceFolder.uri).fsPath, (err, files) => {
        if(err) {
            console.log(err);
        } else {
            LuFilesStatus.luFilesOfWorkspace = [];
            files.filter(file => isLuFile(file)).forEach(file => {
                LuFilesStatus.luFilesOfWorkspace.push(path.join(URI.parse(workspaceFolder.uri).fsPath, file));
            });
        }
    }));
}

export function replaceText(curLineContent : string) : string {
    const labelRegex = /\{\s*[\w.@:\s]+\s*=\s*[\w.@:\s]+\s*\}/g;
    let match : RegExpMatchArray | null;
    let resultStr = '';
    let startIdx = 0;
    while ((match = labelRegex.exec(curLineContent))) {
        const leftBoundIdx = match.index;
        const rightBoundIdx = labelRegex.lastIndex;
        resultStr += curLineContent.slice(startIdx, leftBoundIdx);
        if (leftBoundIdx && rightBoundIdx) {
            const entityStr = curLineContent.slice(leftBoundIdx + 1, rightBoundIdx - 1);
            if (entityStr.split('=').length == 2) {
                const enitity = entityStr.split('=')[1].trim();
                resultStr += enitity;
            }
            startIdx = rightBoundIdx;
        }
    }
    if (startIdx !== curLineContent.length) {
        resultStr += curLineContent.slice(startIdx, curLineContent.length);
    }
    return resultStr;
    
}