var builder = require('botbuilder');

const defaultSettings = {
    showMoreTitle: 'There are more items',
    showMoreValue: 'Next Page',
    selectTemplate: 'Select: ',
    pageSize: 10,
    unknownOption: 'I couldn\'t understand your selection. Please try again.'
};

module.exports = {
    create: function (getPageFunc, getItemFunc, itemToCardFunc, settings) {
        // parameter validation
        settings = Object.assign({}, defaultSettings, settings);
        if (typeof (getPageFunc) !== 'function') {
            throw new Error('getPageFunc must be a function');
        }

        if (typeof (getItemFunc) !== 'function') {
            throw new Error('getItemFunc must be a function');
        }

        if (typeof (itemToCardFunc) !== 'function') {
            throw new Error('itemToCardFunc must be a function');
        }

        // map item info into HeroCard
        var asCard = function (cardInfo) {
            var card = new builder.HeroCard()
                .title(cardInfo.title)
                .buttons([
                    new builder.CardAction()
                        .type('imBack')
                        .value(settings.selectTemplate + cardInfo.title)
                        .title(cardInfo.buttonLabel)
                ]);

            if (cardInfo.subtitle) {
                card = card.subtitle(cardInfo.subtitle);
            }

            if (cardInfo.imageUrl) {
                card = card.images([new builder.CardImage().url(cardInfo.imageUrl).alt(cardInfo.title)])
            }

            return card;
        };

        // return dialog handler funciton
        return function (session, args, next) {
            var pageNumber = session.dialogData.pageNumber || 1;
            var input = session.message.text;
            if (!!input && input.toLowerCase() === settings.showMoreValue.toLowerCase()) {
                // next page
                pageNumber++;
            } else if (!!input && isSelection(input, settings.selectTemplate)) {
                // Validate selection
                var selectedName = input.substring(settings.selectTemplate.length);
                getItemFunc(selectedName).then((selectedItem) => {
                    if (!selectedItem) {
                        return session.send(settings.unknownOption);
                    }

                    // reset page
                    session.dialogData.pageNumber = null;

                    // return selection to dialog stack
                    return next({ selected: selectedItem });
                });

                return;
            }

            // retrieve from service and send items
            getPageFunc(pageNumber, settings.pageSize).then((pageResult) => {
                // save current page number
                session.dialogData.pageNumber = pageNumber;

                // items carousel
                var cards = pageResult.items
                    .map(itemToCardFunc)
                    .map(asCard);
                var message = new builder.Message(session)
                    .attachmentLayout(builder.AttachmentLayout.carousel)
                    .attachments(cards);
                session.send(message);

                // more items link
                if (pageResult.totalCount > pageNumber * settings.pageSize) {
                    var moreCard = new builder.HeroCard(session)
                        .title(settings.showMoreTitle)
                        .buttons([
                            builder.CardAction.imBack(session, settings.showMoreValue, settings.showMoreValue)
                        ]);
                    session.send(new builder.Message(session).addAttachment(moreCard));
                }
            });
        }
    }
};

function isSelection(input, selectTemplate) {
    return input.toLowerCase().indexOf(selectTemplate.toLowerCase()) === 0;
}