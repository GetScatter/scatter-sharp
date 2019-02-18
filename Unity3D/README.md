# Unity3D Easy Setup

### Prerequisite to build

- Unity >= 2018.1
- Visual Studio 2017+

-------------------------------

## Creating a new project

- clone this repository ( `git clone https://github.com/GetScatter/scatter-sharp.git YOUR_PROJECT_NAME` )
- `cd YOUR_PROJECT_NAME/Unity3d/`
- run `.\create_plugins.bat`
- Open the `ScatterSharpUnity3D` project in Unity!

-------------------------------

## Adding to existing project.

### Build package windows
- run `.\build_package_win.bat`

You will need to modify a few settings to allow for .NET 2.0.

Inside your project:
- **Go to Edit -> Project Settings -> Player -> Other Settings -> Configuration -> Scripting Runtime Version -> .NET 4.6 Equivalent**
- This will ask you to restart Unity3d, when it opens back up:
  **Go to Edit -> Project Settings -> Player -> Other Settings -> Configuration -> Api Compatibility Level -> .NET 4.6**
- Get the [`scatter-sharp.unitypackage` from this repository](https://raw.githubusercontent.com/GetScatter/scatter-sharp/master/Unity3D/scatter-sharp.unitypackage)
- Then back inside Unity:
  **Assets -> Import Package** and select the location you saved the above package to, or just open the unitypackage file with your project open.
