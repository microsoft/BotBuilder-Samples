window.App || (window.App = {});

window.App.createDefaultChannelAdapter = ({ directLine }) => {
    return new window.DirectLine.BotConnectionChannelAdapter(directLine);
};
