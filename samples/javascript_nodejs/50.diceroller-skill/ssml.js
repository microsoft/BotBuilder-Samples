// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/** Supports the insertion of recorded audio files. */
function audioTag(src, body) {
    return `<audio src="${ src }">${ body }</audio>`;
}
module.exports.audio = audioTag;

/** An empty element used to control the prosodic boundaries (pauses) between words. */
function breakTag(options) {
    options = options || {};
    var output = `<break`;
    if (options.strength) {
        output += ` strength="${ options.strength }"`;
    }
    if (options.time) {
        output += ` time="${ options.time }"`;
    }
    output += `/>`;
    return output;
}
module.exports.break = breakTag;

/** Increases the level of stress (prominence) with which the contained text is spoken. */
function emphasisTag(body, options) {
    options = options || {};
    var output = `<emphasis`;
    if (options.level) {
        output += ` level="${ options.level }"`;
    }
    output += `>${ body }</emphasis>`;
    return output;
}
module.exports.emphasis = emphasisTag;

/** Specifies an external pronunciation lexicon file. */
function lexiconTag(uri, options) {
    options = options || {};
    var output = `<lexicon uri="${ uri }"`;
    if (options.type) {
        output += ` type="${ options.type }"`;
    }
    output += `/>`;
    return output;
}
module.exports.lexicon = lexiconTag;

/**
 * Designates a specific reference point in the text sequence. This element can also be used to
 * mark an output audio stream for asynchronous notification.
 */
function markTag(name) {
    return `<mark name="${ name }"/>`;
}
module.exports.mark = markTag;

/** Marks a paragraph within a document. */
function pTag(body, options) {
    options = options || {};
    var output = `<p`;
    if (options.lang) {
        output += ` lang="${ options.lang }"`;
    }
    output += `>${ body }</p>`;
    return output;
}
module.exports.p = pTag;

/**
 * Specifies the phonetic pronunciation for the contained text using phones from a supported
 * phonetic alphabet.
 */
function phonemeTag(ph, body, options) {
    options = options || {};
    var output = `<phoneme`;
    if (options.alphabet) {
        output += ` alphabet="${ options.alphabet }"`;
    }
    output += ` ph="${ ph }">${ body }</phoneme>`;
    return output;
}
module.exports.phoneme = phonemeTag;

/**
 * Specifies the pitch, contour, range, rate, duration, and volume for speaking the contained
 * text.
 */
function prosodyTag(body, options) {
    options = options || {};
    var output = `<prosody`;
    if (options.pitch) {
        output += ` pitch="${ options.pitch }"`;
    }
    if (options.contour) {
        output += ` contour="${ options.contour }"`;
    }
    if (options.range) {
        output += ` range="${ options.range }"`;
    }
    if (options.rate) {
        output += ` rate="${ options.rate }"`;
    }
    if (options.duration) {
        output += ` duration="${ options.duration }"`;
    }
    if (options.volume) {
        output += ` volume="${ options.volume }"`;
    }
    output += `>${ body }</prosody>`;
    return output;
}
module.exports.prosody = prosodyTag;

/** Marks a sentence within a document. */
function sTag(body, options) {
    options = options || {};
    var output = `<s`;
    if (options.lang) {
        output += ` xml:lang="${ options.lang }"`;
    }
    output += `>${ body }</s>`;
    return output;
}
module.exports.s = sTag;

/**
 * The say-as element indicates the content type (such as number or date) of text contained in
 * the element. This provides guidance to the speech synthesis engine about how to pronounce
 * the contained text.
 */
function sayAsTag(interpretAs, body, options) {
    options = options || {};
    var output = `<say-as interpret-as="${ interpretAs }"`;
    if (options.format) {
        output += ` format="${ options.format }"`;
    }
    if (options.detail) {
        output += ` detail="${ options.detail }"`;
    }
    output += `>${ body }</say-as>`;
    return output;
}
module.exports.sayAs = sayAsTag;

/** The required root element of a Speech Synthesis Markup Language (SSML) document. */
function speakTag(body, options) {
    options = options || {};
    return `<speak xmlns="http://www.w3.org/2001/10/synthesis" ` +
        `version="${ options.version || '1.0' }" ` +
        `xml:lang="${ options.lang || 'en-US' }"` +
        `>${ body }</speak>`;
}
module.exports.speak = speakTag;

/** Specifies a string of text to speak in place of the text contained in the element. */
function subTag(map) {
    var output = '';
    for (var alias in map) {
        if (map.hasOwnProperty(alias)) {
            output += `<sub alias="${ alias }">${ map[alias] }</sub>`;
        }
    }
    return output;
}
module.exports.sub = subTag;

/**
 * With the parameters of the voice element, you can change the voice used in speech synthesis
 * and also specify the attributes of the voice such as the culture, the gender, and the age
 * of the voice.
 */
function voiceTag(body, options) {
    options = options || {};
    var output = `<voice`;
    if (options.name) {
        output += ` name="${ options.name }"`;
    }
    if (options.gender) {
        output += ` gender="${ options.gender }"`;
    }
    if (options.age) {
        output += ` age="${ options.age }"`;
    }
    if (options.lang) {
        output += ` xml:lang="${ options.lang }"`;
    }
    if (options.variant) {
        output += ` variant="${ options.variant }"`;
    }
    output += `>${ body }</voice>`;
    return output;
}

module.exports.voice = voiceTag;
