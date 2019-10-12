Remove-Item -Path c:\app\PokerBot\* -Recurse -Force -Confirm:$false
dotnet publish -o c:\apps\PokerBot -r win-x64
#Copy-Item -Path  .\secrets.json -Destination C:\apps\PokerBot\secrets.json -Force