# Build and Run LaserGRBL Storer Edition
& dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Starting application..." -ForegroundColor Green
    Start-Process dotnet -ArgumentList "run", "--project", "LaserGRBL\LaserGRBL.csproj", "--no-build"
} else {
    Write-Host "Build failed!" -ForegroundColor Red
}
