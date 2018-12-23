using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[Test]
        public static void TestAttributes()
        {
			var description = value1.GetType().GetDescription();
			Assert.AreEqual(value1Description, description);
        }

        //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        //// `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator MemberInfoExtensionTestsWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}
