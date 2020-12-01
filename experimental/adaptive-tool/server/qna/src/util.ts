/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

 /* eslint-disable @typescript-eslint/no-non-null-assertion */

import { WorkspaceFolder } from 'vscode-languageserver'
import { TextDocument, Range, Position } from "vscode-languageserver-textdocument";
import { QnaFilesStatus } from './qnaFilesStatus';
import { URI } from 'vscode-uri'
import * as fs from 'fs';
import * as path from 'path';

export function isQnaFile(fileName: string): boolean {
    if(fileName === undefined || !fileName.toLowerCase().endsWith('.qna')) {
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

export function triggerQNAFileFinder(workspaceFolders: WorkspaceFolder[]) : void {
    QnaFilesStatus.qnaFilesOfWorkspace = [];
    workspaceFolders?.forEach(workspaceFolder => fs.readdir(URI.parse(workspaceFolder.uri).fsPath, (err, files) => {
        if(err) {
            console.log(err);
        } else {
            QnaFilesStatus.qnaFilesOfWorkspace = [];
            files.filter(file => isQnaFile(file)).forEach(file => {
                QnaFilesStatus.qnaFilesOfWorkspace.push(path.join(URI.parse(workspaceFolder.uri).fsPath, file));
            });
        }
    }));
}