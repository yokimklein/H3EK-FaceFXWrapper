using H3EK_FaceFX_Wrapper.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace H3EK_FaceFX_Wrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] != "-exec")
                return;
            string commands = args[1];

            // split FaceFX command args
            int pFrom = commands.IndexOf("loadActor ") + "loadActor ".Length;
            int pTo = commands.IndexOf(";");
            string loadactorArgs = commands.Substring(pFrom, pTo - pFrom);
            loadactorArgs = loadactorArgs.Replace("default.fxa", "johnson.fxa");
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

            // write commands to file to use with FaceFX's exec arg
            using (StreamWriter sw = File.CreateText("commands.fxl"))
            {
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
            while (!File.Exists(ltfPath));
            
            // load and convert the LTF file
            FXX_File FXX = new FXX_File(ltfPath);
            // write FXX file
            FXX.WriteTo(ltfPath.Replace(".ltf", ".fxx"));
            File.Delete(ltfPath);
        }
    }
}
