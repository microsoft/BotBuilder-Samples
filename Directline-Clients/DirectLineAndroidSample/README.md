# DirectLineAndroidSample
Android Sample for Direct Line API - Microsoft Bot Framework

This android sample showcases the REST API usage of Direct Line API endpoints provided by the Microsoft Bot framework https://docs.botframework.com/en-us/restapi/directline3/

There are 4 steps in a chat conversations.

1. Authentication - you get a primary token from the dev.botframework.com. to get the token go to dev.botframework.com, enable the Direct Line connector, and click on Edit.
![Enable the Direct Line Connector](/images/devbot1.JPG)
![Copy the Primary Key from here](/images/devbot2.JPG)

2. Connect to the API and start the conversation, see the startConversation() function. Sends the Primary Token and receives secondary token,conversationId that you will use in your subsequent calls to Direct Line API.

3. Send the Chat Message to Bot - sendMessageToBot(String messageText) function. This function uses the conversationId and the token received in the first authentication call and sends an Activity to the bot. In this sample we are using an activity of type "message", a message can also contain one or multiple attachments.

4. Receiving response from Bot - Responses can be fetched from Bot by polling a GET API or use the web sockets. In this sample we have used the polling approach. function pollBotResponses - polls the GET API for conversation updates, the bot sends a JSON representation that looks like this

`{
  "activities": [
    {
      "type": "message",
      "id": "61S2RSXVCQYKf034GmZMoT|0000001",
      "timestamp": "2017-01-07T12:03:38.7462471Z",
      "channelId": "directline",
      "from": {
        "id": "user1"
      },
      "conversation": {
        "id": "61S2RSXVCQYKf034GmZMoT"
      },
      "text": "i would like to complain about a product "
    },
    {
      "type": "message",
      "id": "61S2RSXVCQYKf034GmZMoT|0000002",
      "timestamp": "2017-01-07T12:03:42.045473Z",
      "channelId": "directline",
      "from": {
        "id": "formbot",
        "name": "FormBot"
      },
      "conversation": {
        "id": "61S2RSXVCQYKf034GmZMoT"
      },
      "text": "Gee! really sorry to hear that, can you please give me the order id that looks like CRNXXXX, i would lodge a complaint in the system and someone will call you back.",
      "replyToId": "61S2RSXVCQYKf034GmZMoT|0000001"
    }
  ],
  "watermark": "2"
}`

this functions polls the GET API using the runnable handler which polls every 5 seconds.

## Configuration Parameters - AndroidManifest.xml file
Replace the values with Relevant values

`<meta-data android:name="botPrimaryToken" android:value="primarykeyofbot"/>
<meta-data android:name="botName" android:value="yourbotname"/>`


Things to do :

1. The calls are synchronous in nature, need to make them asynchronous.

2. Prepare a SDK for Android Direct Line API.
