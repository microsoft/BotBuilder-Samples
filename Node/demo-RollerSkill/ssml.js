var sprintf = require('sprintf-js');

/** Supports the insertion of recorded audio files. */
module.exports.audio = function (src, template, params) {
    var output = '<audio src="' + src + '">';
    output += module.exports.vsprintf(template, params);
    output += '</audio>';
    return output;
}

/** An empty element used to control the prosodic boundaries (pauses) between words. */
module.exports.break = function (options) {
    options = options || {};
    var output = '<break';
    if (options.strength) {
        output += ' strength="' + options.strength + '"';
    }
    if (options.time) {
        output += ' time="' + options.time + '"';
    }
    output += '/>';
    return output;
}

/** Increases the level of stress (prominence) with which the contained text is spoken. */
module.exports.emphasis = function (template, params, options) {
    options = options || {};
    var output = '<emphasis';
    if (options.level) {
        output += ' level="' + options.level + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</emphasis>';
    return output;
}

/** Specifies an external pronunciation lexicon file. */
module.exports.lexicon = function (uri, options) {
    options = options || {};
    var output = '<lexicon uri="' + uri + '"';
    if (options.type) {
        output += ' type="' + options.type + '"';
    }
    output += '/>';
    return output;
}

/** 
 * Designates a specific reference point in the text sequence. This element can also be used to 
 * mark an output audio stream for asynchronous notification.  
 */
module.exports.mark = function (name) {
    return '<mark name="' + name + '"/>';
}

/** Marks a paragraph within a document. */
module.exports.p = function (template, params, options) {
    options = options || {};
    var output = '<p';
    if (options.lang) {
        output += ' lang="' + options.lang + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</p>';
    return output;
}

/** 
 * Specifies the phonetic pronunciation for the contained text using phones from a supported 
 * phonetic alphabet. 
 */
module.exports.phoneme = function (ph, template, params, options) {
    options = options || {};
    var output = '<phoneme';
    if (options.alphabet) {
        output += ' alphabet="' + options.alphabet + '"';
    }
    output += ' ph="' + ph + '">';
    output += module.exports.vsprintf(template, params);
    output += '</phoneme>';
    return output;
}

/** 
 * Specifies the pitch, contour, range, rate, duration, and volume for speaking the contained 
 * text. 
 */
module.exports.prosody = function (template, params, options) {
    options = options || {};
    var output = '<prosody';
    if (options.pitch) {
        output += ' pitch="' + options.pitch + '"';
    }
    if (options.contour) {
        output += ' contour="' + options.contour + '"';
    }
    if (options.range) {
        output += ' range="' + options.range + '"';
    }
    if (options.rate) {
        output += ' rate="' + options.rate + '"';
    }
    if (options.duration) {
        output += ' duration="' + options.duration + '"';
    }
    if (options.volume) {
        output += ' volume="' + options.volume + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</prosody>';
    return output;
}

/** Marks a sentence within a document. */
module.exports.s = function (template, params, options) {
    options = options || {};
    var output = '<s';
    if (options.lang) {
        output += ' xml:lang="' + options.lang + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</s>';
    return output;
}

/** 
 * The say-as element indicates the content type (such as number or date) of text contained in 
 * the element. This provides guidance to the speech synthesis engine about how to pronounce 
 * the contained text. 
 */
module.exports.sayAs = function (interpretAs, template, params, options) {
    options = options || {};
    var output = '<say-as interpret-as="' + interpretAs + '"';
    if (options.format) {
        output += ' format="' + options.format + '"';
    }
    if (options.detail) {
        output += ' detail="' + options.detail + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</say-as>';
    return output;
}

/** The required root element of a Speech Synthesis Markup Language (SSML) document. */
module.exports.speak = function (template, params, options) {
    options = options || {};
    var output = '<speak xmlns="http://www.w3.org/2001/10/synthesis" ' +
        'version="' + (options.version || '1.0') + '" ' +
        'xml:lang="' + (options.lang || 'en-US') + '">';
    output += module.exports.vsprintf(template, params);
    output += '</speak>';
    return output;
}

/** Specifies a string of text to speak in place of the text contained in the element. */
module.exports.sub = function (map) {
    var output = '';
    for (var alias in map) {
        if (map.hasOwnProperty(alias)) {
            output += '<sub alias="' + alias + '">' + map[alias] + '</sub>';
        }
    }
    return output;
}

/** 
 * With the parameters of the voice element, you can change the voice used in speech synthesis 
 * and also specify the attributes of the voice such as the culture, the gender, and the age 
 * of the voice.
 */
module.exports.voice = function (template, params, options) {
    options = options || {};
    var output = '<voice';
    if (options.name) {
        output += ' name="' + options.name + '"';
    }
    if (options.gender) {
        output += ' gender="' + options.gender + '"';
    }
    if (options.age) {
        output += ' age="' + options.age + '"';
    }
    if (options.lang) {
        output += ' xml:lang="' + options.lang + '"';
    }
    if (options.variant) {
        output += ' variant="' + options.variant + '"';
    }
    output += '>';
    output += module.exports.vsprintf(template, params);
    output += '</voice>';
    return output;
}


/** 
 * Expands a template with the supplied parameters. Template syntax is expected to be sprintf-js 
 * syntax. 
 */
module.exports.vsprintf = function (template, params) {
    return params && params.length ? sprintf.vsprintf(template, params) : template;
}