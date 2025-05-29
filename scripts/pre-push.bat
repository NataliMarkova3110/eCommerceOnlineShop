@echo off

echo Running dotnet-format validation...

REM Run dotnet-format check
dotnet format --verify-no-changes

REM Store the exit code
set EXIT_CODE=%ERRORLEVEL%

if %EXIT_CODE% NEQ 0 (
    echo ❌ dotnet-format validation failed. Please run 'dotnet format' to fix the issues.
    exit /b 1
)

echo ✅ dotnet-format validation passed.
exit /b 0 