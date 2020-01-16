$url = 'http://www.pokerhand.org/_img/standard/'
$suits = "s", "d", "h", "c"
foreacH($n in 2..10) {
    foreach($s in $suits) {
        $cardUrl = "$url$n$s.png"
    
        $file = Invoke-WebRequest -Uri $cardUrl
        if(-not (Test-Path $localPath)) {
            New-Item -Path ".\wwwroot\media" -Name "$n$s.png" -ItemType File -Force
        }
        $localPath = ".\wwwroot\media\$n$s.png"
        $file.Content | Set-Content -Path $localPath -Encoding Byte
    }
}
foreach($n in "T","J","Q","K","A") {
    foreach($s in $suits) {
        $cardUrl = "$url$n$s.png"
    
        $file = Invoke-WebRequest -Uri $cardUrl
        if(-not (Test-Path $localPath)) {
            New-Item -Path ".\wwwroot\media" -Name "$n$s.png" -ItemType File -Force
        }
        $localPath = ".\wwwroot\media\$n$s.png"
        $file.Content | Set-Content -Path $localPath -Encoding Byte
    }
}

