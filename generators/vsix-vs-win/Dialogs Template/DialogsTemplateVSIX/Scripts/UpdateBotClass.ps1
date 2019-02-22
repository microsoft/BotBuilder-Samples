param(
	[Parameter(Position=0)][string]$inputName="botClass",
	[Parameter(Position=1)][string]$dialogsName="dialogsName"
	)

$FileName = "*Bot.cs"
$FileOriginal = Get-Content $FileName
$PaternUsing = "using Microsoft.Bot.Builder;"
$PaternBotClass = "public class"
$PaternConstructor = "public " + $inputName
$PaternOnTurnAsync = "public async Task OnTurnAsync"
$PaternActivityType = "turnContext.Activity.Type == ActivityTypes.Message"
$PaternSendActivity = "await turnContext.SendActivityAsync"

[String[]] $FileModified = @() 

Foreach ($Line in $FileOriginal)
{    
	if ($Line -match $PaternConstructor) 
    {
		$foreach.movenext()
		$foreach.movenext()
		$FileModified += "		public " + $inputName + "(UserState userState, ConversationState conversationState, ILoggerFactory loggerFactory)"
		$FileModified += "		{"
		$FileModified += "			_userState = userState ?? throw new ArgumentNullException(nameof(userState));"
		$FileModified += "			_conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));"
		$FileModified += ""
		$FileModified += "			_greetingStateAccessor = _userState.CreateProperty<GreetingState>(nameof(GreetingState));"
		$FileModified += "			_dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));"
		$FileModified += ""
		$FileModified += "			Dialogs = new DialogSet(_dialogStateAccessor);"
		$FileModified += "			Dialogs.Add(new " + $dialogsName + "(_greetingStateAccessor, loggerFactory));"
		$FileModified += "		}"
		$FileModified += ""
		$FileModified += "		private DialogSet Dialogs { get; set; }"
		$FileModified += ""
	}
	else
	{
		if ($Line -match $PaternSendActivity) 
		{
			$FileModified += "				// " + $Line.Trim()
		}
		else
		{
			$FileModified += $Line
		}
	}
	
	if ($Line -match $paternUsing) 
    {
		$FileModified += "using System;"
		$FileModified += "using Microsoft.Bot.Builder.Dialogs;"
		$FileModified += "using Microsoft.Extensions.Logging;"
	}
	
	if ($Line -match $paternBotClass) 
    {
		$foreach.movenext()
		$FileModified += "	{"
		$FileModified += "		private readonly IStatePropertyAccessor<GreetingState> _greetingStateAccessor;"
		$FileModified += "		private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;"
		$FileModified += "		private readonly UserState _userState;"
		$FileModified += "		private readonly ConversationState _conversationState;"
		$FileModified += ""
	}
	
	if ($Line -match $PaternOnTurnAsync) 
    {
		$foreach.movenext()
		$FileModified += "		{"
		$FileModified += "			var activity = turnContext.Activity;"
		$FileModified += ""
		$FileModified += "			var dialogContext = await Dialogs.CreateContextAsync(turnContext);"
		$FileModified += ""
	}

    if ($Line -match $PaternActivityType) 
    {
		$foreach.movenext()
		$FileModified += "			{"
        $FileModified += "				var results = await dialogContext.ContinueDialogAsync();"
        $FileModified += ""
        $FileModified += "				if (results.Status == DialogTurnStatus.Empty)"
        $FileModified += "				{"
        $FileModified += "					await dialogContext.BeginDialogAsync(nameof(" + $dialogsName + "));"
        $FileModified += "				}"
		$FileModified += ""
		$FileModified += "				// Save the dialog state into the conversation state."
		$FileModified += "				await _conversationState.SaveChangesAsync(turnContext);"
		$FileModified += ""
		$FileModified += "				// Save the user profile updates into the user state."
		$FileModified += "				await _userState.SaveChangesAsync(turnContext);"
		$FileModified += ""
    } 
}
Set-Content $fileName $FileModified