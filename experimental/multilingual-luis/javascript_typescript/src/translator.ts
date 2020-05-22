// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as request from 'request-promise-native';

export interface TranslateArrayOptions {
    texts: string[]; from: string;
    to: string;
    contentType?: string;
    category?: string;
}

export interface TranslationResult {
    translatedText: string;
}

export interface TranslationRequest {
    text: string;
}

export class MicrosoftTranslator {

    private postProcessor: TranslatorPostProcessor;

    constructor(private apiKey: string, noTranslatePatterns?: { [id: string]: string[] }, wordDictionary?: { [id: string]: string }) {
        this.apiKey = apiKey;
        this.postProcessor = new TranslatorPostProcessor(noTranslatePatterns, wordDictionary);

    }

    public detect = async (text: string): Promise<object> => {
        const uri: any = "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        let translationRequest = {
            text: text
        };

        const response = await request({
            url: uri,
            method: 'POST',
            headers: {
                'Ocp-Apim-Subscription-Key': this.apiKey,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify([translationRequest]),
        });
        return JSON.parse(response);
    };

    public translateArray = async (options: TranslateArrayOptions): Promise<object> => {
        try {
            const { from, to, texts } = options;
            let orgTexts = [];
            let translationRequests = [];
            let results = [];
            texts.forEach((text, index) => {
                let normalizedText = text.trim();
                orgTexts.push(normalizedText);
                texts[index] = normalizedText;
                let translationRequest = {
                    text: normalizedText
                };
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
            const translations = JSON.parse(response);
            translations.forEach((traslationModel, index) => {
                let translation = traslationModel.translations[0].text;
                if (traslationModel.translations[0].alignment) {
                    let alignment = traslationModel.translations[0].alignment.proj;
                    translation = this.postProcessor.postprocessTranslation(orgTexts[index], alignment, translation, from);
                }
                results.push({ translatedText: translation });
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
    noTranslatePatterns: { [languageId: string]: string[] };
    wordDictionary: { [id: string]: string };

    constructor(noTranslatePatterns?: { [languageId: string]: string[] }, wordDictionary?: { [id: string]: string }) {
        this.noTranslatePatterns = {};
        this.wordDictionary = wordDictionary;

        if (wordDictionary) {
            Object.keys(this.wordDictionary).forEach(word => {
                if (word !== word.toLowerCase()) {
                    Object.defineProperty(this.wordDictionary, word.toLowerCase(),
                        Object.getOwnPropertyDescriptor(this.wordDictionary, word));
                    delete this.wordDictionary[word];
                }
            });

        }

        if (noTranslatePatterns) {
            for (const key in noTranslatePatterns) {
                noTranslatePatterns[key].forEach((pattern, index) => {
                    if (!(key in this.noTranslatePatterns)) {
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
        const words = sentence.split(' ');
        const lastWord = words[words.length - 1];
        if (".,:;?!".indexOf(lastWord[lastWord.length - 1]) !== -1) {
            words[words.length - 1] = lastWord.substr(0, lastWord.length - 1);
        }
        let alignSplitWrds: string[] = [];
        const outWrds: string[] = [];
        let wrdIndexInAlignment = 1;

        if (isSrcSentence) {
            wrdIndexInAlignment = 0;
        } else {
            alignments.sort((a, b) => {
                const aIndex = parseInt(a.split('-')[wrdIndexInAlignment].split(':')[0]);
                const bIndex = parseInt(b.split('-')[wrdIndexInAlignment].split(':')[0]);
                if (aIndex <= bIndex) {
                    return -1;
                } else {
                    return 1;
                }
            });
        }
        const sentenceWithoutSpaces = sentence.replace(/\s/g, '');
        alignments.forEach(alignData => {
            alignSplitWrds = outWrds;
            const wordIndexes = alignData.split('-')[wrdIndexInAlignment];
            const startIndex = parseInt(wordIndexes.split(':')[0]);
            const length = parseInt(wordIndexes.split(':')[1]) - startIndex + 1;
            const wrd = sentence.substr(startIndex, length);
            let newWrds: string[] = new Array(outWrds.length + 1);
            if (newWrds.length > 1) {
                newWrds = alignSplitWrds.slice();
            }
            newWrds[outWrds.length] = wrd;

            const subSentence = this.join("", newWrds);

            if (sentenceWithoutSpaces.indexOf(subSentence) !== -1) {
                outWrds.push(wrd);
            }
        });

        alignSplitWrds = outWrds;

        if (this.join("", alignSplitWrds) == this.join("", words)) {
            return alignSplitWrds;
        } else {
            return words;
        }
    }

    private wordAlignmentParse(alignments: string[], srcWords: string[], trgWords: string[]): { [id: number]: number } {
        let alignMap: { [id: number]: number } = {};

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

            if (srcWrdIndex !== -1 && trgWrdIndex !== -1) {
                alignMap[srcWrdIndex] = trgWrdIndex;
            }
        });
        return alignMap;
    }

    private keepSrcWrdInTranslation(alignment: { [id: number]: number }, sourceWords: string[], targetWords: string[], srcWrdIndex: number) {
        if (!(typeof alignment[srcWrdIndex] === "undefined")) {
            targetWords[alignment[srcWrdIndex]] = sourceWords[srcWrdIndex];
        }
        return targetWords;
    }

    /**
     * postprocessTranslation is the method responsible for applying customizations on the raw translation for example :
     * applying patterns customization and custom user configured vocab dictionary
     * 
     * @param sourceMessage 
     * @param alignment 
     * @param targetMessage 
     * @param languageId 
     */
    public postprocessTranslation(sourceMessage: string, alignment: string, targetMessage: string, languageId: string): string {
        let numericMatches = sourceMessage.match(/[0-9]+/g);
        let containsNum = numericMatches !== null;

        if ((!containsNum && (!this.noTranslatePatterns || !this.noTranslatePatterns[languageId] || this.noTranslatePatterns[languageId].length == 0) && !this.wordDictionary) || alignment.trim() === '') {
            return targetMessage;
        }

        let toBeReplaced: string[] = [];
        if (this.noTranslatePatterns[languageId]) {
            this.noTranslatePatterns[languageId].forEach(pattern => {
                let regExp = new RegExp(pattern, "i");
                let matches = sourceMessage.match(regExp);
                if (matches !== null) {
                    toBeReplaced.push(pattern);
                }
            });
        }

        let toBeReplacedByDictionary: string[] = [];
        if (this.wordDictionary) {
            Object.keys(this.wordDictionary).forEach(word => {
                if (sourceMessage.toLowerCase().indexOf(word.toLowerCase()) !== -1) {
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
                const sourceMessageCharacters = sourceMessage.split('');

                srcWords.forEach(wrd => {
                    if (chrIndx == noTranslateStartChrIndex) {
                        srcIndx = wrdIndx;
                    }
                    if (srcIndx !== -1) {
                        if (newChrLengthFromMatch + srcWords[wrdIndx].length >= noTranslateMatchLength) {
                            return;
                        }
                        newNoTranslateArrayLength++;
                        newChrLengthFromMatch += srcWords[wrdIndx].length;
                    }
                    if (chrIndx + wrd.length < sourceMessageCharacters.length && sourceMessageCharacters[chrIndx + wrd.length] === ' ') {
                        chrIndx += wrd.length + 1;
                    }
                    else {
                        chrIndx += wrd.length;
                    }
                    wrdIndx++;

                    if (srcIndx === -1) {
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

        if (this.wordDictionary) {
            srcWords.forEach((wrd, index) => {
                if (wrd.toLowerCase() in this.wordDictionary) {
                    const replacementWord = this.wordDictionary[wrd.toLowerCase()];
                    trgWords[alignMap[index]] = replacementWord;
                } else if (wrd.toUpperCase() in this.wordDictionary) {
                    const replacementWord = this.wordDictionary[wrd.toUpperCase()];
                    trgWords[alignMap[index]] = replacementWord;
                }
            });
        }

        return this.join(" ", trgWords);
    }
}
