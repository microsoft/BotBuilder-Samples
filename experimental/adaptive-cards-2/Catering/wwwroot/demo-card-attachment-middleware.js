'use strict';

window.App || (window.App = {});

window.App.createDemoCardAttachmentMiddleware = ({ directLine }) => {
  const { AdaptiveCards2, createDemoCardChannelAdapter, sampleCard, sampleData } = window.App;

  return () => next => ({ activity, attachment, ...others }) => {
    // We are temporarily hacking the response of the bot.
    // When the bot say "Showing  markdown" with an AC attachment, we show a different card.
    if (activity.text === 'Showing  markdown' && attachment.contentType === 'application/vnd.microsoft.card.adaptive') {
      const content = { ...sampleCard, $data: sampleData };

      return <AdaptiveCards2 channelAdapter={createDemoCardChannelAdapter()} content={content} />;
    }

    return next({ activity, attachment, ...others });
  };
};
