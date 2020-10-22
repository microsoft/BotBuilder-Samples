@echo off
for /l %%i in (1, 1, 23) do (
    call rd /s /q %temp%\unittest%%i.out
    call ..\bin\run dialog:generate ../../library/test/forms/unittest%%i.schema -o %temp%/unittest%%i.out --force --verbose
    call gencopy %temp%\unittest%%i.out %REPOS%\botbuilder-dotnet\tests\Microsoft.Bot.Builder.Dialogs.Adaptive.Tests\Tests\GeneratorTests\unittest%%i\ %1
)