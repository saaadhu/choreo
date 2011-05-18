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
        static Dictionary<uint, string> commandIdFunctionMap = new Dictionary<uint, string>();
        static string loadPath;
        static DTE2 dte;

        public static void Initialize(ChoreoPackage otherPackage, string otherLoadPath)
        {
            package = otherPackage;
            loadPath = otherLoadPath;

            dte = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static bool IsKnownCmdId(uint cmdId)
        {
            return commandIdFunctionMap.ContainsKey(cmdId);
        }

        public static void LoadMacros()
        {
            foreach (var pythonFile in Directory.EnumerateFiles(loadPath, "*.py"))
            {
                using (var pythonHost = new PythonHost())
                {
                    pythonHost.AddSharedObject("dte", dte);
                    var functions = pythonHost.GetFullyQualifiedFunctionNamesInFile(pythonFile);
                    RegisterCommands(functions);
                }
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

        private static void Execute(string fullyQualifiedFunctionName)
        {
            using (var pythonHost = new PythonHost())
            {
                pythonHost.AddSharedObject("dte", dte);
                pythonHost.ExecuteFunctionInFile(loadPath, fullyQualifiedFunctionName);
            }
        }
    }
}
