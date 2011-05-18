using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using IronPython.Runtime;
using System.IO;

namespace Choreo
{
    public class PythonHost : IDisposable
    {
        ScriptEngine engine;
        IDictionary<string, object> sharedObjects = new Dictionary<string, object>();

        public PythonHost()
        {
            engine = Python.CreateEngine();
        }

        public void AddSharedObject(string objectName, object obj)
        {
            sharedObjects[objectName] = obj;
        }

        public void Execute(string code)
        {
            var source = engine.CreateScriptSourceFromString(code);
            var scope = engine.CreateScope(sharedObjects);

            source.Execute(scope);
        }

        public T Execute<T>(string code)
        {
            var source = engine.CreateScriptSourceFromString(code);
            return source.Execute<T>();
        }

        public void ExecuteFunctionInFile(string loadPath, string fullyQualifiedFunctionName)
        {
            var fileNameAndFunctionName = fullyQualifiedFunctionName.Split('.');
            var fullFilePath = Path.ChangeExtension(Path.Combine(loadPath, fileNameAndFunctionName[0]), "py");

            var source = engine.CreateScriptSourceFromFile(fullFilePath);
            var scope = engine.CreateScope(sharedObjects);
            source.Execute(scope);

            var functionCallSource = engine.CreateScriptSourceFromString(fileNameAndFunctionName[1] + "()");
            functionCallSource.Execute(scope);
        }

        public IEnumerable<string> GetFullyQualifiedFunctionNamesInFile(string fileName)
        {
            var source = engine.CreateScriptSourceFromFile(fileName);
            var moduleName = Path.GetFileNameWithoutExtension(fileName);
            var scope = engine.CreateScope(sharedObjects);
            source.Execute(scope);

            var items = scope.GetItems();

            return items.Where(item => item.Value is PythonFunction)
                .Select(kvp => (PythonFunction)kvp.Value)
                .Select(function => moduleName + "." + function.func_name);
        }

        public IEnumerable<string> GetFunctions(string code)
        {
            var source = engine.CreateScriptSourceFromString(code);
            var scope = engine.CreateScope(sharedObjects);

            source.Execute(scope);

            return scope.GetItems().Select(kvp => kvp.Key).Where(name => !name.StartsWith("__"));
        }

        public void Dispose()
        {
            
        }
    }
}
