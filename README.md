# H3EK-FaceFXWrapper
A middleman tool which allows you to use the Dragon Age Origins toolset build of FaceFX with the Halo 3 & Halo Reach Editing Kits to generate lipsync animations.


Please mind the gnarly text parsing code.
# Setup Guide
1) Download the [Dragon Age Origins toolset](http://lvlt.bioware.cdn.ea.com/bioware/u/f/eagames/bioware/dragonage/toolset/DragonAgeToolset1.01Setup.exe)
2) Download and install the [.NET 5.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/5.0/runtime?cid=getdotnetcore&os=windows&arch=x64)
3) Download the latest release of the [middleman tool](https://github.com/yokimklein/H3EK-FaceFXWrapper/releases)
4) Open DragonAgeToolset1.01Setup.exe with 7-Zip and copy the contents of DragonAgeToolset\FaceFX to H3EK\bin\FaceFX
5) Rename FxStudio.exe to FxStudioOriginal.exe
6) Copy the contents of H3EK-FaceFXWrapper1.X.X.zip into your H3EK\bin\FaceFX folder (make sure this includes the bungie_facefx_actors folder!)
7) (Optional) Add a .txt file placed next to your sound file with the same name in the data folder, containing a read out of your voice lines for better lipsync generation
8) Import your sound using tool.exe with either the unit_dialog, cinematic_dialog or mission_dialog sound classes

# Limitations
- Tool cannot import more than 1024 curve keys, so the wrapper will only write the first 1024 to a FXX file. This should only be an issue for very long sounds (greater than a few minutes in length). Avoid using lengthy sounds for lipsync.
- Check the FaceFXWrapper.log file if you suspect there has been an issue with the lipsync generation.

# Example
1) Move cave_johnson.txt & cave_johnson.wav from the SampleData folder into H3EK\data
2) Run tool.exe sound-single "cave_johnson.wav" cinematic_dialog "fmod\pc\english.fsb"
[![H3EK Full Lipsync Generation on YouTube](https://i3.ytimg.com/vi/kjMR_M8xbb4/maxresdefault.jpg)](http://www.youtube.com/watch?v=kjMR_M8xbb4 "H3EK Full Lipsync Generation")
# How does this work?
- Development studios were often given access to FaceFX's source code when they purchased a license, so these studios would sometimes make modifications.
- Bungie added the exportAnim command to their build, which would export the facial animation data to their proprietary .fxx format.
- The only publicly available version of old FaceFX I could find from around Halo 3's development was a build found in the Dragon Age Origins toolset. It lacks the exportAnim command, but it still allows you to export to the older text based LTF format which Halo 2 used.
- FXX files are just LTF files in raw binary form, except they don't contain any phoneme timing data.
- The FaceFXWrapper tool sits in place of the FxStudio executable, so when tool.exe gives it commands to generate lipsync data with, the wrapper intercepts it and replaces the exportAnim command with exportLTF. Once FaceFX generates the LTF file, the wrapper converts the LTF to the FXX format for tool to use.

# Advanced usage
- Use "FxStudio.exe -convertLTF <path>" to convert an LTF file to FXX
- Set the file paths in FaceFXWrapper.cfg to use a custom LTF or FXX file when importing a sound. This will override the FXX or LTF files used as long as the paths are set. Make sure to clear the config after use
