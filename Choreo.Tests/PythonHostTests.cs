using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Choreo.Tests
{
    [TestClass]
    public class PythonHostTests
    {
        [TestMethod]
        public void Execute_ArithmeticExpression_HostExecutesAndReturnsResult()
        {
            var pyHost = new PythonHost();
            Assert.AreEqual(2, pyHost.Execute<int>("1 + 1"));
        }

        [TestMethod]
        public void GetFunctions_TwoFunctions_HostReturnsBoth()
        {
            var pyHost = new PythonHost();
            CollectionAssert.AreEqual(new[] { "foo", "doo"},
                pyHost.GetFunctions(
@"def foo():
    pass 

def doo():
    pass").ToArray());
            
        }
    }
}
