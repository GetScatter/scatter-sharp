echo off
setlocal

set ScatterSharpUnity3DDir=ScatterSharpUnity3D\Assets\Plugins\

if not exist "ScatterSharpUnity3D\Assets\Plugins\" ( 
	call create_plugins.bat
)

if "%UNITY_PATH%" == "" (
    set UNITY_PATH=%1
)

set SEARCH_UNITY_PATH=%ProgramFiles%\Unity\Hub\Editor\Unity.exe
if "%UNITY_PATH%" == "" (
	FOR /F "delims=" %%i IN ('dir /b /s "%SEARCH_UNITY_PATH%"') DO set UNITY_PATH=%%i
)

if "%UNITY_PATH%" == "" (
    echo Error: Unity path not defined. Please set the UNITY_PATH environment variable to the Unity.exe file, or pass the path as argument
    exit /B 1
)

if not exist "%UNITY_PATH%" (
    echo Error: File %UNITY_PATH% doesn't exist
    exit /B 1
)

echo Using UNITY_PATH = %UNITY_PATH%
echo Launching Unity to build scatter-sharp.unitypackage

"%UNITY_PATH%" -batchmode -nographics -logFile out.txt -projectPath "%cd%/ScatterSharpUnity3D/" -exportPackage "Assets/Plugins" "..\scatter-sharp.unitypackage" -quit

echo scatter-sharp.unitypackage builded successfully