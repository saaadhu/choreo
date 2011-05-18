using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using Microsoft.VisualStudio;
using System.ComponentModel.Design;

namespace Choreo
{
    static class MacroManager
    {
        static ChoreoPackage package;
        static OleMenuCommandService cmdService;
        static Dictionary<uint, string> commandIdFunctionMap = new Dictionary<uint, string>();
        static string loadPath;
        static DTE2 dte;

        public static void Initialize(ChoreoPackage otherPackage, OleMenuCommandService otherCmdService)
        {
            package = otherPackage;
            cmdService = otherCmdService;
            dte = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static bool IsKnownCmdId(uint cmdId)
        {
            return commandIdFunctionMap.ContainsKey(cmdId);
        }

        public static void LoadFile(string path)
        {
            loadPath = path;
            var text = File.ReadAllText(path);

            using(var pythonHost = new PythonHost())
            {
                pythonHost.AddSharedObject("dte", dte); 
                var functions = pythonHost.GetFunctions(text);
                RegisterCommands(functions);
            }
        }

        public static void Run(uint nCmdId)
        {
            var function = commandIdFunctionMap[nCmdId];
            Execute(function);
        }

        private static void RegisterCommands(IEnumerable<string> functions)
        {
            foreach (var function in functions)
            {
                var profferCommands = (IVsProfferCommands4)Package.GetGlobalService(typeof(IVsProfferCommands));
                var pkgGuid = Guid.Parse(GuidList.guidChoreoPkgString);
                uint cmdId;
                var commandName = "Choreo." + function;
                profferCommands.RemoveNamedCommand(commandName);

                var guid = GuidList.guidChoreoCmdSet;

                profferCommands.AddNamedCommand(ref pkgGuid, ref guid, commandName, out cmdId, commandName, function, null, null, 0, 0, 0, 0, null);
                var command = dte.Commands.Item(commandName);
                commandIdFunctionMap[cmdId] = function;
            }
        }

        private static void Execute(string function)
        {
            using (var pythonHost = new PythonHost())
            {
                pythonHost.AddSharedObject("dte", dte);
                pythonHost.Execute(File.ReadAllText(loadPath));
                pythonHost.Execute(function + "()");
            }
        }
    }
}
