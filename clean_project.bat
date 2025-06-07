@echo off
echo Cleaning Animal Revolt Project...

REM Delete all documentation files
del ANIMAL_REVOLT_GAME_DESIGN.md 2>nul
del ANIMAL_REVOLT_TODO_LIST.md 2>nul
del SETUP_PROGRESS_REPORT.md 2>nul
del ERROR_FIXES_REPORT.md 2>nul
del FINAL_ERROR_RESOLUTION_GUIDE.md 2>nul

REM Delete old scripts directory
rmdir /s /q Assets\Scripts 2>nul

echo Project cleaned successfully!
echo Only Animal Revolt scripts remain.
pause