# Create directory for certificates if it doesn't exist
if (-not (Test-Path -Path "https")) {
    New-Item -ItemType Directory -Path "https" | Out-Null
    Write-Host "Directory 'https' created"
}

# Generate certificate
dotnet dev-certs https -ep https\aspnetapp.pfx -p "yourpassword"

Write-Host "SSL certificate successfully generated in 'https/' folder"
Write-Host "Use password 'yourpassword' for this certificate"

# Trust certificate (optional)
Write-Host "Trust this certificate? (Y/N)"
$answer = Read-Host
if ($answer -eq "Y" -or $answer -eq "y") {
    dotnet dev-certs https --trust
    Write-Host "Certificate added to trusted certificates"
} 