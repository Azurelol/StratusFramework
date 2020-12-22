using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public partial class StratusExtensionsTests
	{
		private const string testDataObjectDescription = "A test class used for the unit tests";
		[ClassDescription(testDataObjectDescription)]
		private class TestDataObject
		{
			[MemberDescription(nameDescription)]
			public string name;
			public const string nameDescription = "The name of the object";

			[HideInInspector, SerializeField]
			public int value;

			[MemberDescription(inverseValueDescription)]
			public int inverseValue => -this.value;
			public const string inverseValueDescription = "The inverse value";

			public TestDataObject(string name, int value)
			{
				this.name = name;
				this.value = value;
			}

			public override string ToString()
			{
				return this.name;
			}

			public void Boop(int n, int b)
			{
				n.Iterate(() => Console.WriteLine(this.value + b));
			}
		}

		[Test]
		public void TestIntegerExtensions()
		{
			// Iterate
			{
				int n = 3;

				int result = 0;
				n.Iterate(() => result += 1);
				Assert.AreEqual(n, result);

				result = 0;
				n.Iterate((i) => result += i * 2);
				Assert.AreEqual(0 * 2 + 1 * 2 + 2 * 2, result);

				result = 3;
				n.IterateReverse((i) =>
				{
					Assert.AreEqual(result - 1, i);
					result--;
				});
			}
		}

		[Test]
		public static void TestFloatExtensions()
		{
			float a = 0f, b = 1f;

			// Lerp To
			Assert.AreEqual(0f, a.LerpTo(b, 0f));
			Assert.AreEqual(0.25f, a.LerpTo(b, 0.25f));
			Assert.AreEqual(0.5f, a.LerpTo(b, 0.5f));
			Assert.AreEqual(0.75f, a.LerpTo(b, 0.75f));
			Assert.AreEqual(1f, a.LerpTo(b, 1f));

			// Lerp From
			Assert.AreEqual(0f, b.LerpFrom(a, 0f));
			Assert.AreEqual(0.25f, b.LerpFrom(a, 0.25f));
			Assert.AreEqual(0.5f, b.LerpFrom(a, 0.5f));
			Assert.AreEqual(0.75f, b.LerpFrom(a, 0.75f));
			Assert.AreEqual(1f, b.LerpFrom(a, 1f));

			// To Percentage String
			float percentage = 0.75f;
			Assert.AreEqual(percentage.ToPercentageRoundedString(), $"75%");
			Assert.AreEqual(75f.ToPercent(), percentage);
		}

		

		[Test]
		public void TestStringExtensions()
		{
			// Count Lines, Trim Lines
			{
				string value = "hello\nthere\ncat";
				Assert.AreEqual(3, value.CountLines());
				value = value.ReplaceNewLines(" ");
				Assert.AreEqual("hello there cat", value);
			}

			// Title Case
			{
				void TestTitleCase(string value, string expected) => Assert.AreEqual(expected, value.ToTitleCase());
				TestTitleCase("COOL_MEMBER_NAME", "Cool Member Name");
				TestTitleCase("war and peace", "War And Peace");
				TestTitleCase("cool_class_name", "Cool Class Name");
				TestTitleCase("_cool_class_name", "Cool Class Name");
				TestTitleCase("_coolClassName", "Cool Class Name");
			}

			// Upper First
			{
				string value = "cat";
				Assert.AreEqual("Cat", value.UpperFirst());
			}

			// Append Line
			{
				string value = "dog";
				Assert.AreEqual("dog\ncat", value.AppendLine("cat"));
			}

			// Join
			{
				string[] values = new string[]
				{
					"A",
					"B",
					"C"
				};
				Assert.AreEqual("A B C", values.Join(" "));
				Assert.AreEqual("A,B,C", values.Join(","));
				Assert.AreEqual("A\nB\nC", values.JoinLines());
			}

			// Null or Empty, Valid
			{
				string value = null;
				Assert.True(value.IsNullOrEmpty());
				value = string.Empty;
				Assert.True(value.IsNullOrEmpty());
				value = "Boo!";
				Assert.True(value.IsValid());
			}

			// Rich Text
			{
				string value = "Boo";
				Assert.AreEqual($"<b>Boo</b>", value.ToRichText(FontStyle.Bold));
				Assert.AreEqual($"<i>Boo</i>", value.ToRichText(FontStyle.Italic));
				Assert.AreEqual($"<b><i>Boo</i></b>", value.ToRichText(FontStyle.BoldAndItalic));

				Color color = Color.red;
				Assert.AreEqual($"<b><color=#{color.ToHex()}>Boo</color></b>", value.ToRichText(FontStyle.Bold, color));
			}

		}

		[Test]
		public static void TestVectorExtensions()
		{
			float min = 0f, max = 1f;
			Vector2 value1 = new Vector2(min, max);

			// Inclusive
			for (float i = min; i < max; i += 0.1f)
			{
				Assert.True(value1.ContainsInclusive(i));
			}
			Assert.False(value1.ContainsInclusive(-0.01f));
			Assert.False(value1.ContainsInclusive(1.25f));

			// Exclusive
			Assert.False(value1.ContainsExclusive(max));
			Assert.False(value1.ContainsExclusive(min));

			// Average
			float average = min + max / 2f;
			Assert.AreEqual(average, value1.Average());

			// Trimming
			float x = 1f, y = 2f, z = 3f;
			Vector3 value2 = new Vector3(x, y, z);
			Assert.AreEqual(new Vector2(x, y), value2.XY());
			Assert.AreEqual(new Vector2(x, z), value2.XZ());
			Assert.AreEqual(new Vector2(y, z), value2.YZ());
		}

		[Test]
		public static void TestColorExtensions()
		{
			// To Alpha
			{
				Assert.AreEqual(new Color(1, 0, 0, 0.5f), Color.red.ScaleAlpha(0.5f));
			}

			// To Hex
			{
				Assert.AreEqual(ColorUtility.ToHtmlStringRGBA(Color.blue), Color.blue.ToHex());
			}

			// HSV
			{

			}
		}

		//------------------------------------------------------------------------/
		// Member Info
		//------------------------------------------------------------------------/
		[Test]
		public static void TestMemberInfoExtensions()
		{
			Type testType = typeof(TestDataObject);

			// Get Field Exhaustive 
			FieldInfo nameField = testType.GetFieldExhaustive(nameof(TestDataObject.name));
			FieldInfo valueField = testType.GetFieldExhaustive(nameof(TestDataObject.value));
			PropertyInfo inverseValueProperty = testType.GetProperty(nameof(TestDataObject.inverseValue));

			// Get Description
			{
				Assert.AreEqual(testType.GetDescription(), testDataObjectDescription);
				Assert.AreEqual(nameField.GetDescription(), TestDataObject.nameDescription);
				Assert.AreEqual(inverseValueProperty.GetDescription(), TestDataObject.inverseValueDescription);
			}

			// Get Value <T>			
			TestDataObject a = new TestDataObject("A", 7);
			Assert.AreEqual(7, valueField.GetValue<int>(a));

			// Attribute Has/Get
			{
				Assert.True(nameField.HasAttribute<MemberDescriptionAttribute>());
				Assert.True(nameField.HasAttribute(typeof(MemberDescriptionAttribute)));
				Assert.False(valueField.HasAttribute<MemberDescriptionAttribute>());
				Assert.NotNull(inverseValueProperty.GetAttribute<MemberDescriptionAttribute>());
			}

			// Map Attribute
			{
				Dictionary<Type, Attribute> map = valueField.MapAttributes();
				Assert.True(map.ContainsKey(typeof(HideInInspector)));
				Assert.True(map.ContainsKey(typeof(SerializeField)));
			}

			// Get Full Name
			{
				MethodBase boopMethod = testType.GetMethod(nameof(TestDataObject.Boop));
				Assert.AreEqual("Boop(int n, int b)", boopMethod.GetFullName());
				Assert.AreEqual("int n, int b", boopMethod.GetParameterNames());
			}
		}



	}
}
