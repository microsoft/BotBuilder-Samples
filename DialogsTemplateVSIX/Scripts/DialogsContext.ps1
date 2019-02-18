param([string]$inputName="fileinputname")

$FileName = "*Bot.cs"
$FileOriginal = Get-Content $FileName
$PaternUsing = "using Microsoft.Bot.Builder;"
$PaternBotClass = "public class"
$Patern = "turnContext.Activity.Type == ActivityTypes.Message"

<# create empty Array and use it as a modified file... #>
[String[]] $FileModified = @() 

Foreach ($Line in $FileOriginal)
{    
    $FileModified += $Line
	
	if ($Line -match $paternUsing) 
    {
		<# add using statement #>
		$FileModified += "using Microsoft.Bot.Builder.Dialogs;"
	}
	
	if ($Line -match $paternBotClass) 
    {
		<# add Dialog set and prompts #>
		$foreach.movenext()
		$FileModified += "	{"
		$FileModified += "		private readonly " + $inputName + "Accessors _accessors;"
		$FileModified += ""
		$FileModified += "		private DialogSet _dialogs;"
		$FileModified += ""
		$FileModified += "		public " + $inputName + "(" + $inputName + "Accessors accessors)"
		$FileModified += "		{"
		$FileModified += "			_accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));"
		$FileModified += ""
		$FileModified += "			// The DialogSet needs a DialogState accessor, it will call it when it has a turn context."
		$FileModified += "			_dialogs = new DialogSet(accessors.ConversationDialogState);"
		$FileModified += ""
		$FileModified += "			// Define the steps of the waterfall dialog and add it to the set."
		$FileModified += "			var waterfallSteps = new WaterfallStep[]"
		$FileModified += "			{"
		$FileModified += "				NameStepAsync,"
		$FileModified += "				NameConfirmStepAsync,"
		$FileModified += "				AgeStepAsync,"
		$FileModified += "				AgeConfirmStepAsync,"
		$FileModified += "				SummaryAsync,"
		$FileModified += "			};"
		$FileModified += ""
		$FileModified += "			// Add named dialogs to the DialogSet. These names are saved in the dialog state."
		$FileModified += "			_dialogs.Add(new WaterfallDialog(""details"", waterfallSteps));"
		$FileModified += "			_dialogs.Add(new TextPrompt(""name""));"
		$FileModified += "			_dialogs.Add(new NumberPrompt<int>(""age""));"
		$FileModified += "			_dialogs.Add(new ConfirmPrompt(""confirm""));"
		$FileModified += "		}"
		$FileModified += ""
	}

    if ($Line -match $patern) 
    {
		<# add Dialogs Context #>
		$foreach.movenext()
		$FileModified += "			{"
        $FileModified += "				var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);"
		$FileModified += ""
        $FileModified += "				var results = await dialogContext.ContinueDialogAsync(cancellationToken);"
        $FileModified += ""
        $FileModified += "				if (results.Status == DialogTurnStatus.Empty)"
        $FileModified += "				{"
        $FileModified += "					await dialogContext.BeginDialogAsync(""details"", null, cancellationToken);"
        $FileModified += "				}"
		$FileModified += ""
		$FileModified += "				// Save the dialog state into the conversation state."
		$FileModified += "				await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);"
		$FileModified += ""
		$FileModified += "				// Save the user profile updates into the user state."
		$FileModified += "				await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);"
    } 
}
Set-Content $fileName $FileModified