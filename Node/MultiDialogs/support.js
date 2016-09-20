module.exports = {
    Label: 'Support',
    Dialog: function (session) {
        // Generate ticket
        var tickerNumber = Math.ceil(Math.random() * 20000);

        // Reply and return to parent dialog
        session.send('Your message \'%s\' was registered. Once we resolve it; we will get back to you.', session.message.text);
        session.endDialogWithResult({
            response: tickerNumber
        });
    }
};