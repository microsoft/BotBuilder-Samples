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

  private storageType: string;
  private v3StorgaeProvider: any;
  private userId: string;
  
  public constructor(v3StorgaeProvider: any) {
    if (!v3StorgaeProvider) {
      throw new Error('A storage provider must be provided.');
    }

    this.storageType = v3StorgaeProvider.storageClient.constructor.name;
    this.v3StorgaeProvider = v3StorgaeProvider;
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
    
      switch (this.storageType) {
        // CosmosDB
        case 'DocumentDbClient':
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.getData(context, (err, data) => {
              if (err) return reject();
              if (!data || !data.userData) return resolve({});
              const responseData = this.formatDataResponse(context.userId, data.userData);
              return resolve(responseData);
            });
          });
        case 'AzureTableClient':
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.storageClient.retrieve(context.userId, 'userData', (err, data) => {
              if (err) return reject();
              if (!data) return resolve({});
              const responseData = this.formatDataResponse(data.partitionKey, data.data);
              return resolve(responseData);
            });
          });
        case 'AzureSqlClient':
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.storageClient.retrieve(context.userId, 'userData', (err, data) => {
              if (err) console.log(err);
              if (err) return reject();
              if (!data) return resolve({});
              const responseData = this.formatDataResponse(context.userId, data.data);
              return resolve(responseData);
            });
          });
      }
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
          
      switch (this.storageType) {
        // CosmosDB
        case 'DocumentDbClient':
          data = {
            userData: {...extractUserStateProps(changes, userStateKey)}
          };
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.saveData(context, data, (err) => {
              if (err) return reject();
              return resolve();
            });
          });
        case 'AzureTableClient':
          data = {...extractUserStateProps(changes, userStateKey)};
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.storageClient.insertOrReplace(context.userId, 'userData', data, false, (err, data) => {
              if (err) return reject();
              return resolve(data);
            });
          });
        case 'AzureSqlClient':
          data = {...extractUserStateProps(changes, userStateKey)};
          return new Promise((resolve, reject) => {
            this.v3StorgaeProvider.storageClient.insertOrReplace(context.userId, 'userData', data, false, (err, data) => {
              if (err) return reject();
              return resolve(data);
            });
          });
      }
    }
      
    return Promise.resolve();
  }
    
  public delete(keys: string[]): Promise<void> {
    // V3 storage provider does not implement delete functionality
    return Promise.resolve();
  }
    
}