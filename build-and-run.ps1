# Build and Run LaserGRBL Storer Edition
& dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Starting application..." -ForegroundColor Green
    Start-Process .\LaserGRBL\bin\Debug\LaserGRBL.exe
} else {
    Write-Host "Build failed!" -ForegroundColor Red
}
