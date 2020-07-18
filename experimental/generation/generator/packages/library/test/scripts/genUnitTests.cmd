@echo off
rd /s /q %temp%\unittests.out
call ..\..\..\..\bin\run dialog:generate ../forms/unittests.schema -o %temp%/unittests.out --force --verbose
call gencopy %temp%\unittests.out %REPOS%\botbuilder-dotnet\tests\Microsoft.Bot.Builder.Dialogs.Adaptive.Tests\Tests\GeneratorTests\unittests\ %1

