var util = require('util');
var builder = require('botbuilder');

// This Dialog works as a waterfall dialogs, with the option to control flow within each step.
// Each step is reponsible for calling next() in order to advance. Flow is returned to parent dialog when next() is invoked from the last steps closure. You can pass an optional argument to the last step that will be returned to the parent dialog.
function SimpleWaterfallDialog(dialogSteps) {
    function fn(session, args) {
        if (session.dialogData.step === undefined) {
            session.dialogData.step = 0;
        }

        var next = function (args) {
            session.dialogData.step++;
            var dialogStep = dialogSteps[session.dialogData.step];
            if (!dialogStep) {
                // no more steps
                if (args) {
                    session.endDialogWithResult(args);
                } else {
                    session.endDialog();
                }
            } else {
                dialogStep(session, args, next);
            }
        };

        // run step
        dialogSteps[session.dialogData.step](session, args, next);
    }
    SimpleWaterfallDialog.super_.call(this, fn);
}

util.inherits(SimpleWaterfallDialog, builder.SimpleDialog);

module.exports = SimpleWaterfallDialog; 