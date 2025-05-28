@echo off
echo Starting Cookie Authentication Demo Applications...

echo Starting API on https://localhost:5001
start cmd /k "cd c:\Learning_Project_Works\AuthTest\CookieAuth\CookieAuth.API && dotnet run --launch-profile https"

echo Starting Web UI on https://localhost:5002
start cmd /k "cd c:\Learning_Project_Works\AuthTest\CookieAuth\CookieAuth.Web && dotnet run --launch-profile https"

echo Both applications are starting. Please wait...
echo Web UI will be available at: https://localhost:5002
echo API will be available at: https://localhost:5001

start https://localhost:5002

echo Press any key to close all applications...
pause
taskkill /f /fi "WINDOWTITLE eq *CookieAuth*"
