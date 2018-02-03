# Speech To Text Bot Sample

A sample bot that illustrates how to use the Microsoft Cognitive Services Bing Speech API to analyze an audio file and convert the audio stream to text.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/SpeechToText]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/SpeechToText]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Subscribe to Bing Speech API services [here](https://azure.microsoft.com/en-us/services/cognitive-services/speech/) to obtain a Trial API Key and update the `MicrosoftSpeechApiKey` key in key in [Web.config](Web.config) file to try it out further.

````XML
  <appSettings>
    <add key="MicrosoftSpeechApiKey" value="PUT-YOUR-OWN-API-KEY-HERE" />
  </appSettings>
````

### Usage

Attach an audio file (wav format).

### Code Highlights

Microsoft Cognitive Services provides a Speech Recognition API to convert audio into text. Check out [Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api) for a complete reference of Speech APIs available. In this sample we are using the Speech Recognition API using the [REST API](https://www.microsoft.com/cognitive-services/en-us/Speech-api/documentation/API-Reference-REST/BingVoiceRecognition).

In this sample we are using the API to get the text and send it back to the user. Check out the use of the `MicrosoftCognitiveSpeechService.GetTextFromAudioAsync()` method in the [Controllers/MessagesController](Controllers/MessagesController.cs) class.
````C#
var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav"));
if (audioAttachment != null)
{
    using (var client = new HttpClient())
    {
        var stream = await client.GetStreamAsync(audioAttachment.ContentUrl);
        var text = await this.speechService.GetTextFromAudioAsync(stream);
        message = ProcessText(activity.Text, text);
    }
}
````

and here is the implementation of `MicrosoftCognitiveSpeechService.GetTextFromAudioAsync()` in [Services/MicrosoftCognitiveSpeechService.cs](Services/MicrosoftCognitiveSpeechService.cs)
````C#
/// <summary>
/// Gets text from an audio stream.
/// </summary>
/// <param name="audiostream"></param>
/// <returns>Transcribed text. </returns>
public async Task<string> GetTextFromAudioAsync(Stream audiostream)
{
    var requestUri = @"https://speech.platform.bing.com/recognize?scenarios=smd&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale=en-US&device.os=bot&version=3.0&format=json&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3&requestid=" + Guid.NewGuid();

    using (var client = new HttpClient())
    {
        var token = Authentication.Instance.GetAccessToken();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);

        using (var binaryContent = new ByteArrayContent(StreamToBytes(audiostream)))
        {
            binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

            var response = await client.PostAsync(requestUri, binaryContent);
            var responseString = await response.Content.ReadAsStringAsync();
            try
            {
                dynamic data = JsonConvert.DeserializeObject(responseString);
                return data.header.name;
            }
            catch (JsonReaderException ex)
            {
                throw new Exception(responseString, ex);
            }
        }
    }
}
````

### Outcome

You will see the following when connecting the Bot to the Emulator and send it an audio file and a command:

Input:

["What's the weather like?"](audio/whatstheweatherlike.wav)

Output:

![Sample Outcome](images/outcome-emulator.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Microsoft Cognitive Services Bing Speech API please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Microsoft Cognitive Services Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api)
