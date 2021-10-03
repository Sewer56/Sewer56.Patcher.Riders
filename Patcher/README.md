# Regravitified Patcher by Sewer56

Extensible and easily modifiable patcher created for the patching of games.
This one is specific to Regrav.

## Building
- Download Visual Studio 2019+
- Clone this repository (and all submodules).
- Open the solution `Sewer56.Patcher.Regravitified.sln`.
- Build.

After building, you will need to copy the Assets folder from the last patcher release.

## Changing Configuration
The project can either be built in DX mode (patch vanilla -> DX) or for Regravitified.
You can change the mode by opening `Sewer56.Patcher.Riders.csproj` and modifying the `DefineConstants` line.

e.g. Build for Regravitified
```
<DefineConstants>REGRAV</DefineConstants>
```

or 
e.g. Build for DX.
```
<DefineConstants>SRDX</DefineConstants>
```

## Project Structure

- **Sewer56.Patcher.Riders:** The UI Components of the patcher.
- **Sewer56.Patcher.Riders.Common:** Generic code shared between each component.
- **Sewer56.Patcher.Riders.Regrav:** Code specific to Patching Regravitified.
- **Sewer56.Patcher.Riders.Dx:** Code specific to Patching DX.
- **Sewer56.Patcher.Riders.Tests:** Automated tests.

## Generating Patches

The patcher is actually a command line program that displays a UI only if it receives no parameters.
For reference, try
```
.\Sewer56.Patcher.Riders.exe --help
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

## Updating Patches for Regravitified

If you plan to make a new patch, you have to do 2 things:

1. Copy the `Assets` folder from last released patch. Important.
2. Unpack the new ROM using the patcher.
- In other words run `Sewer56.Patcher.Regravitified.exe ExtractISO`

3. Generate new Hashes from extracted ISO.

- To do this, run `Sewer56.Patcher.Regravitified.exe GenerateHashes`. 
- Place new `hashes.json` file in `Assets\RG\Hashes\Regrav`

Hashes are used for verifying whether the ROM is correct before repacking it during the patching process.

4. Extract and verify your copy of the NTSC-U Version of Sonic Riders ZG.

- Must be a good dump, e.g. the one from Vimm's Lair.
- Verify your dump using `Sewer56.Patcher.Regravitified.exe VerifyHashes`.
- Use the hashes in `Assets\RG\Hashes\US 1.0.1`

5. Generate new patch from NTSC-U to Regravitified.

- Run `Sewer56.Patcher.Regravitified.exe GeneratePatch`. 
- Place patch in `Assets\RG\Patches\To Regrav\US 1.0.1 to Regrav`

Please note you'll need around 6GB of spare RAM to do this.
It will also take a while, so go watch an episode of Anime.

## Creating Patches for SRDX

1. Copy the `Assets` folder from last released patch. Important.
2. Unpack the new ROM using the patcher.
- In other words run `Sewer56.Patcher.Regravitified.exe ExtractISO`

3. Generate new Hashes from extracted ISO.

- To do this, run `Sewer56.Patcher.Regravitified.exe GenerateHashes`. 
- Place new `hashes.json` file in `Assets\DX\Hashes\DX ROM`

4. Extract and verify your copy of the NTSC-U Version of Sonic Riders.

- Must be a good dump, e.g. the one from Vimm's Lair.
- Verify your dump using `Sewer56.Patcher.Regravitified.exe VerifyHashes`.
- Use the hashes in `Assets\DX\Hashes\Original ROM`

5. Generate new patch from NTSC-U to Regravitified.

- Run `Sewer56.Patcher.Regravitified.exe GeneratePatch`. 
- Place patch in `Assets\DX\Patches\Vanilla to DX\Patch`

Please note you'll need around 6GB of spare RAM to do this.
It will also take a while, so go watch an episode of Anime.