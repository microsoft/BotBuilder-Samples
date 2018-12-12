
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ambiguity = require('./ambiguity.js');
const constraints = require('./constraints.js');
const languageGeneration = require('./languageGeneration.js');
const parsing = require('./parsing.js');
const ranges = require('./ranges.js');
const resolution = require('./resolution.js');

// Creating TIMEX expressions from natural language using the Recognizer package.
ambiguity.dateAmbiguity();
ambiguity.timeAmbiguity();
ambiguity.dateTimeAmbiguity();
ranges.dateRange();
ranges.timeRange();

// Manipulating TIMEX expressions in code using the TIMEX Datatype package.
parsing.examples();
languageGeneration.examples();
resolution.examples();
constraints.examples();
