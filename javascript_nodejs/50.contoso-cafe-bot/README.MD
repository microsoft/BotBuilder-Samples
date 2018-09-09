ludown parse toqna -l dialogs\chitChat\resources -o cognitiveModels --verbose -n chitchat.qna
ludown parse toqna -l dialogs\qna\resources -o cognitiveModels -n contosoCafe.qna


<INSERT AT MOST ONE PARAGRAPH DESCRIPTION OF WHAT THIS SAMPLE DOES> 

# Concepts introduced in this sample
<DESCRIPTION OF THE CONCEPTS>


# To try this sample
-	<STEPS TO CLONE REPO AND GET SETUP>
> ludown parse toluis --in dialogs\mainDialog\resources\cafeDispatchModel.lu -o cognitiveModels -n cafeDispatchModel.luis --verbose
> ludown parse toluis --in dialogs\bookTable\resources\turn-N.lu -o cognitiveModels -n cafeBotBookTableTurnN.luis --verbose
> ludown parse toluis --in dialogs\whoAreYou\resources\getUserProfile.lu -o cognitiveModels -n getUserProfile.luis --verbose
> ludown parse toqna --in dialogs\mainDialog\resources\cafeFAQ_ChitChat.lu --verbose -o cognitiveModels -n cafeFaqChitChat.qna -a

> luis import application --in cognitiveModels\cafeDispatchModel.luis --authoringKey be30825b782843dcbbe520ac5338f567 --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin

> luis import application --in cognitiveModels\cafeBotBookTableTurnN.luis --authoringKey be30825b782843dcbbe520ac5338f567 --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin

> luis import application --in cognitiveModels\getUserProfile.luis --authoringKey be30825b782843dcbbe520ac5338f567 --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin

> qnamaker create kb --in cognitiveModels\cafeFaqChitChat.qna --subscriptionKey d30ebbcc44ef4f07bae1a0e31b69f709 --msbot | msbot connect qna --stdin

> Update LUIS version
> msbot get cafeDispatchModel --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
> msbot get cafeDispatchModel --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis import version --stdin --in cognitiveModels\cafeDispatchModel.luis
> msbot get cafeDispatchModel --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis delete version --stdin --versionId 0.1_old


> msbot get cafeBotBookTableTurnN --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
> msbot get cafeBotBookTableTurnN --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis import version --stdin --in cognitiveModels\cafeBotBookTableTurnN.luis
> msbot get cafeBotBookTableTurnN --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis delete version --stdin --versionId 0.1_old

> msbot get getUserProfile --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
> msbot get getUserProfile --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis import version --stdin --in cognitiveModels\getUserProfile.luis
> msbot get getUserProfile --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis delete version --stdin --versionId 0.1_old

> Train
> msbot get cafeDispatchModel --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis train version --wait --stdin --versionId 0.1
> msbot get cafeBotBookTableTurnN --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis train version --wait --stdin --versionId 0.1
> msbot get getUserProfile --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis train version --wait --stdin --versionId 0.1

> Publish
> msbot get cafeDispatchModel --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis publish version --stdin --versionId 0.1 --region westus
> msbot get cafeBotBookTableTurnN --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis publish version --stdin --versionId 0.1 --region westus
> msbot get getUserProfile --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | luis publish version --stdin --versionId 0.1 --region westus

> Replace qna Kb
> msbot get cafeFaqChitChat --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | qnamaker replace kb --in cognitiveModels\cafeFaqChitChat.qna --stdin

> Train and publish QnA
> msbot get cafeFaqChitChat --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | qnamaker publish kb --stdin

> QnA alterations
> msbot get cafeFaqChitChat --secret Y3Guprzmw+PrQTgJh8QJ989UeJHqnw22705paZJyU3U= | qnamaker replace alterations --in cognitiveModels\cafeFaqChitChat.qna_Alterations.json --stdin


## Prerequisites
-	<REQUIRED TOOLS, VERSIONS>
-	<STEPS TO GET SET UP WITH THE SAMPLE. E.g. RUN AN INCLUDED SCRIPT OR MANUALLY DO SOMETHING ETC>

NOTE: <ANY NOTES ABOUT THE PREREQUISITES OR ALTERNATE THINGS TO CONSIDER TO GET SET UP>

## Visual studio
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO>

## Visual studio code
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO CODE>

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to samples\8.AspNetCore-LUIS-Bot folder
- Select AspNetCore-LUIS-Bot.bot file

# Further reading
-	<LINKS TO ADDITIONAL READING>