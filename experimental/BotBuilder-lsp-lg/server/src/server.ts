// Type definitions for Visual Studio Code 1.46
// Project: https://github.com/microsoft/vscode
// Definitions by: Visual Studio Code Team, Microsoft <https://github.com/Microsoft>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped

/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import {
	createConnection,
	TextDocuments,
	Diagnostic,
	DiagnosticSeverity,
	ProposedFeatures,
	InitializeParams,
	DidChangeConfigurationNotification,
	CompletionItem,
	CompletionItemKind,
	TextDocumentPositionParams,
	TextDocumentSyncKind,
	InitializeResult,
	Files,
	Location,
	MarkedString,
	HoverParams,
	DefinitionParams,
	ExecuteCommandParams,
	SignatureHelpParams,
	SignatureHelp,
	ParameterInformation,
	SignatureInformation,
	TextEdit,
	TextDocumentEdit
} from 'vscode-languageserver';

import { TemplatesStatus, TemplatesEntity } from './templatesStatus';
import * as util from './util';
import * as path from 'path';

import {
	TextDocument, Position, Range, DocumentUri
} from 'vscode-languageserver-textdocument';
import { Templates, Template } from 'botbuilder-lg';
import { SignPrivateKeyInput } from 'crypto';

// Create a connection for the server, using Node's IPC as a transport.
// Also include all preview / proposed LSP features.
const connection = createConnection(ProposedFeatures.all);

// Create a simple text document manager. 
const documents: TextDocuments<TextDocument> = new TextDocuments(TextDocument);

let hasConfigurationCapability = false;
let hasWorkspaceFolderCapability = false;
let hasDiagnosticRelatedInformationCapability = false;


connection.onInitialize((params: InitializeParams) => {
	const capabilities = params.capabilities;

	// Does the client support the `workspace/configuration` request?
	// If not, we fall back using global settings.
	hasConfigurationCapability = !!(
		capabilities.workspace && !!capabilities.workspace.configuration
	);
	hasWorkspaceFolderCapability = !!(
		capabilities.workspace && !!capabilities.workspace.workspaceFolders
	);
	hasDiagnosticRelatedInformationCapability = !!(
		capabilities.textDocument &&
		capabilities.textDocument.publishDiagnostics &&
		capabilities.textDocument.publishDiagnostics.relatedInformation
	);

	const result: InitializeResult = {
		capabilities: {
			textDocumentSync: TextDocumentSyncKind.Incremental,
			// Tell the client that this server supports code completion.
			completionProvider: {
				// resolveProvider: true,
				triggerCharacters: [ '{', '(', '[', '.', '\n', '#', '=', ',' ]
			},
			hoverProvider: true,
			definitionProvider: true,
			executeCommandProvider: {
				commands: ['lg.extension.onEnterKey']
			},
			signatureHelpProvider: {
				triggerCharacters: ['(', ',']
			}
		}
	};
	if (hasWorkspaceFolderCapability) {
		result.capabilities.workspace = {
			workspaceFolders: {
				supported: true
			}
		};
	}
	return result;
});

connection.onInitialized(() => {
	if (hasConfigurationCapability) {
		// Register for all configuration changes.
		connection.client.register(DidChangeConfigurationNotification.type, undefined);
	}

	if (hasWorkspaceFolderCapability) {
		connection.workspace.onDidChangeWorkspaceFolders(_event => {
			// _event.added.forEach(e => path..uri)
			connection.console.log('Workspace folder change event received.');
		});
	}
});

// The example settings
interface ExampleSettings {
	maxNumberOfProblems: number;
}

// The global settings, used when the `workspace/configuration` request is not supported by the client.
// Please note that this is not the case when using this server with the client provided in this example
// but could happen with other clients.
const defaultSettings: ExampleSettings = { maxNumberOfProblems: 1000 };
let globalSettings: ExampleSettings = defaultSettings;

// Cache the settings of all open documents
const documentSettings: Map<string, Thenable<ExampleSettings>> = new Map();


connection.onDidChangeConfiguration(change => {
	if (hasConfigurationCapability) {
		// Reset all cached document settings
		documentSettings.clear();
	} else {
		globalSettings = <ExampleSettings>(
			(change.settings.languageServerExample || defaultSettings)
		);
	}

	// To update templatestatus with only open text documents
	triggerLGFileFinder(); 

	// Revalidate all open text documents
	documents.all().forEach(validateTextDocument);
	documents.all().forEach(updateDiagnostics);
});

function getWordRangeAtPosition(document: TextDocument, position: Position): Range|null {
	const firstPart = /[a-zA-Z0-9_.]+$/.exec(document.getText({start: document.positionAt(0), end: position}));
	const secondPart = /^[a-zA-Z0-9_.]+/.exec(document.getText({start: position, end: document.positionAt(document.getText().length-1)}));
	
	if (!firstPart && !secondPart) {
		return null;
	}

	const startPosition = firstPart==null?null: document.positionAt(document.offsetAt(position) - firstPart[0].length);
	const endPosition = secondPart==null?null: document.positionAt(document.offsetAt(position) + secondPart[0].length);

	const wordRange : Range = {
		start: startPosition==null?position:startPosition,
		end: endPosition==null?position:endPosition
	};
	return wordRange;
}

connection.onHover((params: HoverParams) => {

	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;

	if (!util.isLgFile(path.basename(document.uri))) {
        return;
    }

	
	const wordRange = getWordRangeAtPosition(document, position);

	if (!wordRange) {
		return undefined;
	}

	const wordName = document.getText(wordRange);
	const functionEntity = util.getFunctionEntity(document.uri, wordName);

	if (functionEntity !== undefined) {
		const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
		const functionIntroduction = `${wordName}(${functionEntity.params.join(", ")}): ${returnType}`;

		const contents = [];
		contents.push(MarkedString.fromPlainText(functionIntroduction));
		contents.push(MarkedString.fromPlainText(functionEntity.introduction));
		
		return {contents, wordRange};
	}

});

// connection.onExecuteCommand((params: ExecuteCommandParams) =>{
	
// 	const commandName = params.command;

// 	const args = params.arguments;

// 	if(commandName == 'lg.extension.onEnterKey' && args != undefined) {
// 		onEnterKey(args);
// 	}


// });

// function onEnterKey(args: any[]) {
	
// 	const document = documents.get(args[0]);
//     // const editor = window.document.documentURI;//.activeTextEditor;
// 	const cursorPos: Position = args[1];
	
	
// 	const textBeforeCursor = document?.getText({
// 		start: {
// 			line: cursorPos.line,
// 			character: 0
// 		},
// 		end: cursorPos
// 	});

//     const lineBreakPos = cursorPos;

//     let matches: RegExpExecArray | { replace: (arg0: string, arg1: string) => void; }[];
//     if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor)) !== null && !util.isInFencedCodeBlock(document!, cursorPos)) {
//         // -IF: new line would indent 
//         return editor.edit(editBuilder => {
//             const emptyNumber  = matches as RegExpExecArray;
//             const dashIndex = '\n    ' + emptyNumber[1];
//             editBuilder.insert(lineBreakPos, dashIndex);
//         }).then(() => { editor.revealRange(editor.selection); });
//     } else if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !util.isInFencedCodeBlock(document!, cursorPos)) {
//         // in '- ' or '# ' line
//         return editor.edit(editBuilder => {
//             const replacedStr = `\n${matches[1].replace('#', '-')} `;
//             editBuilder.insert(lineBreakPos, replacedStr);
//         }).then(() => { editor.revealRange(editor.selection); });
//     } else if ((matches = /^(\s*-)\s*/.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
//         // in '-' empty line, enter would delete the head dash
//         return editor.edit(editBuilder => {
//             const range = new vscode.Range(lineBreakPos.line, 0, lineBreakPos.line, lineBreakPos.character);
//             editBuilder.delete(range);
//         }).then(() => { editor.revealRange(editor.selection); });
//     } else {
//         return commands.executeCommand('type', { source: 'keyboard', text: '\n' });
//     }
// }

connection.onSignatureHelp((params: SignatureHelpParams) => {
	
	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;

	if (!util.isLgFile(path.basename(document.uri))) {
		return;
	}

	const {functionName, paramIndex} = parseFunction(document, position);
	const functionEntity = util.getFunctionEntity(document.uri, functionName);

	if (functionEntity === undefined) {
		return;
	}

	const paramInfoList: ParameterInformation[] = [];
	functionEntity.params.forEach(u => paramInfoList.push(ParameterInformation.create(u)));
	
	const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
	const sigLabel = `${functionName}(${functionEntity.params.join(", ")}): ${returnType}`;
	const sigInfo = SignatureInformation.create(sigLabel);
	sigInfo.parameters = paramInfoList;
	
	const signatureHelp : SignatureHelp = {
		activeParameter: paramIndex,
		activeSignature: 0,
		signatures: [sigInfo]
	};

	return signatureHelp;
});



function parseFunction(document: TextDocument, position: Position) : {functionName:string, paramIndex: number}
{
	let functionName = "";
	let paramIndex = 0;
	const range: Range = {
		start: {
			line: position.line, 
			character: 0
		}, 
		end: position
	};
	const text: string = document.getText(range);
	let bracketsDiffNumber = 0;  // right bracket number - left bracket number, if bracketsDiffNumber === 0, present that is out of param inner scope
	for (let i = text.length - 1; i >= 0; i--) {
		const currentChar: string = text.charAt(i);
		if (currentChar === ',' && bracketsDiffNumber === 0) {
			paramIndex++;
		} else if (currentChar === '(' && bracketsDiffNumber === 0 && i > 0) {
			const wordRange = getWordRangeAtPosition(document, {line: position.line, character: i - 1});
			if (wordRange !== undefined) {
				functionName = document.getText(wordRange!);
				break;
			}
		} else if (currentChar === ')') {
			bracketsDiffNumber++;
		} else if (currentChar === '(') {
			bracketsDiffNumber--;
		}
	}

	return { functionName, paramIndex };
}


connection.onDefinition((params: DefinitionParams) => {

	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;
		
	if (!util.isLgFile(path.basename(document.uri))) {
		return;
	}

	try {
		const wordRange = getWordRangeAtPosition(document, position);
		if (!wordRange) {
			return undefined;
		}
		let templateName = document.getText(wordRange);
		if (templateName.startsWith('lg.')) {
			templateName = templateName.substr('lg.'.length);
		}

		const templates: Templates = util.getTemplatesFromCurrentLGFile(document.uri);
		const template: Template|undefined = templates.allTemplates.find(u=>u.name === templateName);
		if (template === undefined)
			return undefined;

		const lineNumber: number = template.sourceRange.range.start.line - 1;
		const columnNumber: number = template.sourceRange.range.start.character;
		const definitionPosition: Position = {line: lineNumber, character: columnNumber};

		let definitionUri: DocumentUri|undefined = undefined;
		TemplatesStatus.templatesMap.forEach((value, key) => {
			if (template.sourceRange.source === key) {
				definitionUri = value.uri;
			}
		});

		if (definitionUri === undefined) {
			return undefined;
		}

		// return new Location(definitionUri, definitionPosition);
		return Location.create(definitionUri, {start: definitionPosition, end: definitionPosition});
	} catch(e){
		return undefined;
   }
});

function triggerLGFileFinder() {
    TemplatesStatus.lgFilesOfWorkspace = [];
    documents.all().forEach(textDocument => {
        TemplatesStatus.lgFilesOfWorkspace.push(Files.uriToFilePath(textDocument.uri)!);
    });
}

function getDocumentSettings(resource: string): Thenable<ExampleSettings> {
	if (!hasConfigurationCapability) {
		return Promise.resolve(globalSettings);
	}
	triggerLGFileFinder();
	let result = documentSettings.get(resource);
	if (!result) {
		result = connection.workspace.getConfiguration({
			scopeUri: resource,
			section: 'languageServerExample'
		});
		documentSettings.set(resource, result);
	}
	return result;
}

// Only keep settings for open documents
documents.onDidClose(e => {
	documentSettings.delete(e.document.uri);
	// delete templates from map
	triggerLGFileFinder();
	if(TemplatesStatus.templatesMap.has(Files.uriToFilePath(e.document.uri)!)) {
		TemplatesStatus.templatesMap.delete(e.document.uri);
	}
});

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent(change => {
	validateTextDocument(change.document);
	updateDiagnostics(change.document);
});

async function updateDiagnostics(document: TextDocument): Promise<void> {
	
    if(!util.isLgFile(path.basename(document.uri)) ) 
		return;
	
    const confDiagLevel = await connection.workspace.getConfiguration({
		scopeUri: document.uri, 
		section : 'LG.Expression.ignoreUnknownFunction'
	}); //.then(((value: string) => value));
    const confCustomFuncListSetting : string = await connection.workspace.getConfiguration({
		scopeUri: document.uri, 
		section: 'LG.Expression.customFunctionList'
	}).then(((value: string) => value == null ? '' : value));    
	
	const engine: Templates = Templates.parseText(document.getText(), Files.uriToFilePath(document.uri));
    const diagnostics = engine.diagnostics;

    TemplatesStatus.templatesMap.set(Files.uriToFilePath(document.uri)!, new TemplatesEntity(document.uri, engine));

	const lspDiagnostics: Diagnostic[] = [];
    let customFunctionList: string[] = [];
    if (confCustomFuncListSetting.length >= 1) {
        customFunctionList = confCustomFuncListSetting.split(",").map(u => u.trim());
    }
    diagnostics.forEach(u => {
        const isUnkownFuncDiag: boolean = u.message.includes("it's not a built-in function or a custom function");
        
		let severity : DiagnosticSeverity;
		switch(u.severity) {
			case 0: 
				severity = DiagnosticSeverity.Error;					
				break;
			case 1:
				severity = DiagnosticSeverity.Warning;
				break;
			case 2:
				severity = DiagnosticSeverity.Information;
				break;
			case 3:
				severity = DiagnosticSeverity.Hint;
				break;
			default:
				severity = DiagnosticSeverity.Error;
				break;
		}

		if (isUnkownFuncDiag === true){
            let ignored = false;
            const funcName = extractFuncName(u.message);
            if (customFunctionList.includes(funcName)) {
                ignored = true;
            } else {
                switch (confDiagLevel) {
                    case "ignore":
                        if (isUnkownFuncDiag) {
                            ignored = true;
                        }
                        break;
                
                    case "warn":
                            if (isUnkownFuncDiag) {
                                u.severity = DiagnosticSeverity.Warning;
                            }
                        break;
                    default:
                        break;
                }
			}

            if (ignored === false){
                const diagItem = Diagnostic.create(
                    {
                        start: {
							line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, 
							character: u.range.start.character
						},
                        end: {
							line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, 
							character: u.range.end.character
						},
					},
					u.message,
                    severity
				);
                lspDiagnostics.push(diagItem);
            }
        } else {
            const diagItem = Diagnostic.create(
                {
					start: {
						line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, 
						character: u.range.start.character
					},
					end: {
						line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, 
						character: u.range.end.character
					}
				},
                u.message,
                severity
            );
            lspDiagnostics.push(diagItem);
        }
	});
	
	// Send the computed diagnostics to VSCode.
	connection.sendDiagnostics({ uri: document.uri, diagnostics: lspDiagnostics });
}

function extractFuncName(errorMessage: string): string {
    const message = errorMessage.match(/'\.\s([\w][\w0\-.9_]*)\s+does\snot\shave/)![1];
    return message;
}

async function validateTextDocument(textDocument: TextDocument): Promise<void> {
	// In this simple example we get the settings for every validate run.
	const settings = await getDocumentSettings(textDocument.uri);

	// The validator creates diagnostics for all uppercase words length 2 and more
	const text = textDocument.getText();
	const pattern = /\b[A-Z]{2,}\b/g;
	let m: RegExpExecArray | null;

	let problems = 0;
	const diagnostics: Diagnostic[] = [];
	while ((m = pattern.exec(text)) && problems < settings.maxNumberOfProblems) {
		problems++;
		const diagnostic: Diagnostic = {
			severity: DiagnosticSeverity.Warning,
			range: {
				start: textDocument.positionAt(m.index),
				end: textDocument.positionAt(m.index + m[0].length)
			},
			message: `${m[0]} is all uppercase.`,
			source: 'ex'
		};
		if (hasDiagnosticRelatedInformationCapability) {
			diagnostic.relatedInformation = [
				{
					location: {
						uri: textDocument.uri,
						range: Object.assign({}, diagnostic.range)
					},
					message: 'Spelling matters'
				},
				{
					location: {
						uri: textDocument.uri,
						range: Object.assign({}, diagnostic.range)
					},
					message: 'Particularly for names'
				}
			];
		}
		diagnostics.push(diagnostic);
	}

	// Send the computed diagnostics to VSCode.
	connection.sendDiagnostics({ uri: textDocument.uri, diagnostics });
}

connection.onDidChangeWatchedFiles(_change => {
	// Monitored files have change in VSCode
	connection.console.log('We received an file change event');
});

// This handler provides the initial list of the completion items.
connection.onCompletion(
	(_textDocumentPosition: TextDocumentPositionParams): CompletionItem[] => {
		// The pass parameter contains the position of the text document in
		// which code complete got requested. For the example we ignore this
		// info and always provide the same completion items.
		const document = documents.get(_textDocumentPosition.textDocument.uri);
		const fspath = Files.uriToFilePath(document!.uri);
// 		const test = path.normalize(_textDocumentPosition.textDocument.uri);
// 		const fspath = path.parse(path.normalize(_textDocumentPosition.textDocument.uri));
// 		const basename = path.basename(fspath.dir);
// 		const resolve = path.resolve(_textDocumentPosition.textDocument.uri);
// 		const dirname = path.dirname(_textDocumentPosition.textDocument.uri);
// 		const isab = path.isAbsolute(fs!);
// 		const tests = path.relative(fspath.dir,_textDocumentPosition.textDocument.uri);
// //"c:\Users\t-sheny\OneDrive - Microsoft\Desktop\test.lg"




		const position = _textDocumentPosition.position;
		const lineTextBefore = document?.getText({
			start: {
				line: position.line,
				character: 0
			},
			end: position
		}).toString();

		if (/\[[^\]]*\]\([^)]*$/.test(lineTextBefore!) && !util.isInFencedCodeBlock(document!, position)) {
            // []() import suggestion
            const paths = Array.from(new Set(TemplatesStatus.lgFilesOfWorkspace));

            return paths.filter(u => fspath !== u).reduce((prev : any[], curr: string) => {
                const relativePath = path.relative(path.dirname(fspath!), curr);
                const item = {
					label: relativePath, 
					kind: CompletionItemKind.Reference,
					detail: curr
				};
                item.detail = curr;
                prev.push(item);
                return prev;
            }, []);
        } else if (/\$\{[^}]*$/.test(lineTextBefore!)) {
            // buildin function prompt in expression
            const items: CompletionItem[] = [];
            const functions = util.getAllFunctions(document!.uri);
            functions.forEach((value, key) => {
                const completionItem = CompletionItem.create(key);
                const returnType = util.getreturnTypeStrFromReturnType(value.returntype);
                completionItem.detail = `${key}(${value.params.join(", ")}): ${returnType}`;
                completionItem.documentation = value.introduction;
                completionItem.insertText = `${key}(${value.params.map(u => u.split(':')[0].trim()).join(", ")})`;
                items.push(completionItem);
            });

            return items;
        } else if (isStartStructure(document!, position)) {
            // structure name and key suggestion
            const items: CompletionItem[] = [];
            util.cardTypes.forEach(value => {
                const completionItem = CompletionItem.create(value);
                completionItem.detail = `create ${value} structure`;
                let insertTextArray = util.cardPropDict.Others;
                // if (value === 'CardAction' || value === 'Suggestions' || value === 'Attachment' || value === 'Activity') {
                if (value === 'CardAction' || value === 'Attachment' || value === 'Activity') {
                    insertTextArray = util.cardPropDict[value];
                } else if (value.endsWith('Card')){
                    insertTextArray = util.cardPropDict.Cards;
                }

                completionItem.insertText = value + '\r\n' + insertTextArray.map(u => `\t${u.name}=${u.placeHolder}`).join('\r\n') + '\r\n';
                items.push(completionItem);
            });

            return items;
        } else if (isInStructure(document!, position).isInStruct && /^\s*$/.test(lineTextBefore!)) {
            const structureName = isInStructure(document!, position).struType;
            const items: CompletionItem[] = []; 

            const nameToPropertiesMapping = Object.entries(util.cardPropDictFull);
            const propertiesMapping = nameToPropertiesMapping.find(u => u[0].toLowerCase() === structureName!.toLowerCase());
            if (propertiesMapping !== undefined) {
                const properties = propertiesMapping[1];
                properties.forEach(propertyItem => {
                    const completionItem = CompletionItem.create(propertyItem.name);
                    completionItem.detail = `create property ${propertyItem.name}`;
                    const placeHolder = 'placeHolder' in propertyItem ? propertyItem['placeHolder'] : `{${propertyItem.name}}`;
                    completionItem.insertText = propertyItem.name + '=' + placeHolder;
                    items.push(completionItem);
                });
                return items;
            }
        }  else if (/^>\s!#/.test(lineTextBefore!)) {
            // options suggestion following "> !#"
            const items: CompletionItem[] = [];
            if (/>\s!#$/.test(lineTextBefore!)) {
                AddToCompletion(optionsMap.options, items);
            } else {
                if (/>\s!#\s*@strict\s*=$/.test(lineTextBefore!)) {
                    AddToCompletion(optionsMap.strictOptions, items);
                } else if (/>\s!#\s*@replaceNull\s*=$/.test(lineTextBefore!)) {
                    AddToCompletion(optionsMap.replaceNullOptions, items);
                } else if (/>\s!#\s*@lineBreakStyle\s*=$/.test(lineTextBefore!)) {
                    AddToCompletion(optionsMap.lineBreakStyleOptions, items);
                } else if (/>\s!#\s*@Exports\s*=$/.test(lineTextBefore!) || (/>\s!#\s*@Exports\s*=/.test(lineTextBefore!) && /,\s*$/.test(lineTextBefore!))) {
                    const templatesOptions = TemplatesStatus.templatesMap.get(fspath!)!.templates.toArray();
                    // const templatesOptions = Templates.parseFile(fspath!).toArray();
					for (let i=0;i<templatesOptions!.length;++i) {
                        const templateName = templatesOptions![i].name;
                        const completionItem = CompletionItem.create(" " + templateName);
                        completionItem.detail = " " + templateName;
                        completionItem.insertText = " " + templateName;
                        items.push(completionItem);
                    }
                }
            }
			return items;
		}
    
		return [];
	}	
        
);

function isStartStructure(document: TextDocument,
	position: Position) {
		const lineTextBefore = document?.getText({
			start: {
				line: position.line,
				character: 0
			},
			end: position
		}).toString();
		if (util.isInFencedCodeBlock(document, position)
				|| !(/^\s*\[[^\]]*$/.test(lineTextBefore))
				|| position.line <= 0)
			return false;
		
		let previourLine = position.line - 1;
		const currentOffset = document.offsetAt({
			line: position.line,
			character: 0
		});
		while(previourLine >= 0) {
			const previousLineText = document?.getText({
				start: {
					line: previourLine,
					character: 0
				},
				end: document.positionAt(currentOffset - 1) 
			}).toString().trim();
			if (previousLineText === '') {
				previourLine--;
				continue;
			} else if (previousLineText.startsWith('#')){
				return true;
			} else {
				return false;
			}
		}
		return false;
}

function isInStructure(document: TextDocument,
	position: Position): { isInStruct:boolean; struType:string|undefined } {
		if (util.isInFencedCodeBlock(document, position)
				|| position.line <= 0)
			return {isInStruct:false, struType:undefined};

		let previourLine = position.line - 1;
		const currentOffset = document.offsetAt({
			line: position.line,
			character: 0
		});
		while(previourLine >= 0) {
			const previousLineText = document?.getText({
				start: {
					line: previourLine,
					character: 0
				},
				end: document.positionAt(currentOffset - 1) 
			}).toString().trim();			
			if (previousLineText.startsWith('#')
			|| previousLineText === ']'
			|| previousLineText.startsWith('-')
			|| previousLineText.endsWith('```')) {
				return {isInStruct:false, struType:undefined};
			} else if (previousLineText.startsWith('[')){
				const structureType = previousLineText.substr(1).trim();
				return {isInStruct:true, struType:structureType};
			}
			previourLine--;
		}
		return {isInStruct:false, struType:undefined};
}

function AddToCompletion(options: any, items: CompletionItem[]) {
	for (const option in options) {
		const completionItem = CompletionItem.create(" " + option);
		completionItem.detail = options[option].detail;
		completionItem.documentation = options[option].documentation;
		completionItem.insertText = options[option].insertText;
		items.push(completionItem);
	}
}

// This handler resolves additional information for the item selected in
// the completion list.
// connection.onCompletionResolve(
// 	(item: CompletionItem): CompletionItem => {
// 		if (item.data === 1) {
// 			item.detail = 'TypeScript details';
// 			item.documentation = 'TypeScript documentation';
// 		} else if (item.data === 2) {
// 			item.detail = 'JavaScript details';
// 			item.documentation = 'JavaScript documentation';
// 		}
// 		return item;
// 	}
// );

// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// Listen on the connection
connection.listen();



const optionsMap = {
    options: { 
        '@strict': {
            detail: ' @strict = true',
            documentation: 'Developers who do not want to allow a null evaluated result can implement the strict option.', 
            insertText: ' @strict'},
        '@replaceNull': {
            detail: ' @replaceNull = ${path} is undefined',
            documentation: 'Developers can create delegates to replace null values in evaluated expressions by using the replaceNull option.',
            insertText: ' @replaceNull'},
        '@lineBreakStyle': {
            detail: ' @lineBreakStyle = markdown',
            documentation: 'Developers can set options for how the LG system renders line breaks using the lineBreakStyle option.',
            insertText: ' @lineBreakStyle'},
        '@Namespace': {
            detail: ' @Namespace = foo',
            documentation: 'You can register a namespace for the LG templates you want to export.',
            insertText: ' @Namespace'},
        '@Exports': {
            detail: ' @Exports = template1, template2',
            documentation: 'You can specify a list of LG templates to export.',
            insertText: ' @Exports'}
    },
    strictOptions : {
        'true': {
            detail: ' true',
            documentation: 'Null error will throw a friendly message.',
            insertText: ' true'
        },
        'false': {
            detail: ' false',
            documentation: 'A compatible result will be given.',
            insertText: ' false'
        }
    },
    replaceNullOptions: {
        '${path} is undefined':{
            detail: 'The null input in the path variable would be replaced with ${path} is undefined.',
            documentation: null,
            insertText: ' ${path} is undefined'
        }    
    },
    lineBreakStyleOptions: {
        'default': {
            detail: ' default',
            documentation: 'Line breaks in multiline text create normal line breaks.',
            insertText: ' default'
        },
        'markdown': {
            detail: ' markdown',
            documentation: 'Line breaks in multiline text will be automatically converted to two lines to create a newline.',
            insertText: ' markdown'
        }
    } 
};
