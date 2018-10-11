export declare class TranslateArrayOptions {
    texts: string[];
    from: string;
    to: string;
    contentType?: string;
    category?: string;
}
/**
 * @private
 */
export declare class TranslationResult {
    translatedText: string;
}
/**
 * @private
 */
export declare class TranslationRequest {
    Text: string;
}
/**
 * @private
 */
export declare class MicrosoftTranslator {
    apiKey: string;
    postProcessor: TranslatorPostProcessor;
    constructor(apiKey: string);
    setPostProcessorTemplate(noTranslatePatterns: string[], wordDictionary?: {
        [id: string]: string;
    }): void;
    private getAccessToken;
    private entityMap;
    private escapeHtml;
    detect(text: string): Promise<string>;
    translateArray: (options: TranslateArrayOptions) => Promise<object>;
}
/**
 * @private
 */
declare class TranslatorPostProcessor {
    noTranslatePatterns: string[];
    wordDictionary: {
        [id: string]: string;
    };
    constructor(noTranslatePatterns?: string[], wordDictionary?: {
        [id: string]: string;
    });
    private join;
    private splitSentence;
    private wordAlignmentParse;
    private keepSrcWrdInTranslation;
    private replaceWordInDictionary;
    fixTranslation(sourceMessage: string, alignment: string, targetMessage: string): string;
}
export {};
