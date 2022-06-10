# H3EK-FaceFXWrapper
A middleman tool which allows you to use the Dragon Age Origins toolset build of FaceFX with the Halo 3 Editing Kit for lipsync animation generation.

Please mind the gnarly text parsing code.
# Usage
1) Download the [Dragon Age Origins toolset](http://lvlt.bioware.cdn.ea.com/bioware/u/f/eagames/bioware/dragonage/toolset/DragonAgeToolset1.01Setup.exe)
2) Copy the contents of DragonAgeToolset\FaceFX to H3EK\bin\FaceFX
3) Rename FxStudio.exe to FxStudioOriginal.exe
4) Copy the H3EK-FaceFXWrapper files into your H3EK\bin\FaceFX folder (make sure this includes the bungie_facefx_actors folder!)
5) (Optional) Add a .txt file placed next to your sound file with the same name in the data folder, containing a read out of your voice lines for better lipsync generation
6) Import your sound using tool.exe with either the unit_dialog, cinematic_dialog or mission_dialog sound classes

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
