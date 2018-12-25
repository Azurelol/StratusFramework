using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using System;

namespace Stratus.Tests
{
    public static class Extensions
    {
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[FieldDescription(value1Description)]
		private static float value1;
		private const string value1Description = "A value";
		private static readonly Type extensionsType = typeof(Extensions);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[Test]
        public static void TestAttributes()
        {
			var field = typeof(Extensions).GetFieldExhaustive(nameof(value1));
			var description = field.GetDescription();
			Assert.AreEqual(value1Description, description);
        }
    }
}
