// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    Aborter,
    SharedKeyCredential,
    StorageURL,
    ServiceURL,
    ContainerURL,
    BlockBlobURL
} = require('@azure/storage-blob');

class BlobStore {
    constructor(accountName, accountKey, containerName) {
        const sharedKeyCredential = new SharedKeyCredential(accountName, accountKey);
        const pipeline = StorageURL.newPipeline(sharedKeyCredential, { retryOptions: { maxTries: 4 } });
        const serviceURL = new ServiceURL(`https://${accountName}.blob.core.windows.net`, pipeline);
        this.containerURL = ContainerURL.fromServiceURL(serviceURL, containerName);
    }

    async load(key) {
        const blockBlobURL = BlockBlobURL.fromContainerURL(this.containerURL, key);
        try {
            const downloadResponse = await blockBlobURL.download(Aborter.none, 0);
            const json = await BlobStore.streamToString(downloadResponse.blobDownloadStream);
            return { value: JSON.parse(json), eTag: downloadResponse.eTag };
        }
        catch (err) {
            if (err.statusCode === 404) {
                return { value: {}, eTag: null };
            }
            throw err;
        }
    }

    async save(key, obj, eTag) {
        if (obj) {
            const blockBlobURL = BlockBlobURL.fromContainerURL(this.containerURL, key);
            const json = JSON.stringify(obj);
            const options = {
                blobHTTPHeaders: { blobContentType: 'application/json' },
                accessConditions: { modifiedAccessConditions: { ifMatch: eTag } }
            };
            try {
                const uploadBlobResponse = await blockBlobURL.upload(Aborter.none, json, json.length, options);
            }
            catch (err) {
                if (err.statusCode === 412) {
                    return false;
                }
                else {
                    throw err;
                }
            }
        }
        return true;
    }

    static async streamToString(readableStream) {
        return new Promise((resolve, reject) => {
            const chunks = [];
            readableStream.on('data', data => {
                chunks.push(data.toString());
            });
            readableStream.on('end', () => {
                resolve(chunks.join(''));
            });
            readableStream.on('error', reject);
        });
    }
}

module.exports.BlobStore = BlobStore;
