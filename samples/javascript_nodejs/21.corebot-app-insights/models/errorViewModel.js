class ErrorViewModel {
    constructor(requestId = null) {
        this.requestId = requestId;
    }
    
    showRequestId() {
        return Boolean(this.requestId);
    }
}
