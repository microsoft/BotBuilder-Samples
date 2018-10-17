// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as request from 'request-promise-native';
import { promises } from 'fs';


export class TranslateArrayOptions{
    texts: string[];    from: string;
    to: string;
    contentType?: string;
    category?: string;
}

/**
 * @private
 */
export class TranslationResult {
    translatedText: string;
}

/**
 * @private
 */
export class TranslationRequest{
	Text: string;
}

/**
 * @private
 */
export class MicrosoftTranslator {
    
    apiKey: string;
    postProcessor: TranslatorPostProcessor;

    constructor(apiKey: string, noTranslatePatterns?: {[id: string] : string[]}, wordDictionary?: { [id: string]: string }) {
        this.apiKey = apiKey;
        this.postProcessor = new TranslatorPostProcessor();
        this.postProcessor = new TranslatorPostProcessor(noTranslatePatterns, wordDictionary);

    }

    public detect = async (text: string): Promise<object> => {
        let uri: any = "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        let translationRequest = new TranslationRequest();
        translationRequest.Text = text;
        const response = await request({
            url: uri,
            method: 'POST',
            headers: {
                'Ocp-Apim-Subscription-Key': this.apiKey,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify([translationRequest]),
        });
        let detectedLanguage = JSON.parse(response);
        return detectedLanguage;
    };

    public translateArray = async (options: TranslateArrayOptions):  Promise<object> => {
		try {
            let from = options.from;
            let to = options.to;
            let texts = options.texts;
            let orgTexts = [];
            let translationRequests = [];
            let results = [];
            texts.forEach((text, index, array) => {
                let normalizedText = text.trim();
                    orgTexts.push(normalizedText);
                    texts[index] = normalizedText;
                    let translationRequest = new TranslationRequest();
                    translationRequest.Text = normalizedText;
                    translationRequests.push(translationRequest);
            });
            let uri: any = `https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true&from=${from}&to=${to}`;
			const response = await request({
				url: uri,
				method: 'POST',
				headers: {
                    'Ocp-Apim-Subscription-Key': this.apiKey,
					'Content-Type': 'application/json',
				},
				body: JSON.stringify(translationRequests),
            });
            let translations = JSON.parse(response);
            translations.forEach((traslationModel, index, array) => {
                                let translation = traslationModel.translations[0].text;
                                if(traslationModel.translations[0].alignment){
                                    let alignment = traslationModel.translations[0].alignment.proj;
                                    translation = this.postProcessor.fixTranslation(orgTexts[index], alignment, translation, from);
                                }
                                let result: TranslationResult = { translatedText: translation };
                                 results.push(result);
                            });
			return results;
		} catch (e) {
			switch (e.statusCode) {
				case 401:
					throw new Error('Cognitive Authentication Failed');
				default:
					throw new Error('Internal Error');
			}
		}
    };
}


/**
 * @private
 */
class TranslatorPostProcessor {
    noTranslatePatterns: { [languageId : string] : string[]};
    wordDictionary: { [id: string]: string };

    constructor(noTranslatePatterns?: { [languageId : string] : string[]}, wordDictionary?: { [id: string]: string }) {
        this.noTranslatePatterns = {};
        this.wordDictionary = wordDictionary;
        
        if (wordDictionary) {
            Object.keys(this.wordDictionary).forEach(word => {
                if (word != word.toLowerCase()) {
                    Object.defineProperty(this.wordDictionary, word.toLowerCase(),
                        Object.getOwnPropertyDescriptor(this.wordDictionary, word));
                    delete this.wordDictionary[word];
                }
            });
            
        }

        if(noTranslatePatterns) {
            for(var key in noTranslatePatterns) {
                noTranslatePatterns[key].forEach((pattern, index, array) => {
                    if(!(key in this.noTranslatePatterns)){
                        this.noTranslatePatterns[key] = [];
                    }
                    if (pattern.indexOf('(') == -1) {
                        pattern = `(${pattern})`;
                    }
                    this.noTranslatePatterns[key][index] = pattern;
                });
               
            }
        }  
    }

    private join(delimiter: string, words: string[]): string {
        return words.join(delimiter).replace(/[ ]?'[ ]?/g, "'").trim();
    }

    private splitSentence(sentence: string, alignments: string[], isSrcSentence = true): string[] {
        let wrds = sentence.split(' ');
        let lastWrd = wrds[wrds.length - 1];
        if (".,:;?!".indexOf(lastWrd[lastWrd.length - 1]) != -1) {
            wrds[wrds.length - 1] = lastWrd.substr(0, lastWrd.length - 1);
        }
        let alignSplitWrds: string[] = [];
        let outWrds: string[] = [];
        let wrdIndexInAlignment = 1;

        if (isSrcSentence) {
            wrdIndexInAlignment = 0;
        } else {
            alignments.sort((a, b) => {
                let aIndex = parseInt(a.split('-')[wrdIndexInAlignment].split(':')[0]);
                let bIndex = parseInt(b.split('-')[wrdIndexInAlignment].split(':')[0]);
                if (aIndex <= bIndex) {
                    return -1;
                } else {
                    return 1;
                }
            });
        }
        let sentenceWithoutSpaces = sentence.replace(/\s/g, '');
        alignments.forEach(alignData => {
            alignSplitWrds = outWrds;
            let wordIndexes = alignData.split('-')[wrdIndexInAlignment];
            let startIndex = parseInt(wordIndexes.split(':')[0]);
            let length = parseInt(wordIndexes.split(':')[1]) - startIndex + 1;
            let wrd = sentence.substr(startIndex, length);
            let newWrds: string[] = new Array(outWrds.length + 1);
            if (newWrds.length > 1) {
                newWrds = alignSplitWrds.slice();
            }
            newWrds[outWrds.length] = wrd;
            
            let subSentence = this.join("", newWrds);
            
            if (sentenceWithoutSpaces.indexOf(subSentence) != -1) {
                outWrds.push(wrd);
            }
        });
        
        alignSplitWrds = outWrds;
        
        if (this.join("", alignSplitWrds) == this.join("", wrds)) {
            return alignSplitWrds;
        } else {
            return wrds;
        }
    }

    private wordAlignmentParse(alignments: string[], srcWords: string[], trgWords: string[]): { [id: number] : number } {
        let alignMap: { [id: number] : number } = {};

        let sourceMessage = this.join(" ", srcWords);
        let trgMessage = this.join(" ", trgWords);

        alignments.forEach(alignData => {
            let wordIndexes = alignData.split('-');
            
            let srcStartIndex = parseInt(wordIndexes[0].split(':')[0]);
            let srcLength = parseInt(wordIndexes[0].split(':')[1]) - srcStartIndex + 1;
            let srcWrd = sourceMessage.substr(srcStartIndex, srcLength);
            let srcWrdIndex = srcWords.findIndex(wrd => wrd == srcWrd);
            
            let trgstartIndex = parseInt(wordIndexes[1].split(':')[0]);
            let trgLength = parseInt(wordIndexes[1].split(':')[1]) - trgstartIndex + 1;
            let trgWrd = trgMessage.substr(trgstartIndex, trgLength);
            let trgWrdIndex = trgWords.findIndex(wrd => wrd == trgWrd);

            if (srcWrdIndex != -1 && trgWrdIndex != -1) {
                alignMap[srcWrdIndex] = trgWrdIndex;   
            }
        });
        return alignMap;
    }

    private keepSrcWrdInTranslation(alignment: { [id: number] : number }, sourceWords: string[], targetWords: string[], srcWrdIndex: number) {
        if (!(typeof alignment[srcWrdIndex] === "undefined")) {
            targetWords[alignment[srcWrdIndex]] = sourceWords[srcWrdIndex];
        }
        return targetWords;
    }

    private replaceWordInDictionary(alignment: { [id: number] : number }, sourceWords: string[], targetWords: string[], srcWrdIndex: number) {
        if (!(typeof alignment[srcWrdIndex] === "undefined")) {
            targetWords[alignment[srcWrdIndex]] = this.wordDictionary[sourceWords[srcWrdIndex].toLowerCase()];
        }
        return targetWords;
    }

    public fixTranslation(sourceMessage: string, alignment: string, targetMessage: string, languageId : string): string {
        let numericMatches = sourceMessage.match(/[0-9]+/g);
        let containsNum = numericMatches != null;
        
        if ((!containsNum && (!this.noTranslatePatterns || !this.noTranslatePatterns[languageId] || this.noTranslatePatterns[languageId].length == 0) && !this.wordDictionary) || alignment.trim() == '') {
            return targetMessage;
        }

        let toBeReplaced: string[] = [];
        if(this.noTranslatePatterns[languageId]){
            this.noTranslatePatterns[languageId].forEach(pattern => {
                let regExp = new RegExp(pattern, "i");
                let matches = sourceMessage.match(regExp);
                if (matches != null) {
                    toBeReplaced.push(pattern);
                }
            });
        }

        let toBeReplacedByDictionary: string [] = [];
        if (this.wordDictionary) {
            Object.keys(this.wordDictionary).forEach(word => {
                if (sourceMessage.toLowerCase().indexOf(word.toLowerCase()) != -1) {
                    toBeReplacedByDictionary.push(word);
                }
            });
        }
        
        let alignments = alignment.trim().split(' ');
        
        let srcWords = this.splitSentence(sourceMessage, alignments);
        let trgWords = this.splitSentence(targetMessage, alignments, false);
        
        let alignMap = this.wordAlignmentParse(alignments, srcWords, trgWords);

        if (toBeReplaced.length > 0) {
            toBeReplaced.forEach(pattern => {
                let regExp = new RegExp(pattern, "i");
                let match = regExp.exec(sourceMessage);
                
                let noTranslateStartChrIndex = match.index + match[0].indexOf(match[1]);
                let noTranslateMatchLength = match[1].length;
                let wrdIndx = 0;
                let chrIndx = 0;
                let newChrLengthFromMatch = 0;
                let srcIndx = -1;
                let newNoTranslateArrayLength = 1;
                var sourceMessageCharacters = sourceMessage.split('');

                srcWords.forEach(wrd => {
                    if (chrIndx == noTranslateStartChrIndex) {
                        srcIndx = wrdIndx;
                    }
                    if (srcIndx != -1) {
                        if (newChrLengthFromMatch + srcWords[wrdIndx].length >= noTranslateMatchLength) {
                            return;
                        }
                        newNoTranslateArrayLength++;
                        // newChrLengthFromMatch += srcWords[wrdIndx].length + 1;
                        newChrLengthFromMatch += srcWords[wrdIndx].length;
                    }
                    if (chrIndx + wrd.length < sourceMessageCharacters.length && sourceMessageCharacters[chrIndx + wrd.length] == ' ')
                    {
                        chrIndx += wrd.length + 1;
                    }
                    else{
                        chrIndx += wrd.length;
                    }
                    wrdIndx++;

                    if (srcIndx == -1)
                    {
                        return;
                    }
                });
                     
                
                let wrdNoTranslate = srcWords.slice(srcIndx, srcIndx + newNoTranslateArrayLength)
                
                wrdNoTranslate.forEach(srcWrds => {
                    trgWords = this.keepSrcWrdInTranslation(alignMap, srcWords, trgWords, srcIndx);
                    srcIndx++;
                });
                
            });
        }
        
        if(this.wordDictionary){
            srcWords.forEach((wrd, index, Array) => {
                if (wrd.toLowerCase() in this.wordDictionary){
                    const replacementWord = this.wordDictionary[wrd.toLowerCase()];
                    trgWords[alignMap[index]] = replacementWord;
                } else if (wrd.toUpperCase() in this.wordDictionary){
                    const replacementWord = this.wordDictionary[wrd.toUpperCase()];
                    trgWords[alignMap[index]] = replacementWord;
                }
            });
        }
        
        return this.join(" ", trgWords);
    }
}