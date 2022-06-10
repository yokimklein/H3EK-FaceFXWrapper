using H3EK_FaceFX_Wrapper.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace H3EK_FaceFX_Wrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            // generate an LTF with FaceFX and convert it to FXX for tool
            if (args[0] == "-exec")
            {
                bool FXXOverride = false;
                bool LTFOverride = false;
                string fxxOverridePath = "";
                string ltfOverridePath = "";
                // read config
                if (File.Exists("FaceFXWrapper.cfg"))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead("FaceFXWrapper.cfg"), Encoding.UTF8))
                    {
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line.StartsWith("FXXOverridePath"))
                            {
                                int from = line.IndexOf("\"") + 1;
                                int to = line.LastIndexOf("\"");
                                fxxOverridePath = line.Substring(from, to - from);
                                if (fxxOverridePath != "")
                                    FXXOverride = true;
                            }
                            else if (line.StartsWith("LTFOverridePath"))
                            {
                                int from = line.IndexOf("\"") + 1;
                                int to = line.LastIndexOf("\"");
                                ltfOverridePath = line.Substring(from, to - from);
                                if (ltfOverridePath != "")
                                    LTFOverride = true;
                            }
                            line = reader.ReadLine();
                        }
                    }
                }

                string commands = args[1];

                // split FaceFX command args
                int pFrom = commands.IndexOf("loadActor ") + "loadActor ".Length;
                int pTo = commands.IndexOf(";");
                string loadactorArgs = commands.Substring(pFrom, pTo - pFrom);
                loadactorArgs = loadactorArgs.Replace("-file ", "-file \"");
                loadactorArgs += "\"";

                string commandsSubstring = commands.Substring(pFrom + pTo - pFrom + 1);
                pFrom = commandsSubstring.IndexOf("analyze ") + "analyze ".Length;
                pTo = commandsSubstring.IndexOf(";");
                string analyzeArgs = commandsSubstring.Substring(pFrom, pTo - pFrom);
                analyzeArgs = analyzeArgs.Replace("-audio ", "-audio \"");
                analyzeArgs += "\"";

                commandsSubstring = commandsSubstring.Substring(pFrom + pTo - pFrom + 1);
                pFrom = commandsSubstring.IndexOf("exportAnim ") + "exportAnim ".Length;
                string exportanimArgs = commandsSubstring.Substring(pFrom);
                exportanimArgs = exportanimArgs.Replace("anim", "animname");
                exportanimArgs = exportanimArgs.Replace("group", "animgroup");
                exportanimArgs = exportanimArgs.Replace(".fxx", ".ltf");
                exportanimArgs = exportanimArgs.Replace("-file ", "-file \"");
                exportanimArgs += "\"";

                pFrom = exportanimArgs.IndexOf("\"") + "\"".Length;
                string ltfPath = exportanimArgs.Substring(pFrom, exportanimArgs.Length - pFrom - 1);

                // handle overrides
                if (FXXOverride)
                    File.Copy(fxxOverridePath, ltfPath.Replace(".ltf", ".fxx"));
                else if (LTFOverride)
                    File.Copy(ltfOverridePath, ltfPath);

                if (!LTFOverride && !FXXOverride)
                {
                    // write commands to file to use with FaceFX's exec arg
                    using (StreamWriter sw = File.CreateText("commands.fxl"))
                    {
                        sw.WriteLine("robobrad -load default"); // DAO FxStudio will crash if robobrad isn't loaded
                        sw.WriteLine("loadActor " + loadactorArgs);
                        sw.WriteLine("analyze " + analyzeArgs);
                        sw.WriteLine("exportLTF " + exportanimArgs);
                        sw.WriteLine("// LTF path: " + ltfPath);
                    }

                    // launch FaceFX
                    Process fxStudio = new Process();
                    fxStudio.StartInfo.FileName = "FxStudioOriginal.exe";
                    fxStudio.StartInfo.Arguments = $"-exec commands.fxl";
                    fxStudio.Start();

                    // wait for FaceFX to produce our LTF file
                    while (!File.Exists(ltfPath)) ;
                }

                if (!FXXOverride)
                {
                    // load and convert the LTF file
                    FXX_File FXX = new FXX_File();
                    if (FXX.loadLTF(ltfPath)) // write FXX if the LTF was loaded successfully
                        FXX.WriteTo(ltfPath.Replace(".ltf", ".fxx"));
                    File.Delete(ltfPath);
                }
            }
            // convert an LTF file directly to FXX
            else if (args[0] == "-convertLTF")
            {
                string ltfPath = args[1];
                FXX_File FXX = new FXX_File();
                if (FXX.loadLTF(ltfPath))
                    FXX.WriteTo(ltfPath.Replace(".ltf", ".fxx"));
            }
        }
    }
}
