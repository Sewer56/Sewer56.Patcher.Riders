# Riders Patcher by Sewer56

Extensible and easily modifiable patcher created for the patching of Wii & GameCube games.  

Was originally created for [Riders Regravitified](https://www.youtube.com/watch?v=kWI9TMnisa8); but has since been reused for other games.  I did some git magic to move this source out of SRRG's repository to make it available to the public.

![Preview](./Readme_Preview.png)

## Building
- Download [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) and [Visual Studio 2022+](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false) or [Rider](https://www.jetbrains.com/rider/download/#section=windows).  
- Clone this repository (and all submodules).  
- Open the solution `Sewer56.Patcher.Regravitified.sln`.  
- Build.

## Publishing

In Visual Studio:  
   - Right Click `Sewer56.Patcher.Riders` -> Publish.  

Alternatively in the commandline you can run:  
   - `dotnet publish ./Sewer56.Patcher.Regravitified/Sewer56.Patcher.Riders.csproj -c Release --self-contained false`

## Changing Configuration
The project has two presets, DX Mode for original Riders and Regrav mode for Zero Gravity based ROMs.  
You can change the mode by opening `Directory.Build.props` and modifying the `DefineConstants` line.  

e.g. Build for Regravitified 
```
<DefineConstants>REGRAV</DefineConstants>
```

or 
e.g. Build for OG Riders  
```
<DefineConstants>SRDX</DefineConstants>
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

## Generating Patches

The patcher is actually a command line program that displays a UI only if it receives no parameters.
For reference, try
```
.\Sewer56.Patcher.Riders.Cli.exe --help
```

You will get.
```
Sewer56.Patcher.Riders 1.0.0
Created by Sewer56, licensed under GNU GPL V3

  GenerateHashes    Generates a text file containing hashes of each file.

  VerifyHashes      Verifies hashes within a folder from a hash list.

  GeneratePatch     Generates a patch which converts the contents from one
                    directory into the contents of another directory. Does not
                    (yet) add new files!!

  ConvertNKit       Converts an NKit into an ISO.

  ExtractISO        Converts an ISO into the filesystem files. Extracts DATA
                    partition only.

  ApplyPatch        Applies a patch to the given folder.

  ApplyPatches      Applies a set of patches (each inside a folder) to the given
                    folder.

  BuildISO          Builds a new ISO from a given folder.
```

Please note for generating patches you'll need around 6GB of spare RAM to do this.  
It will also take a while, so go watch an episode of Anime.  

## Creating Patches for Zero Gravity [DefineConstants == 'REGRAV']

1. Copy the `Assets` folder from last released patch.  
   - This includes the existing hashes for vanilla ROMs, patches to NTSC-U from PAL, etc.  

2. Unpack the new ROM using the patcher.  
   - In other words run `Sewer56.Patcher.Riders.Cli.exe ExtractISO`.  

3. Generate new Hashes from extracted ISO.  

   - To do this, run `Sewer56.Patcher.Riders.Cli.exe GenerateHashes`.  
   - Place new `hashes.json` file in `Assets\RG\Hashes\Regrav`.  

Hashes are used for verifying whether the ROM is correct before repacking it during the patching process.

4. Extract and verify your copy of the NTSC-U Version of Sonic Riders ZG.  

   - Must be a good dump, e.g. the one from Vimm's Lair.  
   - Verify your dump using `Sewer56.Patcher.Riders.Cli.exe VerifyHashes`.  
   - Use the hashes in `Assets\RG\Hashes\US 1.0.1`.  

5. Generate new patch from NTSC-U to Regravitified.  

   - Run `Sewer56.Patcher.Riders.Cli.exe GeneratePatch`.  
   - Place patch in `Assets\RG\Patches\To Regrav\US 1.0.1 to Regrav`.  

## Creating Patches for Original Riders [DefineConstants == 'SRDX']

1. Copy the `Assets` folder from last released patch.
   - This includes the existing hashes for vanilla ROMs, etc.  

2. Generate Hashes for new Mod ISO.  

   - To do this, run `Sewer56.Patcher.Riders.Cli.exe GenerateHashes`.  
   - Place new `hashes.json` file in `Assets\DX\Hashes\DX ROM`.  

3. Extract and verify your copy of the NTSC-U Version of Sonic Riders.  

   - Must be a good dump, e.g. the one from Vimm's Lair.  
   - Verify your dump using `Sewer56.Patcher.Riders.Cli.exe VerifyHashes`.  
   - Use the hashes in `Assets\DX\Hashes\Original ROM`.  

4. Generate new patch from NTSC-U to SRDX.  

   - Run `Sewer56.Patcher.Riders.Cli.exe GeneratePatch`.  
   - Place patch in `Assets\DX\Patches\Vanilla to DX\Patch`.  

## Creating Self Contained Riders Patches [DefineConstants == 'SRDXSelfContained']

These patches contain the full game on board and use a simple hash check to verify legal ownership of an existing Riders ROM.
To create these, you will need to build the patcher with `<DefineConstants>DEV</DefineConstants>`.  

1. Create Self Contained Bundle:  

   - Run `Sewer56.Patcher.Riders.Cli.exe CreateSelfContained`.  
   - Place the output file in `Assets/Bundle.patch`.  

2. Update the source code:  

   - In `DxSelfContainedPatch.cs`, update the XOR key to align with your created file.  
   - Update the hashes, to include any more recently released ROMs.  

Build with `<DefineConstants>$(DefineConstants);SRDXSelfContained</DefineConstants>` when publishing to end users.

## Usage on Linux/OSX

```
Prerequisites
- Install the .NET 7 Runtime for your Platform.

Run from Terminal:
- dotnet ./Sewer56.Patcher.Riders.Cli.dll PatchGame --rom "./Sonic Riders - Zero Gravity (USA) (En,Ja,Fr,De,Es,It).wbfs"
- or equivalent
```