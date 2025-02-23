$downloadUrl = "https://go.microsoft.com/fwlink/?linkid=866658"
$installerPath = "$env:TEMP\SQL2022-SSEI-Expr.exe"

Write-Host "Downloading SQL Server 2022 Express..."
Invoke-WebRequest -Uri $downloadUrl -OutFile $installerPath

Write-Host "Installing SQL Server 2022 Express..."
Start-Process -FilePath $installerPath -ArgumentList "/IACCEPTSQLSERVERLICENSETERMS", "/Q", "/ACTION=Install", "/FEATURES=SQLEngine", "/INSTANCENAME=SQLEXPRESS", "/SQLSYSADMINACCOUNTS=`"$env:USERDOMAIN\$env:USERNAME`"" -Wait

Write-Host "Installation completed. Starting SQL Server..."
Start-Service -Name "MSSQL`$SQLEXPRESS"

Write-Host "SQL Server Express installation completed successfully!"
