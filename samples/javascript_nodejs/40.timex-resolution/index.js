
const ambiguity = require('./ambiguity.js');
const ranges = require('./ranges.js');
const parsing = require('./parsing.js');
const languageGeneration = require('./languageGeneration.js');
const resolution = require('./resolution.js');
const constraints = require('./constraints.js');

ambiguity.dateAmbiguity();
ambiguity.timeAmbiguity();
ambiguity.dateTimeAmbiguity();
ranges.dateRange();
ranges.timeRange();
parsing.examples();
languageGeneration.examples();
resolution.examples();
constraints.examples();




