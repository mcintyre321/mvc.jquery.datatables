#This file is based on file from Glimpse project, under Apache licence (https://github.com/Glimpse/Glimpse/blob/efa39506576048084250622a2fc2150f8880cbe2/license.txt)
param($installPath, $toolsPath, $package, $project)

$from = $null

$tempDir = $env:TEMP
$log = [System.IO.Path]::Combine($tempDir,"Mvc.JQuery.DataTables\install.log")

if ([System.IO.File]::Exists($log))
{
    $lines = Get-Content $log | where {$_.Contains($project.FileName)} | where {$_.Contains($package.Id)}

    if ($lines -ne $null){
		    
		if ($lines.GetType().Name -eq 'String'){
			$lastline = $lines
		} else {
			$lastLine = $lines[$lines.Length-1]
		}
		       
        $props = $lastLine.Split(',')
        
        $versionRemoved = $props[1]
        $timeRemoved = $props[2]
        $timeRemoved = Get-Date -Date $timeRemoved
        $now = Get-Date
        
        $delta = New-TimeSpan $timeRemoved $now
        
        if ($delta.Minutes -lt 5){
            $from = $versionRemoved
        }
    }
}

if ($from -ne $null -and $from -ne $package.Version){
    $DTE.ItemOperations.Navigate("http://aspdatatables.azurewebsites.net/?" + $package.Id + "=" + $from + ".." + $package.Version)
}
else{
    $DTE.ItemOperations.Navigate("http://aspdatatables.azurewebsites.net/?" + $package.Id + "=" + $package.Version)
}