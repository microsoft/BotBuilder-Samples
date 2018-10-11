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

    constructor(apiKey: string) {
        this.apiKey = apiKey;
        this.postProcessor = new TranslatorPostProcessor();
    }

    public setPostProcessorTemplate(noTranslatePatterns: string[], wordDictionary?: { [id: string]: string }) {
        this.postProcessor = new TranslatorPostProcessor(noTranslatePatterns, wordDictionary);
    }

    private getAccessToken(): Promise<string> {
        
        return request({
            url: `https://api.cognitive.microsoft.com/sts/v1.0/issueToken?Subscription-Key=${this.apiKey}`,
            method: 'POST'
        })
        .then(result => Promise.resolve(result))
    }

    private entityMap: any = {
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': '&quot;',
        "'": '&#39;',
        "/": '&#x2F;'
    };
    
    private escapeHtml(source: string) {
        return String(source).replace(/[&<>"'\/]/g, s => this.entityMap[s]);
    }

    public detect(text: string): Promise<string> {
        let uri: any = "http://api.microsofttranslator.com/v2/Http.svc/Detect";
        let query: any = `?text=${encodeURI(text)}`;
        return this.getAccessToken()
        .then(accessToken => {
            return request({
                url: uri + query,
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + accessToken
                }
            })
        })
        .then(lang => Promise.resolve(lang.replace(/<[^>]*>/g, '')))
    }

    public translateArray = async (options: TranslateArrayOptions):  Promise<object> => {
		try {
            let from = options.from;
            let to = options.to;
            let texts = options.texts;
            let orgTexts = [];
            let translationRequests = [];
            texts.forEach((text, index, array) => {
                orgTexts.push(text);
                let escapedText =(text);
                texts[index] = escapedText;
                let translationRequest = new TranslationRequest();
                translationRequest.Text = escapedText;
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
            let results = [];
            translations.forEach((traslationModel, index, array) => {
                                let translation = traslationModel.translations[0].text;
                                let alignment = traslationModel.translations[0].alignment.proj;
                                translation = this.postProcessor.fixTranslation(orgTexts[index], alignment, translation);
                                let result: TranslationResult = { translatedText: translation };
                                 results.push(result);
                            });
            // Array.from(elements).forEach((element, index, array) => {
            //                 let translation = element.getElementsByTagName('TranslatedText')[0].textContent as string;
            //                 let alignment = element.getElementsByTagName('Alignment')[0].textContent as string;
            //                 translation = this.postProcessor.fixTranslation(orgTexts[index], alignment, translation);
            //                 let result: TranslationResult = { translatedText: translation };
            //                 results.push(result);
            //             });
			return Promise.resolve(results);
		} catch (e) {
			switch (e.statusCode) {
				case 401:
					throw new Error('Cognitive Authentication Failed');
				default:
					throw new Error('Internal Error');
			}
		}
    };
    
    // public translateArray(options: TranslateArrayOptions): Promise<TranslationResult[]> {
    //     let from = options.from;
    //     let to = options.to;
    //     let texts = options.texts;
    //     let orgTexts = [];
    //     texts.forEach((text, index, array) => {
    //         orgTexts.push(text);
    //         let escapedText = this.escapeHtml(text);
    //         texts[index] = escapedText;
    //     });
        
    //     let uri: any = `https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true&from=${from}&to=${to}`;
    //     let body: any = JSON.stringify(texts);
        
        
    //     return this.getAccessToken()
    //     .then(accessToken => {
    //         return request({
    //             url: uri,
    //             method: 'POST',
    //             headers: {
    //                 'Ocp-Apim-Subscription-Key': this.apiKey,
    //                 'Content-Type': 'text/json'
    //             },
    //             body: body,
                
    //         })
    //     })
    //     .then(response => {
    //         let results: JSON.parse( response );
    //         let parser = new DOMParser();
    //         let responseObj = parser.parseFromString(response);
    //         let elements = responseObj.getElementsByTagName("TranslateArray2Response");
    //         Array.from(elements).forEach((element, index, array) => {
    //             let translation = element.getElementsByTagName('TranslatedText')[0].textContent as string;
    //             let alignment = element.getElementsByTagName('Alignment')[0].textContent as string;
    //             translation = this.postProcessor.fixTranslation(orgTexts[index], alignment, translation);
    //             let result: TranslationResult = { translatedText: translation };
    //             results.push(result);
    //         });
    //         return Promise.resolve(results);
    //     })
    // }
}


/**
 * @private
 */
class TranslatorPostProcessor {
    noTranslatePatterns: string[];
    wordDictionary: { [id: string]: string};

    constructor(noTranslatePatterns?: string[], wordDictionary?: { [id: string]: string }) {
        this.noTranslatePatterns = [];
        this.wordDictionary = wordDictionary;
        this.noTranslatePatterns.push("mon nom est (.+)");
        
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
            noTranslatePatterns.forEach(pattern => {
                if (pattern.indexOf('(') == -1) {
                    pattern = `(${pattern})`;
                }
                this.noTranslatePatterns.push(pattern);
            });
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

    public fixTranslation(sourceMessage: string, alignment: string, targetMessage: string): string {
        let numericMatches = sourceMessage.match(/[0-9]+/g);
        let containsNum = numericMatches != null;
        
        if ((!containsNum && this.noTranslatePatterns.length == 0 && !this.wordDictionary) || alignment.trim() == '') {
            return targetMessage;
        }

        let toBeReplaced: string[] = [];
        this.noTranslatePatterns.forEach(pattern => {
            let regExp = new RegExp(pattern, "i");
            let matches = sourceMessage.match(regExp);
            if (matches != null) {
                toBeReplaced.push(pattern);
            }
        });

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

                srcWords.forEach(wrd => {
                    if (chrIndx == noTranslateStartChrIndex) {
                        srcIndx = wrdIndx;
                    }
                    if (srcIndx != -1) {
                        if (newChrLengthFromMatch + srcWords[wrdIndx].length >= noTranslateMatchLength) {
                            return;
                        }
                        newNoTranslateArrayLength++;
                        newChrLengthFromMatch += srcWords[wrdIndx].length + 1;
                    }
                    chrIndx += wrd.length + 1;
                    wrdIndx++;
                });
                                
                let wrdNoTranslate = srcWords.slice(srcIndx, srcIndx + newNoTranslateArrayLength)
                
                wrdNoTranslate.forEach(srcWrds => {
                    trgWords = this.keepSrcWrdInTranslation(alignMap, srcWords, trgWords, srcIndx);
                    srcIndx++;
                });
                
            });
        }
        
        if (toBeReplacedByDictionary.length > 0) {
            toBeReplacedByDictionary.forEach(word => {
                let regExp = new RegExp(word, "i");
                let match = regExp.exec(sourceMessage);
                
                let noTranslateStartChrIndex = match.index;
                
                let noTranslateMatchLength = match[0].length;
                
                let wrdIndx = 0;
                let chrIndx = 0;
                let newChrLengthFromMatch = 0;
                let srcIndx = -1;
                let newNoTranslateArrayLength = 1;

                srcWords.forEach(wrd => {
                    chrIndx += wrd.length + 1;
                    wrdIndx++;
                    if (chrIndx == noTranslateStartChrIndex) {
                        srcIndx = wrdIndx;
                        return;
                    }
                });
                
                let wrdNoTranslate = srcWords.slice(srcIndx, srcIndx + 1)
                
                wrdNoTranslate.forEach(srcWrds => {
                    trgWords = this.replaceWordInDictionary(alignMap, srcWords, trgWords, srcIndx);
                    srcIndx++;
                });
                
            });
        }

        console.log(toBeReplacedByDictionary);
        
        if (toBeReplacedByDictionary.length > 0) {
            toBeReplacedByDictionary.forEach(word => {
                let regExp = new RegExp(word, "i");
                let match = regExp.exec(sourceMessage);
                
                let noTranslateStartChrIndex = match.index;
                
                let noTranslateMatchLength = match[0].length;
                
                let wrdIndx = 0;
                let chrIndx = 0;
                let newChrLengthFromMatch = 0;
                let srcIndx = -1;
                let newNoTranslateArrayLength = 1;

                srcWords.forEach(wrd => {
                    chrIndx += wrd.length + 1;
                    wrdIndx++;
                    if (chrIndx == noTranslateStartChrIndex) {
                        srcIndx = wrdIndx;
                        return;
                    }
                });
                
                let wrdNoTranslate = srcWords.slice(srcIndx, srcIndx + 1)
                
                wrdNoTranslate.forEach(srcWrds => {
                    trgWords = this.replaceWordInDictionary(alignMap, srcWords, trgWords, srcIndx);
                    srcIndx++;
                });
                
            });
        }

        if (containsNum) {
            numericMatches.forEach(numericMatch => {
                let srcIndx = srcWords.findIndex(wrd => wrd == numericMatch)
                trgWords = this.keepSrcWrdInTranslation(alignMap, srcWords, trgWords, srcIndx);
            });
        }
        return this.join(" ", trgWords);
    }
}