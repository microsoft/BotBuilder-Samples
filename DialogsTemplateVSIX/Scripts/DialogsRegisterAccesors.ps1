param([string]$inputName="botClass")

$FileName = "*Startup.cs"
$FileOriginal = Get-Content $FileName
$Patern = "public void ConfigureServices"
$PaternAddBot = "services.AddBot"

<# create empty Array and use it as a modified file... #>
[String[]] $FileModified = @() 

Foreach ($Line in $FileOriginal)
{    
    $FileModified += $Line
	
	if ($Line -match $PaternAddBot) 
    {
		$foreach.movenext()
		$FileModified += "			{"
		$FileModified += "				// The Memory Storage used here is for local bot debugging only. When the bot"
        $FileModified += "	        	// is restarted, everything stored in memory will be gone."
        $FileModified += "	        	IStorage dataStore = new MemoryStorage();"
		$FileModified += ""		
		$FileModified += "				// Create and add conversation state (for empty bot template only)."
		$FileModified += "				var conversationState = new ConversationState(dataStore);"
		$FileModified += "				services.AddSingleton(conversationState);"
		$FileModified += ""
		$FileModified += "				// Create and add user state."
		$FileModified += "				var userState = new UserState(dataStore);"
		$FileModified += "				services.AddSingleton(userState);"
		$FileModified += ""
	}
	
	
	if ($Line -match $patern) 
    {
		<# add ... #>
		$foreach.movenext()
		$FileModified += "		{"
		$FileModified += "			// Create and register state accessors."
		$FileModified += "			services.AddSingleton<" + $inputName + "Accessors>(sp =>"
		$FileModified += "			{"
		$FileModified += "				// We need to grab the conversationState we added on the options in the previous step"
		$FileModified += "				var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;"
		$FileModified += "				if (options == null)"
		$FileModified += "				{"
		$FileModified += "					throw new InvalidOperationException(""BotFrameworkOptions must be configured prior to setting up the State Accessors"");"
		$FileModified += "				}"
		$FileModified += ""
		$FileModified += "				var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();"
		$FileModified += "				if (conversationState == null)"
		$FileModified += "				{"
		$FileModified += "					throw new InvalidOperationException(""ConversationState must be defined and added before adding conversation-scoped state accessors."");"
		$FileModified += "				}"
		$FileModified += ""
		$FileModified += "				var userState = options.State.OfType<UserState>().FirstOrDefault();"
		$FileModified += "				if (userState == null)"
		$FileModified += "				{"
		$FileModified += "					throw new InvalidOperationException(""UserState must be defined and added before adding user-scoped state accessors."");"
		$FileModified += "				}"
		$FileModified += ""
		$FileModified += "				// Create the custom state accessor."
		$FileModified += "				// State accessors enable other components to read and write individual properties of state."
		$FileModified += "				var accessors = new " + $inputName + "Accessors(conversationState, userState)"
		$FileModified += "				{"
		$FileModified += "					ConversationDialogState = conversationState.CreateProperty<DialogState>(""DialogState""),"
		$FileModified += "					UserProfile = userState.CreateProperty<UserProfile>(""UserProfile""),"
		$FileModified += "				};"
		$FileModified += ""
		$FileModified += "				return accessors;"
		$FileModified += "			});"
		$FileModified += ""
	}
}
Set-Content $fileName $FileModified