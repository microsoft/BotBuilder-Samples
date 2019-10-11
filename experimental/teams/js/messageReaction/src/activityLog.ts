// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Storage,
} from 'botbuilder';

import{
    Activity
} from 'botframework-schema'


export class ActivityLog  {
    private readonly _storage: Storage;   

    public constructor(storage: Storage) {
        this._storage = storage;
    }
    
    public async append(activityId: string, activity: Partial<Activity>): Promise<void> {
        if (activityId == null)
        {
            throw new TypeError("activityId is required for ActivityLog.append");
        }

        if (activity == null)
        {
            throw new TypeError("activity is required for ActivityLog.append");
        }

        let obj = { };
        obj[activityId] = { activity };

        await this._storage.write( obj );

        return;
    }

    public async find(activityId: string): Promise<Activity>
    {
        if (activityId == null)
        {
            throw new TypeError("activityId is required for ActivityLog.find");
        }

        var items = await this._storage.read( [ activityId ] );
        return (items && items[activityId]) ? items[activityId].activity : null;
    }
}
