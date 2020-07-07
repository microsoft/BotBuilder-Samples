"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.removeLabelsInUtterance = exports.extractEntityNameInUseFeature = exports.getSuggestionRoles = exports.suggestionNoCompositeEntityTypes = exports.suggestionNoPatternAnyEntityTypes = exports.suggestionAllEntityTypes = exports.getSuggestionEntities = exports.getRegexEntities = exports.isPhraseListEntity = exports.matchedEntityPattern = exports.matchedRolesPattern = exports.matchedEnterPattern = exports.isCompositeEntity = exports.isEntityName = exports.isSeperatedEntityDef = exports.isRegexEntity = exports.isPrebuiltEntity = exports.isEntityType = exports.matchIntentInEntityDef = exports.matchIntentUsesFeatures = exports.matchedEntityCanUsesFeature = exports.getCompositesEntities = exports.getMLEntities = void 0;
function getMLEntities(text) {
    const lines = text.split('\n');
    const mlEntityRegExp = /^\s*@\s*ml\s*([0-9a-zA-Z_.-]+)\s*.*$/;
    const mlEntities = [];
    for (const line of lines) {
        if (mlEntityRegExp.test(line)) {
            const entityGroup = line.match(mlEntityRegExp);
            if (entityGroup && entityGroup.length >= 2) {
                mlEntities.push(entityGroup[1]);
            }
        }
    }
    return mlEntities;
}
exports.getMLEntities = getMLEntities;
function getCompositesEntities(luisJson) {
    const suggestionCompositesList = [];
    if (luisJson !== undefined) {
        if (luisJson.composites !== undefined && luisJson.composites.length > 0) {
            luisJson.composites.forEach(entity => {
                suggestionCompositesList.push(entity.name);
            });
        }
    }
    return suggestionCompositesList;
}
exports.getCompositesEntities = getCompositesEntities;
function matchedEntityCanUsesFeature(lineContent, text, luisJson) {
    const mlTypedEntityusesFeature = /^\s*@\s*ml\s*\w+\s*usesFeature\s*$/;
    const compositesTypedEntityusesFeature = /^\s*@\s*composites\s*\w+\s*usesFeature\s*$/;
    const notTypedEntityusesFeature = /^\s*@\s*(\w*)\s*usesFeature\s*$/;
    if (mlTypedEntityusesFeature.test(lineContent) || compositesTypedEntityusesFeature.test(lineContent)) {
        return true;
    }
    else if (notTypedEntityusesFeature.test(lineContent)) {
        const matchedGroups = lineContent.match(notTypedEntityusesFeature);
        if (matchedGroups && matchedGroups.length >= 2) {
            const entityName = matchedGroups[1];
            const validEntitiesCanusesFeature = getMLEntities(text).concat(getCompositesEntities(luisJson));
            if (validEntitiesCanusesFeature.includes(entityName)) {
                return true;
            }
        }
    }
    return false;
}
exports.matchedEntityCanUsesFeature = matchedEntityCanUsesFeature;
function matchIntentUsesFeatures(content) {
    const intentUsesFeaturesRegEx = /^\s*@\s*intent\s*\w*\s*usesFeature\s*$/;
    return intentUsesFeaturesRegEx.test(content);
}
exports.matchIntentUsesFeatures = matchIntentUsesFeatures;
function matchIntentInEntityDef(content) {
    const intentInEntityDefRegEx = /^\s*@\s*intent\s*\w+\s*$/;
    return intentInEntityDefRegEx.test(content);
}
exports.matchIntentInEntityDef = matchIntentInEntityDef;
function isEntityType(content) {
    const regexEntifyDef = /^\s*@\s*$/;
    return regexEntifyDef.test(content);
}
exports.isEntityType = isEntityType;
function isPrebuiltEntity(content) {
    const regexPrebuiltEntifyDef = /^\s*@\s*prebuilt\s*$/;
    return regexPrebuiltEntifyDef.test(content);
}
exports.isPrebuiltEntity = isPrebuiltEntity;
function isRegexEntity(content) {
    const regexPrebuiltEntifyDef = /^\s*@\s*regex\s*([\w._]+|"[\w._\s]+")+\s*=\s*$/;
    return regexPrebuiltEntifyDef.test(content);
}
exports.isRegexEntity = isRegexEntity;
function isSeperatedEntityDef(content) {
    const regexPrebuiltEntifyDef = /^\s*@\s*([\w._]+|"[\w._\s]+")+\s*=\s*$/;
    return regexPrebuiltEntifyDef.test(content);
}
exports.isSeperatedEntityDef = isSeperatedEntityDef;
function isEntityName(content) {
    const hasNameEntifyDef = /^\s*@\s*(ml|list|regex|prebuilt|composite|patternany|phraseList)\s*([\w._]+|"[\w._\s]+")\s*$/;
    const hasTypeEntityDef = /^\s*@\s*(ml|list|regex|prebuilt|composite|patternany|phraseList|intent)\s*$/;
    const hasNameEntifyDef2 = /^\s*@\s*([\w._]+|"[\w._\s]+")\s*$/;
    return hasNameEntifyDef.test(content) || (!hasTypeEntityDef.test(content) && hasNameEntifyDef2.test(content));
}
exports.isEntityName = isEntityName;
function isCompositeEntity(content) {
    const compositePatternDef = /^\s*@\s*composite\s*[\w]*\s*=\s*\[\s*.*\s*$/;
    const compositePatternDef2 = /^\s*@\s*composite\s*[\w]*\s*=\s*\[\s*.*\s*\]\s*$/;
    return compositePatternDef.test(content) || compositePatternDef2.test(content);
}
exports.isCompositeEntity = isCompositeEntity;
function matchedEnterPattern(content) {
    const regexPatternDef = /^\s*-.*{\s*$/;
    const regexPatternDef2 = /^\s*-.*{\s*}$/;
    return regexPatternDef.test(content) || regexPatternDef2.test(content);
}
exports.matchedEnterPattern = matchedEnterPattern;
function matchedRolesPattern(content) {
    const regexRolesPatternDef = /^\s*-.*{\s*.*:/;
    const regexRolesPatternDef2 = /^\s*-.*{\s*.*:}/;
    return regexRolesPatternDef.test(content) || regexRolesPatternDef2.test(content);
}
exports.matchedRolesPattern = matchedRolesPattern;
function matchedEntityPattern(content) {
    const regexRolesEntityPatternDef = /^\s*-.*{\s*@\s*$/;
    const regexRolesEntityPatternDef2 = /^\s*-.*{\s*@\s*}\s*$/;
    return regexRolesEntityPatternDef.test(content) || regexRolesEntityPatternDef2.test(content);
}
exports.matchedEntityPattern = matchedEntityPattern;
function isPhraseListEntity(content) {
    const phraseListEntityPatternDef = /^\s*@\s*phraselist\s*[\w]+\s*\(\s*$/i;
    return phraseListEntityPatternDef.test(content);
}
exports.isPhraseListEntity = isPhraseListEntity;
function getRegexEntities(luisJson) {
    const suggestionRegexList = [];
    if (luisJson !== undefined) {
        if (luisJson.regex_entities !== undefined && luisJson.regex_entities.length > 0) {
            luisJson.regex_entities.forEach(entity => {
                suggestionRegexList.push(entity.name);
            });
        }
    }
    return suggestionRegexList;
}
exports.getRegexEntities = getRegexEntities;
function getSuggestionEntities(luisJson, suggestionEntityTypes) {
    const suggestionEntityList = [];
    if (luisJson !== undefined) {
        suggestionEntityTypes.forEach(entityType => {
            if (luisJson[entityType] !== undefined && luisJson[entityType].length > 0) {
                luisJson[entityType].forEach(entity => {
                    if (entity.name) {
                        suggestionEntityList.push(entity.name);
                    }
                });
            }
        });
    }
    return suggestionEntityList;
}
exports.getSuggestionEntities = getSuggestionEntities;
exports.suggestionAllEntityTypes = [
    'entities',
    'regex_entities',
    'patternAnyEntities',
    'preBuiltEntities',
    'closedLists',
    'phraselists',
    'composites',
];
exports.suggestionNoPatternAnyEntityTypes = [
    'entities',
    'regex_entities',
    'preBuiltEntities',
    'closedLists',
    'phraselists',
    'composites',
];
exports.suggestionNoCompositeEntityTypes = [
    'entities',
    'regex_entities',
    'patternAnyEntities',
    'preBuiltEntities',
    'closedLists',
    'phraselists',
];
function getSuggestionRoles(luisJson, suggestionEntityTypes) {
    const suggestionRolesList = [];
    if (luisJson !== undefined) {
        suggestionEntityTypes.forEach(entityType => {
            if (luisJson[entityType] !== undefined && luisJson[entityType].length > 0) {
                luisJson[entityType].forEach(entity => {
                    if (entity.roles !== undefined && entity.roles.length > 0) {
                        entity.roles.forEach(role => {
                            suggestionRolesList.push(role);
                        });
                    }
                });
            }
        });
    }
    return suggestionRolesList;
}
exports.getSuggestionRoles = getSuggestionRoles;
function extractEntityNameInUseFeature(lineContent) {
    const notTypedEntityusesFeature = /^\s*@\s*(\w*)\s*usesFeature\s*/;
    if (notTypedEntityusesFeature.test(lineContent)) {
        const matchedGroups = lineContent.match(notTypedEntityusesFeature);
        if (matchedGroups && matchedGroups.length === 2) {
            const entityName = matchedGroups[1];
            return entityName;
        }
    }
    return '';
}
exports.extractEntityNameInUseFeature = extractEntityNameInUseFeature;
function removeLabelsInUtterance(lineContent) {
    const entityLabelRegex = /\{\s*[\w.@:\s]+\s*=\s*[\w.]+\s*\}/g;
    let match;
    let resultStr = '';
    let startIdx = 0;
    while ((match = entityLabelRegex.exec(lineContent))) {
        const leftBoundIdx = match.index;
        const rightBoundIdx = entityLabelRegex.lastIndex;
        resultStr += lineContent.slice(startIdx, leftBoundIdx);
        if (leftBoundIdx && rightBoundIdx) {
            const entityStr = lineContent.slice(leftBoundIdx + 1, rightBoundIdx - 1);
            if (entityStr.split('=').length == 2) {
                const enitity = entityStr.split('=')[1].trim();
                resultStr += enitity;
            }
            startIdx = rightBoundIdx;
        }
    }
    return resultStr;
}
exports.removeLabelsInUtterance = removeLabelsInUtterance;
//# sourceMappingURL=matchingPattern.js.map