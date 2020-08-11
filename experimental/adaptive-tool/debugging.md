## Setting up and using Visual Studio Code to run client and server
### setting up
To configure Visual Studio Code you need to add a target in your launch.settings file.

* **Bot: Launch language server and client on vscode** - Configuration for building and launching your client on vscode and connecting to server
Example is:
```json
        {
			"type": "extensionHost",
			"request": "launch",
			"name": "Launch Client",
			"runtimeExecutable": "${execPath}",
			"args": ["--extensionDevelopmentPath=${workspaceRoot}"],
			"outFiles": ["${workspaceRoot}/client/out/**/*.js"],
			"preLaunchTask": {
				"type": "npm",
				"script": "watch"
			},
			"sourceMaps": true
		},
		{
			"type": "node",
			"request": "attach",
			"name": "Attach to LgServer",
			"port": 6010,
			"restart": true,
			"sourceMaps": true,
			"outFiles": ["${workspaceRoot}/server/out/lg/**/*.js"]
		}
```

### Troubleshooting
There are 2 places your bot can be running depending on the tools you are using.

* **Visual Studio** - Visual studio runs using IIS Express.  IIS Express keeps running even after visual studio shuts down
* **Visual Studio Code** - VS code uses **dotnet run** to run your bot.

If you are switching between environments make sure you are talking to the right instance of your bot.