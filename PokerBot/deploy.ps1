$deployPath = "D:\apps\PokerBot"
Remove-Item -Path $deployPath* -Recurse -Force -Confirm:$false
dotnet publish -o $deployPath -r win-x64
#Copy-Item -Path  .\secrets.json -Destination C:\apps\PokerBot\secrets.json -Force