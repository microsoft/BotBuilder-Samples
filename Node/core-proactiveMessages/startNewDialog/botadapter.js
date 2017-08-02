'use strict';

var builder = require('botbuilder');

function _clone(obj) {
    var cpy = {};
    if (obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key)) {
                cpy[key] = obj[key];
            }
        }
    }
    return cpy;
}

function loadSession(address, opts, cb) {
  
  var _consts = {
    Data : {
      SessionState: 'BotBuilder.Data.SessionState'
    }
  };

  var _this = this;
  this.lookupUser(address, (user) => {
    _this.ensureConversation(address, function (address) {

      var storageCtx = {
        userId: user.id,
        conversationId: address.conversation ? address.conversation.id : null,
        address: address,
        persistUserData: _this.settings.persistUserData,
        persistConversationData: _this.settings.persistConversationData
      };

      var loadedData;
      _this.getStorageData(storageCtx, function (data) {
        if (!_this.localizer) {
          var defaultLocale = _this.settings.localizerSettings ? _this.settings.localizerSettings.defaultLocale : null;
          _this.localizer = new DefaultLocalizer_1.DefaultLocalizer(_this.lib, defaultLocale);
        }
        var session = new builder.Session({
          localizer: _this.localizer,
          autoBatchDelay: _this.settings.autoBatchDelay,
          library: _this.lib,
          actions: _this.actions,
          middleware: _this.mwSession,
          dialogId: opts.dialogId,
          dialogArgs: opts.dialogArgs,
          dialogErrorMessage: _this.settings.dialogErrorMessage,
          onSave: function (cb) {
            var finish = _this.errorLogger(cb);
            loadedData.userData = _clone(session.userData);
            loadedData.conversationData = _clone(session.conversationData);
            loadedData.privateConversationData = _clone(session.privateConversationData);
            loadedData.privateConversationData[_consts.Data.SessionState] = session.sessionState;
            _this.saveStorageData(storageCtx, loadedData, finish, finish);
          },
          onSend: function (messages, cb) {
            _this.send(messages, cb);
          }
        });
        session.on('error', function (err) { return _this.emitError(err); });
        var sessionState;
        session.userData = data.userData || {};
        session.conversationData = data.conversationData || {};
        session.privateConversationData = data.privateConversationData || {};
        if (session.privateConversationData.hasOwnProperty(_consts.Data.SessionState)) {
          sessionState = session.privateConversationData[_consts.Data.SessionState];
          delete session.privateConversationData[_consts.Data.SessionState];
        }

        // Do the important things route/dispatch would have done
        session.sessionState = sessionState;
        var cur = session.curDialog();
        session.dialogData = cur ? cur.state : {};
        session.message = {address : address};

        loadedData = data;
        cb(null, session);

      }, (err) => { _this.errorLogger(err); cb(err); })
    }, (err) => { _this.errorLogger(err); cb(err); })
  }, (err) => { _this.errorLogger(err); cb(err); })
}

function beginDialog(address, dialogId, dialogArgs, opts, done) {
  if (typeof opts === 'function') {
    done = opts;
    opts = {};
  }

  if (opts.resume) {
    this.loadSession(address, { dialogId:dialogId, dialogArgs:dialogArgs }, 
    (err, session) => {
      if (!err) {
        session.beginDialog(dialogId, dialogArgs);
        if (done) {
          done(null);
        }
      }
      else {
        if (done) {
          done(err);
        } 
      }
    });
  }
  else {
    this.beginDialog(address, dialogId, dialogArgs, done);
  }
}

function patch(bot) {
  bot.beginDialog = beginDialog;
  bot.loadSession = loadSession;
  return bot;
}

exports.patch = patch;

