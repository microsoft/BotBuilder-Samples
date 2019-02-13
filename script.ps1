$FileName = "*Bot.cs"
$Patern = "ActivityTypes.Message" # the 2 lines will be added just after this pattern 
$FileOriginal = Get-Content $FileName

<# create empty Array and use it as a modified file... #>
[String[]] $FileModified = @() 

Foreach ($Line in $FileOriginal)
{    
    $FileModified += $Line

    if ($Line -match $patern) 
    {
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
    } 
}
Set-Content $fileName $FileModified
pause