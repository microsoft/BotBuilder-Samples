@echo off
ECHO Generating LUIS and QnA Maker models from .lu files ..
call ludown parse toluis --in dialogs\dispatcher\resources\cafeDispatchModel.lu -o cognitiveModels --out cafeDispatchModel.luis -n cafeDispatchModel
call ludown parse toluis --in dialogs\bookTable\resources\turn-N.lu -o cognitiveModels -n cafeBotBookTableTurnN --out cafeBotBookTableTurnN.luis
call ludown parse toluis --in dialogs\whoAreYou\resources\getUserProfile.lu -o cognitiveModels -n getUserProfile.luis
call ludown parse toqna --in dialogs\dispatcher\resources\cafeFAQ_ChitChat.lu -o cognitiveModels -n cafeFaqChitChat.qna -a

ECHO Updating LUIS models .. 
call msbot get cafeDispatchModel | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
call msbot get cafeDispatchModel | luis import version --stdin --in cognitiveModels\cafeDispatchModel.luis
call msbot get cafeDispatchModel | luis delete version --stdin --versionId 0.1_old --force
call msbot get cafeBotBookTableTurnN | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
call msbot get cafeBotBookTableTurnN | luis import version --stdin --in cognitiveModels\cafeBotBookTableTurnN.luis
call msbot get cafeBotBookTableTurnN | luis delete version --stdin --versionId 0.1_old --force
call msbot get getUserProfile | luis rename version --newVersionId 0.1_old --stdin --versionId 0.1
call msbot get getUserProfile | luis import version --stdin --in cognitiveModels\getUserProfile.luis
call msbot get getUserProfile | luis delete version --stdin --versionId 0.1_old --force

ECHO Training LUIS models ..
call msbot get cafeDispatchModel | luis train version --wait --stdin --versionId 0.1
call msbot get cafeBotBookTableTurnN | luis train version --wait --stdin --versionId 0.1
call msbot get getUserProfile | luis train version --wait --stdin --versionId 0.1

ECHO Publishing LUIS models ..
call msbot get cafeDispatchModel | luis publish version --stdin --versionId 0.1 --region westus
call msbot get cafeBotBookTableTurnN | luis publish version --stdin --versionId 0.1 --region westus
call msbot get getUserProfile | luis publish version --stdin --versionId 0.1 --region westus

ECHO Replacing QnA Maker KB contents .. 
call msbot get cafeFaqChitChat | qnamaker replace kb --in cognitiveModels\cafeFaqChitChat.qna --stdin

ECHO Publishing QnA Maker model ..
call msbot get cafeFaqChitChat | qnamaker publish kb --stdin

ECHO Updating QnA Maker alterations ..
call msbot get cafeFaqChitChat | qnamaker replace alterations --in cognitiveModels\cafeFaqChitChat.qna_Alterations.json --stdin

ECHO All updates complete ..
@echo on

