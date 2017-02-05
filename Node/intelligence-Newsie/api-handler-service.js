const request = require('request');

exports.getResponse = function (url, requestParameters, headers) {
    return new Promise(
        (resolve, reject) => {
            if (url && requestParameters && headers) {
                const requestData = {
                    url: url + constructRequestParams(requestParameters),
                    headers: headers
                }

                request.get(requestData, (error, response, body) => {
                    if (error) {
                        reject(error);
                    } else if (response.statusCode != 200) {
                        reject(body);
                    } else {
                        resolve(body);
                    }
                });
            } else {
                reject("");
            }
        }
    );
}

function constructRequestParams(data) {
    var ret = [];
    for (var d in data)
        ret.push(encodeURIComponent(d) + '=' + encodeURIComponent(data[d]));
    return "?" + ret.join('&');
}