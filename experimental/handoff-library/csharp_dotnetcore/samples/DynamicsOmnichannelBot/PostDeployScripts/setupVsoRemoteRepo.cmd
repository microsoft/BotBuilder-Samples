@echo off
setlocal
rem ------------------------------------------------------------------------------------------
rem setupVsoRemoteRepo [vsoRemote] [vsoUserName] [vsoPersonalAccessToken] [projName{optional}]
rem create and populate VSO git repo for the ABS code instance
rem 
rem vsoRmote: url of the VSO site (e.g. https://awesomebot.visualstudio.com )
rem vosUserName: user account name of the personal access token
rem vsoPersonalAccessToken: the personal access token used to access VSO REST api
rem projName the name of the project to create (default to WEBSITE_SITE_NAME)
rem ------------------------------------------------------------------------------------------
set remoteUrl=%1
set remoteUser=%2
set remotePwd=%3
set projName=%4
if '%projName%'=='' set projName=%WEBSITE_SITE_NAME%
set vstsRoot=%remoteUrl%
set repoUrl=https://%remoteUser%:%remotePwd%@%remoteUrl:~8%/_git/%projName%
set vstsCreateProject=https://%remoteUser%:%remotePwd%@%remoteUrl:~8%/defaultcollection/_apis/projects?api-version=3.0

rem use curl to create project
pushd ..\wwwroot
type PostDeployScripts\vsoProject.json.template | sed -e s/\{WEB_SITE_NAME\}/%projName%/g > %TEMP%\vsoProject.json
call curl -H "Content-Type: application/json"  -d "@%TEMP%\vsoProject.json" -X POST  %vstsCreateProject%
rm %TEMP%\vsoProject.json
rem sleep for 15 seconds for the creation to complete, this is a wild guess
call sleep 15
popd

popd
rem cd to project root
pushd ..\wwwroot

rem init git
call git init
call git config user.name "%remoteUser%"
call git config user.password "%remotePwd%"
call git config user.email "util@botframework.com"
call git add .
call git commit -m "prepare to setup source control"
call git push %repoUrl% master
popd


rem cleanup git stuff
pushd ..\wwwroot
call rm -r -f .git
popd

endlocal