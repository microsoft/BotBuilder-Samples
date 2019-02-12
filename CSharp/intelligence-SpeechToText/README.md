# Speech To Text Bot Sample

A sample bot that illustrates how to use the Microsoft Cognitive Services Speech API to analyze an audio file and convert the audio stream to text.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/SpeechToText]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/SpeechToText]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Subscribe to Congnitive Speech API services [here](https://azure.microsoft.com/en-us/services/cognitive-services/speech/) to obtain a Trial API Key and update the `MicrosoftSpeechApiKey` and `MicrosoftSpeechServiceRegion` keys in [Web.config](Web.config).

````XML
  <appSettings>
    <add key="MicrosoftSpeechApiKey" value="PUT-YOUR-OWN-API-KEY-HERE" />
	<add key="MicrosoftSpeechServiceRegion" value="PUT-YOUR-OWN-API-REGION-HERE" />
  </appSettings>
````

### Usage

Attach an audio file (wav format).

### Code Highlights

Microsoft Cognitive Services provides a Speech Recognition API to convert audio into text. Check out [Speech Services](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/rest-apis) more information. In this sample we are using the Microsoft.CognitiveServices.Speech library [Nuget](https://www.nuget.org/packages/Microsoft.CognitiveServices.Speech).

In this sample we are using the API to get the text and send it back to the user. Check out the use of the `MicrosoftCognitiveSpeechService.GetTextFromActivity()` method in the [Controllers/MessagesController](Controllers/MessagesController.cs) class.
````C#
var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("application/octet-stream"));
if (audioAttachment != null)
{
    var stream = await GetAudioStream(connector, audioAttachment);
    string text = await MicrosoftCognitiveSpeechService.GetTextFromAudioAsync(stream);
    message = ProcessText(text);
}
else
{
    message = "Did you upload an audio file? I'm more of an audible person. Try sending me a wav file";
}
````

and here is the relevant pieces from the implementation of `MicrosoftCognitiveSpeechService.GetTextFromAudioAsync()` in [Services/MicrosoftCognitiveSpeechService.cs](Services/MicrosoftCognitiveSpeechService.cs)
````C#
/// <summary>
/// Gets text from an audio stream.
/// </summary>
/// <param name="audiostream">Audio stream</param>
/// <returns>Transcribed text</returns>
public static async Task<string> GetTextFromAudioAsync(Stream audiostream)
{
    string recognizedText = string.Empty;
            
    // Creates an instance of a speech config with specified subscription key and service region.
    // Replace with your own subscription key and service region (e.g., "westus").
    var config = SpeechConfig.FromSubscription(WebConfigurationManager.AppSettings["MicrosoftSpeechApiKey"], WebConfigurationManager.AppSettings["MicrosoftSpeechServiceRegion"]);

    var stopRecognition = new TaskCompletionSource<int>();

    // Creates a speech recognizer using file as audio input.
    // Replace with your own audio file name.
    using (var audioInput = Helper.OpenWavStream(audiostream))
    {
        using (var recognizer = new SpeechRecognizer(config, audioInput))
        {
            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    recognizedText = e.Result.Text;
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    recognizedText = "NOMATCH: Speech could not be recognized.";
                }
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }

    return recognizedText;
}
````

### Outcome

You will see the following when connecting the Bot to the Emulator and send it an audio file and a command:

Input:

["What's the weather like?"](audio/whatstheweatherlike.wav)

Output:

![Sample Outcome](images/outcome-emulator.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Microsoft Cognitive Services Speech API please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Microsoft Cognitive Speech Services](https://www.microsoft.com/cognitive-services/en-us/speech-api)
