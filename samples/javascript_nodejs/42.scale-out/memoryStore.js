
class MemoryStore {
    constructor() {
        this._store = new Map();
    }

    async loadAsync(key) {
        if (this._store.has(key)) {
            return this._store.get(key);
        }

        return [null, null];
    }

    async saveAsync(key, content, eTag) {
        if (eTag !== null && this._store.has(key)) {
            if (eTag !== this._store.get(key)[1]) {
                return false;
            }
        }

        this._store.set(key, [content, Math.random().toString()]);
        return true;
    }
}

module.exports.MemoryStore = MemoryStore;
