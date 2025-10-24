# Riders Patcher by Sewer56

Extensible and easily modifiable patcher created for the patching of Wii & GameCube games.  

Was originally created for [Riders Regravitified](https://www.youtube.com/watch?v=kWI9TMnisa8); but has since been reused for other games.  I did some git magic to move this source out of SRRG's repository to make it available to the public.

![Preview](./Readme_Preview.png)

## Building
- Download [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) and [Visual Studio 2022+](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false) or [Rider](https://www.jetbrains.com/rider/download/#section=windows).  
- Clone this repository (and all submodules).  
- Open the solution `Sewer56.Patcher.Regravitified.sln`.  
- Build.

## Publishing

In Visual Studio:  
   - Right Click `Sewer56.Patcher.Riders` -> Publish.  

Alternatively in the commandline you can run:  
   - `dotnet publish ./Sewer56.Patcher.Regravitified/Sewer56.Patcher.Riders.csproj -c Release --self-contained false`

## Changing Configuration
The project has three presets.
>**SRDX Mode**: for original Riders ROMs to SRDX.\
>**SRTE Mode**: for original Riders ROMs to SRTE.\
>**Regrav Mode**: for Zero Gravity based ROMs.

You can change the mode by opening `Directory.Build.props` and modifying the `DefineConstants` line.  

e.g. Build for Regravitified 
```
<DefineConstants>REGRAV</DefineConstants>
```

or 
e.g. Build for OG Riders to SRDX
```
<DefineConstants>SRDX</DefineConstants>
```

or
e.g. Build for OG Riders to SRTE
```
<DefineConstants>SRTE</DefineConstants>
```
## Project Structure

- **Sewer56.Patcher.Riders:** The UI Components of the patcher.  
- **Sewer56.Patcher.Riders.Common:** Generic code shared between each component.  
- **Sewer56.Patcher.Riders.Regrav:** Code specific to Patching Regravitified.  
- **Sewer56.Patcher.Riders.Dx:** Code specific to Patching SRDX.  
- **Sewer56.Patcher.Riders.Tests:** Automated tests.  

### Adding Alternative Game/Branding Support

- Clone one of the Projects, e.g. `Sewer56.Patcher.Riders.Dx` to another folder.  
  - Rename `.csproj` file.  

- Add Existing Project to Solution.  
    - Fix Namespaces in your new project (i.e. `Alt+Enter` or `Alt+.` on a namespace, and fix for project).  
  
- Open `Sewer56.Patcher.Riders.Cli.csproj`, add a conditional project reference:  
  - e.g. for `REGRAV` it is defined as `<ProjectReference Include="..\Sewer56.Patcher.Regravitified.Regrav\Sewer56.Patcher.Riders.Regrav.csproj" Condition="$(DefineConstants.Contains(REGRAV))" />`.  
  - Add alternative icon in `Sewer56.Patcher.Riders.csproj`.  

- Add your new define tag to `Directory.Build.props`.  

Make a PR to this repo 😇.

## Usage on Linux/OSX

```
Prerequisites
- Install the .NET 9 Runtime for your Platform.

Run from Terminal:
- dotnet ./Sewer56.Patcher.Riders.Cli.dll PatchGame --rom "./Sonic Riders - Zero Gravity (USA) (En,Ja,Fr,De,Es,It).wbfs"
- or equivalent
```

## How to Generate a New Patch

See [GENERATING_PATCHES.md](GENERATING_PATCHES.md) for detailed instructions on generating new patches.