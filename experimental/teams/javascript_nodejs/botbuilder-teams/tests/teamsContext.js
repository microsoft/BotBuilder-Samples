const assert = require('assert');
const { TeamsContext } = require('../lib');
const { TurnContext } = require('botbuilder');

describe('TeamsContext', () => {
  it('fetch context from turn state', (done) => {
    const teamsCtx = new TeamsContext();
    const turnCtx = new TurnContext(null);
    turnCtx.turnState.set(TeamsContext.stateKey, teamsCtx);
    assert(TeamsContext.from(turnCtx) === teamsCtx);
    done();
  });
});
