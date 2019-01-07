// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Returns a new instance of a Game model.
<<<<<<< HEAD
 * @param {string} type The type of game to create. 
=======
 * @param {string} type The type of game to create.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 * @param {number} sides (Optional) number of sides to each die.
 * @param {number} count (Optional) number of dice to roll of each turn.
 */
function createGame(type, sides, count) {
    switch (type) {
<<<<<<< HEAD
        case 'craps':
            return { type: type, sides: 6, count: 2, turn: 0 };
        default:
            return { type: type, sides: sides, count: count, turn: 0 };
=======
    case 'craps':
        return { type: type, sides: 6, count: 2, turn: 0 };
    default:
        return { type: type, sides: sides, count: count, turn: 0 };
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
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
