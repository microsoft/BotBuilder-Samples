'use strict';

window.App || (window.App = {});

window.App.createAAFAttachmentMiddleware = ({ directLine }) => {
  const { AdaptiveCards2 } = window.App;

  return () => next => ({ attachment, ...others }) => {
    if (attachment.contentType === 'application/vnd.microsoft.card.adaptive') {
      if (attachment.content.appId) {
        return <AdaptiveCards2 content={attachment.content} directLine={directLine} />;
      }

      return (
        <div className="app__ac1-card">
          <div>{next({ attachment, ...others })}</div>
          <footer className="app__ac1-card__footer">This card is rendered using Adaptive Cards 1.2.5.<br />If you want to render using AAF, make sure the card has "appId" set.</footer>
        </div>
      );
    }

    return next({ attachment, ...others });
  };
};
