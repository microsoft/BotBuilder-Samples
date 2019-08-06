@echo off
setlocal
SET password=%1
SET exportSettings=%2
SET repoName=srcRepo
SET repoUrl=file:///%HOMEDRIVE:~0,1%/%HOMEPATH:~1%/site/%repoName%
SET download=bot-src

echo %repoUrl%

rem cd to project root
pushd ..\wwwroot

rem init git
call git init
call git config user.name "botframework"
call git config user.email "util@botframework.com"
call git add .
call git commit -m "prepare to download source"
call git remote add srcRepo %repoUrl%
popd

rem init upstream
pushd %HOME%\site
mkdir srcRepo
cd srcRepo
call git init --bare
popd

rem push to upstream
pushd ..\wwwroot
call git push --set-upstream srcRepo master
popd

rem clone srcRepo
pushd %HOME%\site
call git clone %repoUrl% %download%
rem delete .git
cd %download%
call rm -r -f .git
popd

rem prepare for publish
pushd %HOME%\site\%download%
mkdir Properties\PublishProfiles
pushd Properties\PublishProfiles
type ..\..\PostDeployScripts\publishProfile.xml.template | sed -e s/\{WEB_SITE_NAME\}/%WEBSITE_SITE_NAME%/g > %WEBSITE_SITE_NAME%-Web-Deploy.pubxml
popd

set SOLUTION_NAME=
for /f "delims=" %%a in ('dir /b *.sln') do @set SOLUTION_NAME=%%a

type PostDeployScripts\publish.cmd.template | sed -e s/\{SOLUTION_NAME\}/%SOLUTION_NAME%/g | sed -e s/\{PUBLISH_PROFILE\}/%WEBSITE_SITE_NAME%-Web-Deploy.pubxml/g | sed -e s/\{PASSWORD\}/%password%/g > publish.cmd
type PostDeployScripts\publishSettings.xml.template | sed -e s/\{WEB_SITE_NAME\}/%WEBSITE_SITE_NAME%/g | sed -e s/\{PASSWORD\}/%password%/g > PostDeployScripts\%WEBSITE_SITE_NAME%.PublishSettings

popd

echo %exportSettings%

if "%exportSettings%" == "" goto zip
pushd %HOME%\site\%download%
echo 'export app settings'
call :APP_SETTINGS
popd


:zip
rem preare the zip file
%HOMEDRIVE%\7zip\7za a %HOME%\site\%download%.zip %HOME%\site\%download%\*

rem cleanup git stuff
pushd ..\wwwroot
call rm -r -f .git
popd

pushd %HOME%\site
call rm -r -f %download%
call rm -r -f %repoName%
popd

endlocal

goto :EOF

:APP_SETTINGS
echo 'dump app settings'
node %HOME%\site\wwwroot\PostDeployScripts\mergeSettings.js
exit /b
