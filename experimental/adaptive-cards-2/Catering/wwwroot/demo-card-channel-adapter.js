'use strict';

window.App = window.App || {};

const {
  AAFRuntime: { ActivityStatus, ChannelAdapter }
} = window;

class LocalChannelAdapter extends ChannelAdapter {
  sendRequestAsync(request) {
    return new Promise((resolve, reject) => {
      switch (request.activity.value.action.verb) {
        // case 'succeedReturnCard':
        case 'remoteSucceedReturnCard':
          window.setTimeout(() => {
            resolve({
              request: request,
              status: ActivityStatus.Success,
              content: JSON.stringify(window.App.sampleRefreshCard)
              // status: request.attemptNumber === 2 ? ActivityStatus.Success : ActivityStatus.Failure,
              // content: request.attemptNumber === 2 ? JSON.stringify(window.App.sampleRefreshCard) : undefined
            });
          }, 3000);
          break;

        // case 'succeedReturnString':
        case 'remoteSucceedReturnString':
          resolve({
            request: request,
            status: ActivityStatus.Success,
            content: 'It worked!'
          });
          break;

        // case 'fail':
        case 'remoteFailedUnauthenticated':
        case 'remoteFailedUnrecoverable':
          resolve({
            request: request,
            status: ActivityStatus.Failure,
            content: 'It failed miserably...'
          });
          break;

        case 'exception':
        default:
          reject("It didn't work!");
          break;
      }
    });
  }
}

window.App.createDemoCardChannelAdapter = () => {
  return new LocalChannelAdapter();
};
