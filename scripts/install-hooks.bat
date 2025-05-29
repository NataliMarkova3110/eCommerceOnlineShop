@echo off

REM Copy the pre-push hook
copy /Y scripts\pre-push.bat .git\hooks\pre-push

echo Git hooks installed successfully. 