- [Generating Patches](#generating-patches)
  - [Creating Patches for Original Riders to SRDX \[DefineConstants == 'SRDX'\]](#creating-patches-for-original-riders-to-srdx-defineconstants--srdx)
  - [Creating Patches for Original Riders to SRTE \[DefineConstants == 'SRTE'\]](#creating-patches-for-original-riders-to-srte-defineconstants--srte)
  - [Creating Self Contained Riders Patches \[DefineConstants == 'SRDXSelfContained'\]](#creating-self-contained-riders-patches-defineconstants--srdxselfcontained)
  - [Creating Patches for Zero Gravity \[DefineConstants == 'REGRAV'\]](#creating-patches-for-zero-gravity-defineconstants--regrav)

# Generating Patches

The patcher is actually a command line program that displays a UI only if it receives no parameters.

Try:
```
dotnet Sewer56.Patcher.Riders.Cli.dll --help
```

You will get the following:
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

For simplicity, patch generation was simplified down with scripts.

## Creating Patches for Original Riders to SRDX [DefineConstants == 'SRDX']

1. Copy the contents of `PatchTemplates/SRDX` to patcher folder. 
   - `Assets` folder and `MAKE_PATCH` scripts.
2. Run `MAKE_PATCH` script.
3. ...
4. Profit!

## Creating Patches for Original Riders to SRTE [DefineConstants == 'SRTE']

1. Copy the contents of `PatchTemplates/SRDX` to patcher folder. 
   - `Assets` folder and `MAKE_PATCH` scripts.
2. Run `MAKE_PATCH` script.
3. ...
4. Profit!

## Creating Self Contained Riders Patches [DefineConstants == 'SRDXSelfContained']

These patches contain the full game on board and use a simple hash check to verify legal ownership of an existing Riders ROM.
To create these, you will need to build the patcher with `<DefineConstants>;SRDXSelfContained;DEV</DefineConstants>`.

1. Copy the contents of `PatchTemplates/SRDXSelfContained` to patcher folder.
   - `Assets` folder and `MAKE_PATCH` scripts.
2. Run `MAKE_PATCH` script.
3. ...
4. Build patcher again without `DEV` constant. `<DefineConstants>;SRDXSelfContained</DefineConstants>`
5. Profit!

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
