using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public static class Extensions
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[FieldDescription(field1Description)]
		private static float field1;
		private const string field1Description = "A value";
		private static readonly Type extensionsType = typeof(Extensions);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[Test]
		public static void TestAttributes()
		{
			FieldInfo field1Field = typeof(Extensions).GetFieldExhaustive(nameof(field1));
			Assert.False(field1Field.HasAttribute<ArgumentAttribute>());

			string description = field1Field.GetDescription();
			Assert.AreEqual(field1Description, description);

			float valueToSet = 5;
			field1Field.SetValue(null, valueToSet);
			Assert.AreEqual(valueToSet, field1Field.GetValue(null));
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
		public static void TestFloatExtensions()
		{
			float a = 0f, b = 1f;

			Assert.AreEqual(0f, a.LerpTo(b, 0f));
			Assert.AreEqual(0.25f, a.LerpTo(b, 0.25f));
			Assert.AreEqual(0.5f, a.LerpTo(b, 0.5f));
			Assert.AreEqual(0.75f, a.LerpTo(b, 0.75f));
			Assert.AreEqual(1f, a.LerpTo(b, 1f));

			Assert.AreEqual(0f,    b.LerpFrom(a, 0f));
			Assert.AreEqual(0.25f, b.LerpFrom(a, 0.25f));
			Assert.AreEqual(0.5f,  b.LerpFrom(a, 0.5f));
			Assert.AreEqual(0.75f, b.LerpFrom(a, 0.75f));
			Assert.AreEqual(1f,    b.LerpFrom(a, 1f));
		}


	}
}
