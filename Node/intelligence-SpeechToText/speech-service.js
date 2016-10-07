
const uuid = require("node-uuid"),
    request = require("request");

const SPEECH_API_KEY = process.env.MICROSOFT_SPEECH_API_KEY;

var speechApiAccessToken = "";

exports.getTextFromAudioStream = (stream) => {
    return new Promise(
        (resolve, reject) => {
            if (!speechApiAccessToken) {
                try {
                    authenticate(() => {
                        streamToText(stream, resolve, reject);
                    });
                }
                catch (exception) {
                    reject(exception);
                }
            }
            else {
                streamToText(stream, resolve, reject);
            }
        }
    );
}

const authenticate = (callback) => {
    const requestData = {
        url: "https://oxford-speech.cloudapp.net/token/issueToken",
        headers: { "content-type": "application/x-www-form-urlencoded" },
        form: {
            grant_type: "client_credentials",
            client_id: SPEECH_API_KEY,
            client_secret: SPEECH_API_KEY,
            scope: "https://speech.platform.bing.com"
        }
    }

    request.post(requestData, (error, response, body) => {
        if (error) {
            console.error(error);
        }
        else if (response.statusCode != 200) {
            console.error(body);
        }
        else {
            const token = JSON.parse(body);
            speechApiAccessToken = "Bearer " + token.access_token;
            const expires_in_seconds = token.expires_in;

            // We need to refresh the token before it expires.
            setTimeout(authenticate, (expires_in_seconds - 60) * 1000);
            if (callback) {
                callback();
            }
        }
    });
}

const streamToText = (stream, resolve, reject) => {
    const speechApiUrl = [
        "https://speech.platform.bing.com/recognize?scenarios=smd",
        "appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5",
        "locale=en-US",
        "device.os=wp7",
        "version=3.0",
        "format=json",
        "form=BCSSTT",
        "instanceid=0F8EBADC-3DE7-46FB-B11A-1B3C3C4309F5",
        "requestid=" + uuid.v4(),
        ].join("&");

    const speechRequestData = {
        url: speechApiUrl,
        headers: {
            Authorization: speechApiAccessToken,
            "content-type": 'audio/wav; codec="audio/pcm"; samplerate=16000'
        }
    }

    stream.pipe(request.post(speechRequestData, (error, response, body) => {
        if (error) {
            reject(error);
        }
        else if (response.statusCode != 200) {
            reject(body);
        }
        else {
            resolve(JSON.parse(body).header.name);
        }
    }));

}