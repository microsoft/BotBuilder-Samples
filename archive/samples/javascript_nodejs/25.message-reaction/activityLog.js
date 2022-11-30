// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class ActivityLog {
    constructor(storage) {
        this._storage = storage;
    }

    async append(activityId, activity) {
        if (activityId == null) {
            throw new TypeError('activityId is required for ActivityLog.append');
        }
        if (activity == null) {
            throw new TypeError('activity is required for ActivityLog.append');
        }

        const obj = {};
        obj[activityId] = { activity };
        await this._storage.write(obj);
    }

    async find(activityId) {
        if (activityId == null) {
            throw new TypeError('activityId is required for ActivityLog.find');
        }

        var items = await this._storage.read([activityId]);
        return (items && items[activityId]) ? items[activityId].activity : null;
    }
}
exports.ActivityLog = ActivityLog;
