@echo off
rd /s /q %temp%\sandwichTest.out
call ..\bin\run dialog:generate ../../library/test/forms/sandwich.schema -o %temp%/sandwichTest.out --verbose -p sandwichTest
call gencopy %temp%\sandwichTest.out %REPOS%\botbuilder-dotnet\tests\Microsoft.Bot.Builder.Dialogs.Adaptive.Tests\Tests\GeneratorTests\sandwich\ %1
