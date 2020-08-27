@echo off
rd /s /q %temp%\sandwichTest.out
call ..\bin\run dialog:generate ../../library/test/forms/sandwich.schema -o %temp%/sandwichTest.out --verbose
call ..\bin\run dialog:generate:test ../../library/test/transcripts/sandwich.transcript sandwich -o %temp%
call gencopy %temp%\sandwichTest.out %REPOS%\botbuilder-dotnet\tests\Microsoft.Bot.Builder.Dialogs.Adaptive.Tests\Tests\GeneratorTests\sandwich\ %1
copy %temp%\sandwich.test.dialog %REPOS%\botbuilder-dotnet\tests\Microsoft.Bot.Builder.Dialogs.Adaptive.Tests\Tests\GeneratorTests\Generator_sandwich.test.dialog