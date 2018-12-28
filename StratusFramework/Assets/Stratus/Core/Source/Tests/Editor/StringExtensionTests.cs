using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Stratus.Tests
{
	public partial class ExtensionsTests
	{
        [Test]
        public void TestLines()
        {
			string value = "hello \n there \n cat";
			Assert.AreEqual(3, value.CountLines());
        }

		[Test]
		public void TestJoin()
		{

		}

		[Test]
		public void TestCase()
		{
			string value3 = "COOL_MEMBER_NAME";
			Assert.AreEqual("Cool Member Name", value3.ToTitleCase());
			string value = "helloThereYou";
			Assert.AreEqual("Hello There You", value.FromCamelCase());
			string value2 = "war and peace";
			Assert.AreEqual("War and Peace", value2.ToTitleCase());			
		}
    }
}
