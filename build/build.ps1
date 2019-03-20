Function WriteAndExecute([string]$command) {
	Write-Output $command
	Invoke-Expression $command
}

Push-Location
Set-Location .\web
try {
    WriteAndExecute 'npm install'
    WriteAndExecute 'npm run prod'
} finally { Pop-Location }