@echo off
setlocal

set CONFIGURATION=%1
if "%CONFIGURATION%" == "" (
	echo Using default build configuration Release
    set CONFIGURATION=Release
)

for /f "usebackq tokens=1* delims=: " %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

set ScatterSharpTestDir=ScatterSharpTest\Assets\Plugins\

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (

  echo Building ScatterSharp project
  cd ..\ScatterSharp\
  dotnet restore
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" ScatterSharp.sln /p:Configuration=%CONFIGURATION%
  
  echo Copying ScatterSharp project to Assets Plugins
  cd ..\Unity3D
  
  if not exist "%ScatterSharpTestDir%" ( 
	mkdir %ScatterSharpTestDir% 
  )
  
  copy %userprofile%\.nuget\packages\cryptography.ecdsa.secp256k1\1.1.2\lib\netstandard2.0\Cryptography.ECDSA.dll %ScatterSharpTestDir%
  copy %userprofile%\.nuget\packages\json.net.aot\9.0.1\lib\netstandard2.0\Newtonsoft.Json.dll %ScatterSharpTestDir%
  copy ..\eos-sharp\EosSharp\EosSharp.Core\bin\%CONFIGURATION%\netstandard2.0\EosSharp.Core.dll %ScatterSharpTestDir%
  copy ..\eos-sharp\EosSharp\EosSharp.Unity3D\bin\%CONFIGURATION%\netstandard2.0\EosSharp.Unity3D.dll %ScatterSharpTestDir%
  copy ..\..\socketio-sharp\SocketIOSharp\WebSocketSharp\websocket-sharp\bin\Debug_Ubuntu\websocket-sharp.dll %ScatterSharpTestDir%
  copy ..\..\socketio-sharp\SocketIOSharp\SocketIOSharp.Core\bin\%CONFIGURATION%\netstandard2.0\SocketIOSharp.Core.dll %ScatterSharpTestDir%
  copy ..\..\socketio-sharp\SocketIOSharp\SocketIOSharp.Unity3D\bin\%CONFIGURATION%\netstandard2.0\SocketIOSharp.Core.dll %ScatterSharpTestDir%
  copy ..\..\socketio-sharp\SocketIOSharp\SocketIOSharp.Unity3D\WebSocket.jslib %ScatterSharpTestDir%
  copy ..\ScatterSharp\ScatterSharp.Core\bin\%CONFIGURATION%\netstandard2.0\ScatterSharp.Core.dll %ScatterSharpTestDir%
  copy ..\ScatterSharp\ScatterSharp.Unity3D\bin\%CONFIGURATION%\netstandard2.0\ScatterSharp.Unity3D.dll %ScatterSharpTestDir%
  copy ..\ScatterSharp\ScatterSharp.EosProvider\bin\%CONFIGURATION%\netstandard2.0\ScatterSharp.EosProvider.dll %ScatterSharpTestDir%
  
  echo Project Assets Plugins build successfully
)
