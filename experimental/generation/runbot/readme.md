#RunBot

This bot allows you to easily run Bot Framework Adaptive Dialog declarative samples that use only standard sDK components.  
It starts a web server you can connect the emulator to `http://localhost:5000/api/messages` to interact with your bot.
To use LUIS you need to register your endpoint key by doing `dotnet user-secrets --id RunBot set luis:endpointKey <yourKey>` once.

Command line args:
* **--root <PATH>**: Absolute path to the root directory for declarative resources all *.main.dialog be options.  Default current directory");
* **--region <REGION>**: LUIS endpoint region.  Default westus");
* **--environment <ENVIRONMENT>**: LUIS environment settings to use.  Default is user alias.  This is used to find your `luis.settings.<environment>.<region>.json` settings file for luis.



