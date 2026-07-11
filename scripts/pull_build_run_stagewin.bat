@echo off
setlocal

rem ============================================================
rem TestBench_Field / StageWin
rem Expected script folder:
rem   C:\Users\jwkang01\Downloads\TestBench_Field\scripts
rem Project root is resolved as:
rem   C:\Users\jwkang01\Downloads\TestBench_Field
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
set "VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"

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

call :FindMSBuild
if errorlevel 1 (
    popd
    exit /b 1
)

if not exist ".git" (
    echo This folder is not a Git repository: %CD%
    echo Expected this script path:
    echo C:\Users\jwkang01\Downloads\TestBench_Field\scripts
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

:FindMSBuild
set "MSBUILD="

where MSBuild.exe >nul 2>nul
if %errorlevel%==0 (
    for /f "delims=" %%M in ('where MSBuild.exe 2^>nul') do (
        if not defined MSBUILD set "MSBUILD=%%M"
    )
)

if not defined MSBUILD if exist "%FALLBACK_MSBUILD%" set "MSBUILD=%FALLBACK_MSBUILD%"

if not defined MSBUILD if exist "%VSWHERE%" (
    for /f "usebackq delims=" %%I in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
        if not defined MSBUILD set "MSBUILD=%%I"
    )
)

if not defined MSBUILD (
    for %%M in (
        "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    ) do (
        if not defined MSBUILD if exist %%~M set "MSBUILD=%%~M"
    )
)

if not defined MSBUILD (
    echo MSBuild.exe was not found.
    echo Install "Visual Studio 2022 Build Tools" with ".NET desktop build tools".
    echo Or update FALLBACK_MSBUILD in this file.
    exit /b 1
)

echo MSBuild: %MSBUILD%
exit /b 0
