// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Returns a new instance of a Game model.
 * @param {string} type The type of game to create.
 * @param {number} sides (Optional) number of sides to each die.
 * @param {number} count (Optional) number of dice to roll of each turn.
 */
function createGame(type, sides, count) {
    switch (type) {
    case 'craps':
        return { type: type, sides: 6, count: 2, turn: 0 };
    default:
        return { type: type, sides: sides, count: count, turn: 0 };
    }
}
module.exports.createGame = createGame;

/**
 * List of pre-defined game types.
 */
const GameTypes = {
    craps: 'craps',
    custom: 'custom'
};
module.exports.GameTypes = GameTypes;
