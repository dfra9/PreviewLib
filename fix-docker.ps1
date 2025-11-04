# Script untuk fix Docker container LibreOffice di Windows

Write-Host "Menghapus container lama..." -ForegroundColor Yellow
docker rm -f libre-preview 2>$null

Write-Host "`nMembuat container baru dengan LibreOffice (pre-installed image)..." -ForegroundColor Green
docker run -d `
  --name libre-preview `
  -v E:\PreviewLib\APIWebPreview\APIWebPreview\storage:/data `
  linuxserver/libreoffice:latest `
  tail -f /dev/null

Write-Host "`nMenunggu container siap (10 detik)..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

Write-Host "`nCek status container:" -ForegroundColor Green
docker ps --filter "name=libre-preview"

Write-Host "`nTest konversi:" -ForegroundColor Green
docker exec libre-preview libreoffice --version

Write-Host "`nSelesai! Container siap digunakan." -ForegroundColor Green
Write-Host "Sekarang restart aplikasi C# Anda." -ForegroundColor Yellow
