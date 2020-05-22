/**
 * @module v4-v3-storage-mapper
 */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Storage, StoreItems } from 'botbuilder';
import * as validate from 'uuid-validate';

export class StorageMapper implements Storage {

  private v3StorageProvider: any;
  private userId: string;
  
  public constructor(v3StorageProvider: any) {
    if (!v3StorageProvider) {
      throw new Error('A storage provider must be provided.');
    }

    this.v3StorageProvider = v3StorageProvider;
    this.userId = '';
  }

  private extractUserId(key: string): string {
    const keySegments: Array<string> = key.split(',');
    const user_id: string = keySegments.find(segment => {
      return validate(segment);
    });
    this.userId = user_id;
    return user_id;
  }

  private formatDataResponse = (key, inputdata) => {
    const itemKey = `${key ? key : this.userId},userData`;
    const formattedData = {};
    formattedData[itemKey] = {};
    formattedData[itemKey]['userProfile'] = inputdata;
    return formattedData;
  };
  
  public read(keys: string[]): Promise<StoreItems> {
    if (!keys || keys.length === 0) {
      return Promise.resolve({});
    }

    const userStateKey: string = keys.find(key => {
      return key.includes("userData");
    });

    if (userStateKey) {
      const context = {
        userId: this.extractUserId(userStateKey),
        persistUserData: true,
        persistConversationData: false
      };

      return new Promise((resolve, reject) => {
        this.v3StorageProvider.getData(context, (err, data) => {
          if (err) return reject();
          if (!data || !data.userData) return resolve({});
          const responseData = this.formatDataResponse(context.userId, data.userData);
          return resolve(responseData);
        });
      });
    }
    return Promise.resolve({});
  }

  public write(changes: StoreItems): Promise<void> {
    if (!changes || Object.keys(changes).length === 0) {
        return Promise.resolve();
    }

    const changesKeys = Object.keys(changes);
    const userStateKey: string = changesKeys.find(change => {
      return change.includes("userData");
    });

    const extractUserStateProps = (changes, key) => {
      return changes[key].userProfile;
    }

    if (userStateKey) {
      const context = {
        userId: this.extractUserId(userStateKey),
        persistUserData: true,
        persistConversationData: false
      };

      let data = null;

      data = {
        userData: {...extractUserStateProps(changes, userStateKey)}
      };

      return new Promise((resolve, reject) => {
        this.v3StorageProvider.saveData(context, data, (err) => {
          if (err) return reject();
          return resolve();
        });
      });
    }
      
    return Promise.resolve();
  }
    
  public delete(keys: string[]): Promise<void> {
    // V3 storage provider does not implement delete functionality
    return Promise.resolve();
  }
    
}
