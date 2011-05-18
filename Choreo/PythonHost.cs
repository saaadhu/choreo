using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

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
