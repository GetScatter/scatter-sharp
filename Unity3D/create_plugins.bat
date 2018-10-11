for /f "usebackq tokens=1* delims=: " %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  cd ..\ScatterSharp\
  dotnet restore
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" ScatterSharp.sln /t:ScatterSharp /p:Configuration=Release
  cd ..\Unity3D
  mkdir Plugins
  copy %userprofile%\.nuget\packages\cryptography.ecdsa.secp256k1\1.1.2\lib\netstandard2.0\Cryptography.ECDSA.dll ScatterSharpTest\Assets\Plugins\
  copy %userprofile%\.nuget\packages\eos-sharp\1.0.2\lib\netstandard2.0\EosSharp.dll ScatterSharpTest\Assets\Plugins\
  copy %userprofile%\.nuget\packages\fastmember\1.4.1\lib\netstandard2.0\FastMember.dll ScatterSharpTest\Assets\Plugins\
  copy %userprofile%\.nuget\packages\newtonsoft.json\11.0.2\lib\netstandard2.0\Newtonsoft.Json.dll ScatterSharpTest\Assets\Plugins\
  copy ..\ScatterSharp\ScatterSharp\bin\Release\netstandard2.0\ScatterSharp.dll ScatterSharpTest\Assets\Plugins\
)
