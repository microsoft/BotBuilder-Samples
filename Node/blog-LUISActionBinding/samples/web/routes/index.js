require('dotenv-extended').load({
    path: '../.env'
});

var express = require('express');
var router = express.Router();
var _ = require('lodash');

var LuisActions = require('../../../core');

var ModelUrl = process.env.LUIS_MODEL_URL;
var SampleActions = require('../../all');

router.get('/', function (req, res, next) {
  res.render('index');
});

router.post('/', function (req, res, next) {
  var query = req.body.query;
  var actionModel = parseActionModel(req.body.actionModel);

  // restart if missing query or model
  if (!query && !actionModel) {
    return res.render('index');
  }

  tryEvaluate(actionModel, query, req.body)
    .then(actionModel => {

      if(actionModel.contextModel) {
        // triggering Contextual Action from scratch is not supported on this Web Sample
        return res.render('index');
      }

      switch (actionModel.status) {
        case LuisActions.Status.NoActionRecognized:
          // No action identified, restart
          return res.render('index', { query: query });

        case LuisActions.Status.MissingParameters:
          // Missing or invalidate parameters
          var action = SampleActions.find(a => a.intentName === actionModel.intentName);
          var viewModel = {
            query: query,
            hasIntent: true,
            actionModelJson: JSON.stringify(actionModel),
            fields: createFieldsViewModel(action.schema, actionModel.parameters, actionModel.parameterErrors)
          };

          return res.render('index', viewModel);

        case LuisActions.Status.Fulfilled:
          // Action fulfilled
          return res.render('fulfill', { result: actionModel.result });
      }
    }).catch(err => {
      console.error('Error with LuisAction', err);
      res.render('error', { message: 'Error with LuisAction', error: err });
    });

});

module.exports = router;

function tryEvaluate(actionModel, query, formPost) {
  if (actionModel) {
    // Populate action parameters from form data
    var action = SampleActions.find(a => a.intentName === actionModel.intentName);
    actionModel.parameters = _.pick(formPost, _.keys(action.schema));
    return LuisActions.evaluate(ModelUrl, SampleActions, actionModel);
  } else {
    return LuisActions.evaluate(ModelUrl, SampleActions, null, query);
  }
}

function createFieldsViewModel(schema, parameteres, fieldErrors) {
  return _.map(schema, function (paramenterSchema, parameterName) {
    var fieldError = fieldErrors.find(function (err) { return err.parameterName === parameterName; });
    return {
      fieldName: parameterName,
      fieldType: paramenterSchema.type,
      fieldValue: parameteres[parameterName],
      fieldError: fieldError ? fieldError.message : null
    };
  });
}

function parseActionModel(actionModelJson) {
  try {
    return JSON.parse(actionModelJson);
  } catch (err) {
    return null;
  }
}