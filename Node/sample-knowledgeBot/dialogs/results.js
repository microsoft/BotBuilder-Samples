module.exports = function () {
    bot.dialog('/showResults', [
        function (session, args) {
            var msg = new builder.Message(session).attachmentLayout(builder.AttachmentLayout.carousel);
                args.result['value'].forEach(function (musician, i) {
                    msg.addAttachment(
                        new builder.HeroCard(session)
                            .title(musician.Name)
                            .subtitle("Era: " + musician.Era + " | " + "Search Score: " + musician['@search.score'])
                            .text(musician.Description)
                            .images([builder.CardImage.create(session, musician.imageURL)])
                    );
                })
                session.endDialog(msg);
        }
    ])
}