param([string]$inputName="botClass")

$FileName = "*Startup.cs"
$FileOriginal = Get-Content $FileName
$PaternUsing = "using System;"
$PaternClassConfigServ = "public void ConfigureServices"

<# create empty Array and use it as a modified file... #>
[String[]] $FileModified = @() 

Foreach ($Line in $FileOriginal)
{    
    $FileModified += $Line
	
	if ($Line -match $paternUsing) 
    {
		<# add using statement #>
		$FileModified += "using Microsoft.Bot.Builder;"
	}
	
	if ($Line -match $PaternClassConfigServ) 
    {
		$foreach.movenext()
		$FileModified += "		{"
		$FileModified += "			// The Memory Storage used here is for local bot debugging only. When the bot"
        $FileModified += "	        // is restarted, everything stored in memory will be gone."
        $FileModified += "	        IStorage dataStore = new MemoryStorage();"
		$FileModified += ""	
		$FileModified += "			// Create and add conversation state (for empty bot template only)."
		$FileModified += "			var conversationState = new ConversationState(dataStore);"
		$FileModified += "			services.AddSingleton(conversationState);"
		$FileModified += ""
		$FileModified += "			// Create and add user state."
		$FileModified += "			var userState = new UserState(dataStore);"
		$FileModified += "			services.AddSingleton(userState);"		
		$FileModified += ""
	}
}
Set-Content $fileName $FileModified