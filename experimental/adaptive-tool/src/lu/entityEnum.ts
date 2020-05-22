/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

export const EntityTypesObj = {
    EntityType: ['ml', 'prebuilt', 'regex', 'list', 'composite', 'patternany', 'phraselist'],
    Prebuilt: [
      'age',
      'datetimeV2',
      'dimension',
      'email',
      'geographyV2',
      'keyPhrase',
      'money',
      'number',
      'ordinal',
      'ordinalV2',
      'percentage',
      'personName',
      'phonenumber',
      'temperature',
      'url',
      'datetime',
    ],
  };
  
  export type LineState = 'listEntity' | 'utterance' | 'mlEntity' | 'other';
  
