// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BlobServiceClient, StorageSharedKeyCredential } = require('@azure/storage-blob');

class BlobStore {
    constructor(accountName, accountKey, containerName) {
        if (!accountName) {
            throw new Error('accountName is required');
        }

        if (!accountKey) {
            throw new Error('accountKey is required');
        }

        if (!containerName) {
            throw new Error('containerName is required');
        }

        const sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
        const blobServiceClient = new BlobServiceClient(`https://${ accountName }.blob.core.windows.net`, sharedKeyCredential);
        this.containerClient = blobServiceClient.getContainerClient(containerName);
    }

    async loadAsync(key) {
        if (!key) {
            throw new Error('key is required');
        }

        const blobClient = this.containerClient.getBlockBlobClient(key);
        try {
            const downloadBlockBlobResponse = await blobClient.download();
            const content = await streamToString(downloadBlockBlobResponse.readableStreamBody);
            const obj = JSON.parse(content);
            const etag = downloadBlockBlobResponse.properties.etag;
            return { content: obj, etag: etag };
        } catch (error) {
            if (error.statusCode === 404) {
                return { content: {}, etag: null };
            }
            throw error;
        }
    }

    async saveAsync(key, obj, etag) {
        if (!key) {
            throw new Error('key is required');
        }

        if (!obj) {
            throw new Error('obj is required');
        }

        const blobClient = this.containerClient.getBlockBlobClient(key);
        blobClient.properties.contentType = 'application/json';
        const content = JSON.stringify(obj);
        if (etag) {
            try {
                await blobClient.upload(content, content.length, { conditions: { ifMatch: etag } });
            } catch (error) {
                if (error.statusCode === 412) {
                    return false;
                }
                throw error;
            }
        } else {
            await blobClient.upload(content, content.length);
        }

        return true;
    }
}

async function streamToString(readableStream) {
    return new Promise((resolve, reject) => {
        const chunks = [];
        readableStream.on('data', (data) => {
            chunks.push(data.toString());
        });
        readableStream.on('end', () => {
            resolve(chunks.join(''));
        });
        readableStream.on('error', reject);
    });
}

module.exports.BlobStore = BlobStore;
