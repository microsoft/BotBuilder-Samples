/**
 * @module botbuilder
 */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Activity, BotState, Storage, TurnContext } from 'botbuilder-core';

const NO_KEY: string = `UserState: overridden getStorageKey method did not return a key.`;

/**
 * Reads and writes user state for your bot to storage.
 *
 * @remarks
 * Each user your bot communicates with will have its own isolated storage object that can be used
 * to persist information about the user across all of the conversation you have with that user.
 *
 * ```JavaScript
 * const { UserState, MemoryStorage } = require('botbuilder');
 *
 * const userState = new UserState(new MemoryStorage());
 * ```
 */
export class V3UserState extends BotState {
    /**
     * Creates a new UserState instance.
     * @param storage Storage provider to persist user state to.
     * @param namespace (Optional) namespace to append to storage keys. Defaults to an empty string.
     */
    constructor(storage: Storage, private namespace: string = '') {
        super(storage, (context: TurnContext) => {
            // Calculate storage key
            const key: string = this.getStorageKey(context);

            return key ? Promise.resolve(key) : Promise.reject(new Error(NO_KEY));
        });
    }

    /**
     * Returns the storage key for the current user state.
     * @param context Context for current turn of conversation with the user.
     */
    public getStorageKey(context: TurnContext): string | undefined {
        const activity: Activity = context.activity;
        const channelId: string = activity.channelId;
        const userId: string = activity && activity.from && activity.from.id ? activity.from.id : undefined;

        if (!channelId) {
            throw new Error('missing activity.channelId');
        }

        if (!userId) {
            throw new Error('missing activity.from.id');
        }

        return `${ userId },userData`;
    }
}
