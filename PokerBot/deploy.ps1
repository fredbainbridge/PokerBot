$deployPath = "D:\apps\PokerBot"
Get-Process -Name "PokerBot" -ErrorAction SilentlyContinue | Stop-Process -ErrorAction SilentlyContinue
Remove-Item -Path $deployPath* -Recurse -Force -Confirm:$false
dotnet publish -o $deployPath -r win-x64
Start-Process -FilePath $deployPath\PokerBot.exe -WorkingDirectory $deployPath
#Copy-Item -Path  .\secrets.json -Destination C:\apps\PokerBot\secrets.json -Force