using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Stratus.Tests
{
    public class ConsoleCommandTests : IConsoleCommandHandler
    {
        [Test]
        public void TestRegistration()
        {
			Debug.Log("Classes that support console commands:");
			Type[] types = ConsoleCommand.handlerTypes;
			Debug.Log(types.TypeNames().JoinLines());
        }
    }
}
