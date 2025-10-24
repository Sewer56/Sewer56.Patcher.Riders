@echo off
REM Check if pwsh is installed
where pwsh >nul 2>nul
if %errorlevel% neq 0 (
    echo PowerShell Core ^(pwsh^) is not installed.
    echo Please download and install it from: https://aka.ms/powershell
    echo.
    pause
    exit /b 1
)

REM Execute the PowerShell script
pwsh -ExecutionPolicy Bypass -File "%~dp0MAKE_PATCH.ps1"
