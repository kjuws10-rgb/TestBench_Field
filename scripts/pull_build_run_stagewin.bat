@echo off
setlocal

rem ============================================================
rem TestBench_Field / StageWin
rem Run this file from the repository scripts folder.
rem It pulls latest source, builds StageWin.sln, then runs StageWin.exe.
rem ============================================================

set "SCRIPT_DIR=%~dp0"
set "ROOT=%SCRIPT_DIR%.."
set "REPO_URL=https://github.com/kjuws10-rgb/TestBench_Field.git"
set "SOLUTION=StageWin.sln"
set "CONFIGURATION=Debug"
set "PLATFORM=x64"
set "RUN_EXE=D:\AppEXE\StageWin.exe"
set "FALLBACK_GIT=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\Git\cmd\git.exe"
set "FALLBACK_MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"

pushd "%ROOT%"
if errorlevel 1 (
    echo Failed to enter project folder: %ROOT%
    exit /b 1
)

where git >nul 2>nul
if %errorlevel%==0 (
    set "GIT=git"
) else if exist "%FALLBACK_GIT%" (
    set "GIT=%FALLBACK_GIT%"
) else (
    echo Git was not found. Install Git for Windows or update FALLBACK_GIT in this file.
    popd
    exit /b 1
)

where MSBuild.exe >nul 2>nul
if %errorlevel%==0 (
    set "MSBUILD=MSBuild.exe"
) else if exist "%FALLBACK_MSBUILD%" (
    set "MSBUILD=%FALLBACK_MSBUILD%"
) else (
    echo MSBuild.exe was not found. Install Visual Studio 2022 Build Tools or update FALLBACK_MSBUILD in this file.
    popd
    exit /b 1
)

if not exist ".git" (
    echo This folder is not a Git repository: %CD%
    echo Expected this script to be located under: ^<repo-root^>\scripts
    echo Repository URL: %REPO_URL%
    popd
    exit /b 1
)

if not exist "%SOLUTION%" (
    echo Solution file was not found: %CD%\%SOLUTION%
    popd
    exit /b 1
)

echo [1/3] Pulling latest source...
"%GIT%" pull --ff-only origin main
if errorlevel 1 goto fail

echo [2/3] Building solution...
"%MSBUILD%" "%SOLUTION%" /p:Configuration=%CONFIGURATION% /p:Platform=%PLATFORM% /m /v:minimal
if errorlevel 1 goto fail

echo [3/3] Running application...
if not exist "%RUN_EXE%" (
    echo Run executable was not found: %RUN_EXE%
    echo Check build output path in StageWin\WinForms.csproj.
    goto fail
)

start "" "%RUN_EXE%"
if errorlevel 1 goto fail

popd
exit /b 0

:fail
echo.
echo Failed. Review the error output above.
popd
pause
exit /b 1
