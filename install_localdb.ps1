$downloadUrl = "https://go.microsoft.com/fwlink/?LinkID=866658"
$installerPath = "$env:TEMP\SQL2022-SSEI-Expr.exe"

Write-Host "Downloading SQL Server LocalDB..."
Invoke-WebRequest -Uri $downloadUrl -OutFile $installerPath

Write-Host "Installing SQL Server LocalDB..."
Start-Process -FilePath $installerPath -ArgumentList "/IACCEPTSQLSERVERLICENSETERMS", "/Q", "/ACTION=Install", "/FEATURES=LocalDB" -Wait

Write-Host "Installation completed!"
