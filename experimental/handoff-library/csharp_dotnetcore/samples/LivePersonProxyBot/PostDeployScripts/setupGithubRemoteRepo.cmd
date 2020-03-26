@echo off
setlocal
rem ------------------------------------------------------------------------------------------
rem setupVsoRemoteRepo [remoteUser] [personalAccessToken] [projName{optional}]
rem create and populate VSO git repo for the ABS code instance
rem 
rem remoteUser: user account name of the personal access token
rem personalAccessToken: the personal access token used to access github REST API (requires repos scope)
rem projName the name of the project to create (default to WEBSITE_SITE_NAME)
rem ------------------------------------------------------------------------------------------
set remoteUrl=https://api.github.com
set remoteUser=%1
set remotePwd=%2
set projName=%3
if '%projName%'=='' set projName=%WEBSITE_SITE_NAME%
set repoUrl=https://%remoteUser%:%remotePwd%@github.com/%remoteUser%/%projName%.git
rem use curl to create project
pushd ..\wwwroot
type PostDeployScripts\githubProject.json.template | sed -e s/\{WEB_SITE_NAME\}/%projName%/g > %TEMP%\githubProject.json
call curl -H "Content-Type: application/json" -u %remoteUser%:%remotePwd% -d "@%TEMP%\githubProject.json" -X POST  %remoteUrl%/user/repos
rem rm %TEMP%\githubProject.json
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