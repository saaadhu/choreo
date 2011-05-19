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
        static IVsProfferCommands4 profferCommands;

        public static void Initialize(ChoreoPackage otherPackage, string otherLoadPath)
        {
            package = otherPackage;
            loadPath = otherLoadPath;

            profferCommands = (IVsProfferCommands4)Package.GetGlobalService(typeof(IVsProfferCommands));
            dte = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static bool IsKnownCmdId(uint cmdId)
        {
            return commandIdFunctionMap.ContainsKey(cmdId);
        }

        public static void LoadMacros()
        {
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
            foreach (Command command in dte.Commands)
            {
                if (command.Guid == GuidList.guidChoreoCmdSetString)
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
