'use strict';

window.App || (window.App = {});

const {
  AAFRuntime: { AdaptiveApplet },
  React: { useEffect, useMemo, useRef }
} = window;

const AdaptiveCards2 = ({ channelAdapter, content, directLine }) => {
  const ref = useRef();

  const applet = useMemo(() => new AdaptiveApplet(), []);

  useEffect(() => {
    applet.channelAdapter = channelAdapter || window.App.createDefaultChannelAdapter({ directLine });
  }, [applet, channelAdapter]);

  useEffect(() => {
    applet.setCard(content);

    const { current } = ref;

    current.appendChild(applet.renderedElement);

    return () => {
      while (current.children.length) {
        current.removeChild(current.children[0]);
      }
    };
  }, [content]);

  return <div ref={ref} />;
};

window.App.AdaptiveCards2 = AdaptiveCards2;
