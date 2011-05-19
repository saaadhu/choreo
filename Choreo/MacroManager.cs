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
using System.Reflection;

namespace Choreo
{
    static class MacroManager
    {
        static ChoreoPackage package;
        static Dictionary<uint, string> commandIdFunctionMap = new Dictionary<uint, string>();
        static string loadPath;
        static DTE2 dte;
        static IVsProfferCommands4 profferCommands;

        public static void Initialize(ChoreoPackage otherPackage)
        {
            package = otherPackage;
            loadPath = GetMacrosPath();

            profferCommands = (IVsProfferCommands4)Package.GetGlobalService(typeof(IVsProfferCommands));
            dte = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        private static string GetMacrosPath()
        {
            var assemblyLoadPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var macrosPath = Path.Combine(assemblyLoadPath, "Macros");
            return macrosPath;
        }

        public static bool IsKnownCmdId(uint cmdId)
        {
            return commandIdFunctionMap.ContainsKey(cmdId);
        }

        public static void LoadMacros()
        {
            commandIdFunctionMap.Clear();

            var existingMacros = GetMacros().ToArray();

            foreach (var pythonFile in Directory.EnumerateFiles(loadPath, "*.py"))
            {
                using (var pythonHost = new PythonHost())
                {
                    pythonHost.AddSharedObject("dte", dte);
                    var functions = pythonHost.GetFullyQualifiedFunctionNamesInFile(pythonFile);
                    RegisterCommands(functions, existingMacros);
                }
            }

            RemoveZombieMacros(existingMacros);
        }

        public static void Run(uint nCmdId)
        {
            var function = commandIdFunctionMap[nCmdId];
            Execute(function);
        }

        private static void RegisterCommands(IEnumerable<string> functions, IEnumerable<Command> existingMacros)
        {
            foreach (var function in functions)
            {
                var commandName = "Choreo." + function;
                int commandId = CreateOrAttachToExistingCommand(commandName, existingMacros);
                commandIdFunctionMap[(uint)commandId] = function;
            }
        }

        static void RemoveZombieMacros(IEnumerable<Command> existingMacros)
        {
            var commandsToBeDeleted = existingMacros.Where(e => !commandIdFunctionMap.ContainsKey((uint)e.ID));
            foreach (var command in commandsToBeDeleted)
            {
                profferCommands.RemoveNamedCommand(command.Name);
            }
        }

        private static int CreateOrAttachToExistingCommand(string commandName, IEnumerable<Command> existingMacros)
        {
            var existingMacroWithSameName = existingMacros.SingleOrDefault(m => m.Name == commandName);

            if (existingMacroWithSameName != null)
                return existingMacroWithSameName.ID;

            var pkgGuid = Guid.Parse(GuidList.guidChoreoPkgString);
            uint cmdId;
            var guid = GuidList.guidChoreoCmdSet;
            profferCommands.AddNamedCommand(ref pkgGuid, ref guid, commandName, out cmdId, commandName, commandName, null, null, 0, 0, 0, 0, null);
            return (int)cmdId;
        }


        private static IEnumerable<Command> GetMacros()
        {
            var cmdSetGuid = "{" + GuidList.guidChoreoCmdSetString.ToUpperInvariant() + "}";

            foreach (Command command in dte.Commands)
            {
                // Don't include "Refresh Choreo" in the list of macros, even if it's in the same cmd set.
                if (command.Guid == cmdSetGuid && command.ID != PkgCmdIDList.cmdidMyCommand)
                        yield return command;
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
