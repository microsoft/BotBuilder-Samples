// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/** Supports the insertion of recorded audio files. */
function audioTag(src, body) {
<<<<<<< HEAD
    return `<audio src="${src}">${body}</audio>`;
=======
    return `<audio src="${ src }">${ body }</audio>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
}
module.exports.audio = audioTag;

/** An empty element used to control the prosodic boundaries (pauses) between words. */
function breakTag(options) {
    options = options || {};
    var output = `<break`;
    if (options.strength) {
<<<<<<< HEAD
        output += ` strength="${options.strength}"`;
    }
    if (options.time) {
        output += ` time="${options.time}"`;
=======
        output += ` strength="${ options.strength }"`;
    }
    if (options.time) {
        output += ` time="${ options.time }"`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
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
<<<<<<< HEAD
        output += ` level="${options.level}"`;
    }
    output += `>${body}</emphasis>`;
=======
        output += ` level="${ options.level }"`;
    }
    output += `>${ body }</emphasis>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.emphasis = emphasisTag;

/** Specifies an external pronunciation lexicon file. */
function lexiconTag(uri, options) {
    options = options || {};
<<<<<<< HEAD
    var output = `<lexicon uri="${uri}"`;
    if (options.type) {
        output += ` type="${options.type}"`;
=======
    var output = `<lexicon uri="${ uri }"`;
    if (options.type) {
        output += ` type="${ options.type }"`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
    output += `/>`;
    return output;
}
module.exports.lexicon = lexiconTag;

<<<<<<< HEAD
/** 
 * Designates a specific reference point in the text sequence. This element can also be used to 
 * mark an output audio stream for asynchronous notification.  
 */
function markTag(name) {
    return `<mark name="${name}"/>`;
=======
/**
 * Designates a specific reference point in the text sequence. This element can also be used to
 * mark an output audio stream for asynchronous notification.
 */
function markTag(name) {
    return `<mark name="${ name }"/>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
}
module.exports.mark = markTag;

/** Marks a paragraph within a document. */
function pTag(body, options) {
    options = options || {};
    var output = `<p`;
    if (options.lang) {
<<<<<<< HEAD
        output += ` lang="${options.lang}"`;
    }
    output += `>${body}</p>`;
=======
        output += ` lang="${ options.lang }"`;
    }
    output += `>${ body }</p>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.p = pTag;

<<<<<<< HEAD
/** 
 * Specifies the phonetic pronunciation for the contained text using phones from a supported 
 * phonetic alphabet. 
=======
/**
 * Specifies the phonetic pronunciation for the contained text using phones from a supported
 * phonetic alphabet.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 */
function phonemeTag(ph, body, options) {
    options = options || {};
    var output = `<phoneme`;
    if (options.alphabet) {
<<<<<<< HEAD
        output += ` alphabet="${options.alphabet}"`;
    }
    output += ` ph="${ph}">${body}</phoneme>`;
=======
        output += ` alphabet="${ options.alphabet }"`;
    }
    output += ` ph="${ ph }">${ body }</phoneme>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.phoneme = phonemeTag;

<<<<<<< HEAD
/** 
 * Specifies the pitch, contour, range, rate, duration, and volume for speaking the contained 
 * text. 
=======
/**
 * Specifies the pitch, contour, range, rate, duration, and volume for speaking the contained
 * text.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 */
function prosodyTag(body, options) {
    options = options || {};
    var output = `<prosody`;
    if (options.pitch) {
<<<<<<< HEAD
        output += ` pitch="${options.pitch}"`;
    }
    if (options.contour) {
        output += ` contour="${options.contour}"`;
    }
    if (options.range) {
        output += ` range="${options.range}"`;
    }
    if (options.rate) {
        output += ` rate="${options.rate}"`;
    }
    if (options.duration) {
        output += ` duration="${options.duration}"`;
    }
    if (options.volume) {
        output += ` volume="${options.volume}"`;
    }
    output += `>${body}</prosody>`;
=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.prosody = prosodyTag;

/** Marks a sentence within a document. */
function sTag(body, options) {
    options = options || {};
    var output = `<s`;
    if (options.lang) {
<<<<<<< HEAD
        output += ` xml:lang="${options.lang}"`;
    }
    output += `>${body}</s>`;
=======
        output += ` xml:lang="${ options.lang }"`;
    }
    output += `>${ body }</s>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.s = sTag;

<<<<<<< HEAD
/** 
 * The say-as element indicates the content type (such as number or date) of text contained in 
 * the element. This provides guidance to the speech synthesis engine about how to pronounce 
 * the contained text. 
 */
function sayAsTag(interpretAs, body, options) {
    options = options || {};
    var output = `<say-as interpret-as="${interpretAs}"`;
    if (options.format) {
        output += ` format="${options.format}"`;
    }
    if (options.detail) {
        output += ` detail="${options.detail}"`;
    }
    output += `>${body}</say-as>`;
=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    return output;
}
module.exports.sayAs = sayAsTag;

/** The required root element of a Speech Synthesis Markup Language (SSML) document. */
function speakTag(body, options) {
    options = options || {};
    return `<speak xmlns="http://www.w3.org/2001/10/synthesis" ` +
<<<<<<< HEAD
        `version="${options.version || '1.0'}" ` +
        `xml:lang="${options.lang || 'en-US'}"` + 
        `>${body}</speak>`;
=======
        `version="${ options.version || '1.0' }" ` +
        `xml:lang="${ options.lang || 'en-US' }"` +
        `>${ body }</speak>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
}
module.exports.speak = speakTag;

/** Specifies a string of text to speak in place of the text contained in the element. */
function subTag(map) {
    var output = '';
    for (var alias in map) {
        if (map.hasOwnProperty(alias)) {
<<<<<<< HEAD
            output += `<sub alias="${alias}">${map[alias]}</sub>`;
=======
            output += `<sub alias="${ alias }">${ map[alias] }</sub>`;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        }
    }
    return output;
}
module.exports.sub = subTag;

<<<<<<< HEAD
/** 
 * With the parameters of the voice element, you can change the voice used in speech synthesis 
 * and also specify the attributes of the voice such as the culture, the gender, and the age 
=======
/**
 * With the parameters of the voice element, you can change the voice used in speech synthesis
 * and also specify the attributes of the voice such as the culture, the gender, and the age
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 * of the voice.
 */
function voiceTag(body, options) {
    options = options || {};
    var output = `<voice`;
    if (options.name) {
<<<<<<< HEAD
        output += ` name="${options.name}"`;
    }
    if (options.gender) {
        output += ` gender="${options.gender}"`;
    }
    if (options.age) {
        output += ` age="${options.age}"`;
    }
    if (options.lang) {
        output += ` xml:lang="${options.lang}"`;
    }
    if (options.variant) {
        output += ` variant="${options.variant}"`;
    }
    output += `>${body}</voice>`;
    return output;
}
module.exports.voice = voiceTag;
=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
