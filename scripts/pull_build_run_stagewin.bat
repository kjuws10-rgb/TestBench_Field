@echo off
setlocal EnableExtensions EnableDelayedExpansion

rem ============================================================
rem StageWin_v2.32 pull -> build -> run
rem Target repository:
rem   https://github.com/kjuws10-rgb/TestBench_Field.git
rem Target local path:
rem   C:\Users\jwkang01\Downloads\StageWin_v2.32
rem ============================================================

set "REPO_URL=https://github.com/kjuws10-rgb/TestBench_Field.git"
set "TARGET_DIR=C:\Users\jwkang01\Downloads\StageWin_v2.32"
set "SOLUTION_FILE=StageWin.sln"
set "CONFIGURATION=Debug"
set "PLATFORM=x64"
set "DEFAULT_EXE=D:\AppEXE\StageWin.exe"

echo.
echo [StageWin] GitHub 최신 코드 적용, 빌드, 실행을 시작합니다.
echo [StageWin] Repository : %REPO_URL%
echo [StageWin] Target     : %TARGET_DIR%
echo.

call :FindGit
if errorlevel 1 goto :Fail

call :FindMSBuild
if errorlevel 1 goto :Fail

if not exist "%TARGET_DIR%\.git" (
    echo [StageWin] 대상 Git 저장소가 없습니다. clone을 진행합니다.
    for %%I in ("%TARGET_DIR%") do set "TARGET_PARENT=%%~dpI"
    if not exist "!TARGET_PARENT!" mkdir "!TARGET_PARENT!"
    "%GIT_EXE%" clone "%REPO_URL%" "%TARGET_DIR%"
    if errorlevel 1 (
        echo [ERROR] git clone 실패
        goto :Fail
    )
) else (
    echo [StageWin] 기존 저장소에서 최신 코드를 가져옵니다.
    "%GIT_EXE%" -C "%TARGET_DIR%" fetch origin
    if errorlevel 1 (
        echo [ERROR] git fetch 실패
        goto :Fail
    )
    "%GIT_EXE%" -C "%TARGET_DIR%" pull --ff-only origin main
    if errorlevel 1 (
        echo [ERROR] git pull 실패. 로컬 변경사항 또는 브랜치 상태를 확인하세요.
        goto :Fail
    )
)

if not exist "%TARGET_DIR%\%SOLUTION_FILE%" (
    echo [ERROR] 솔루션 파일을 찾을 수 없습니다: %TARGET_DIR%\%SOLUTION_FILE%
    goto :Fail
)

echo.
echo [StageWin] 프로젝트 빌드 시작: %CONFIGURATION%^|%PLATFORM%
"%MSBUILD_EXE%" "%TARGET_DIR%\%SOLUTION_FILE%" /p:Configuration=%CONFIGURATION% /p:Platform=%PLATFORM% /m /v:minimal
if errorlevel 1 (
    echo.
    echo [ERROR] 빌드 실패
    echo [HINT] ACS.SPiiPlusNET.dll 참조가 설치되어 있는지 확인하세요.
    echo [HINT] 현재 프로젝트는 x64 빌드 출력 경로로 D:\AppEXE 를 사용합니다.
    goto :Fail
)

call :ResolveExe
if errorlevel 1 goto :Fail

echo.
echo [StageWin] 실행 파일: %RUN_EXE%
start "" "%RUN_EXE%"
if errorlevel 1 (
    echo [ERROR] 실행 실패
    goto :Fail
)

echo.
echo [StageWin] 완료되었습니다.
goto :End

:FindGit
set "GIT_EXE="
if exist "C:\Program Files\Git\cmd\git.exe" set "GIT_EXE=C:\Program Files\Git\cmd\git.exe"
if not defined GIT_EXE if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\Git\cmd\git.exe" set "GIT_EXE=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\Git\cmd\git.exe"
if not defined GIT_EXE (
    for /f "delims=" %%G in ('where git.exe 2^>nul') do (
        if not defined GIT_EXE set "GIT_EXE=%%G"
    )
)
if not defined GIT_EXE (
    echo [ERROR] git.exe를 찾을 수 없습니다. Git for Windows 또는 Visual Studio Git 도구를 설치하세요.
    exit /b 1
)
echo [StageWin] Git: %GIT_EXE%
exit /b 0

:FindMSBuild
set "MSBUILD_EXE="
if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
if not defined MSBUILD_EXE if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
if not defined MSBUILD_EXE if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not defined MSBUILD_EXE (
    for /f "delims=" %%M in ('where MSBuild.exe 2^>nul') do (
        if not defined MSBUILD_EXE set "MSBUILD_EXE=%%M"
    )
)
if not defined MSBUILD_EXE (
    echo [ERROR] MSBuild.exe를 찾을 수 없습니다. Visual Studio 2022 Build Tools를 설치하세요.
    exit /b 1
)
echo [StageWin] MSBuild: %MSBUILD_EXE%
exit /b 0

:ResolveExe
set "RUN_EXE="
if exist "%DEFAULT_EXE%" set "RUN_EXE=%DEFAULT_EXE%"
if not defined RUN_EXE if exist "%TARGET_DIR%\StageWin\bin\x64\%CONFIGURATION%\StageWin.exe" set "RUN_EXE=%TARGET_DIR%\StageWin\bin\x64\%CONFIGURATION%\StageWin.exe"
if not defined RUN_EXE if exist "%TARGET_DIR%\StageWin\bin\%CONFIGURATION%\StageWin.exe" set "RUN_EXE=%TARGET_DIR%\StageWin\bin\%CONFIGURATION%\StageWin.exe"
if not defined RUN_EXE (
    echo [ERROR] 빌드 후 실행 파일을 찾을 수 없습니다.
    echo [CHECK] %DEFAULT_EXE%
    echo [CHECK] %TARGET_DIR%\StageWin\bin\x64\%CONFIGURATION%\StageWin.exe
    echo [CHECK] %TARGET_DIR%\StageWin\bin\%CONFIGURATION%\StageWin.exe
    exit /b 1
)
exit /b 0

:Fail
echo.
echo [StageWin] 실패했습니다. 위 오류 메시지를 확인하세요.
pause
exit /b 1

:End
pause
exit /b 0
