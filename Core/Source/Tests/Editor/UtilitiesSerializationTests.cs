using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;


namespace Stratus.Tests
{
	public static partial class UtilitiesTests
	{
		public static class SerializationTests
		{
			public abstract class A { }

			public class B : A { }
			public class B1 : B { }
			public abstract class B2 : B { }

			public class C : A { }
			public class D { }

			[Test]
			public static void TestSubclassInstancer()
			{
				var instancer = new StratusSubclassInstancer<A>();
				Assert.Null(instancer.Get<A>());
				Assert.NotNull(instancer.Get<B>());
				Assert.NotNull(instancer.Get<B1>());
				Assert.Null(instancer.Get<B2>());
				Assert.NotNull(instancer.Get<C>());
			}

			[Test]
			public static void TestPrefsVariable()
			{
				// Bool
				{
					StratusPlayerPrefsVariable booleanVariable = new StratusPlayerPrefsVariable(
						nameof(booleanVariable), 
						StratusPrefsVariable.VariableType.Boolean);

					booleanVariable.Set(true);
					Assert.AreEqual(true, booleanVariable.Get());
					booleanVariable.Set(false);
					Assert.AreEqual(false, booleanVariable.Get());
					booleanVariable.Delete();
				}

				// Int
				{
					StratusPlayerPrefsVariable intVariable = new StratusPlayerPrefsVariable(
						nameof(intVariable),
						StratusPrefsVariable.VariableType.Integer);

					intVariable.Set(42);
					Assert.AreEqual(42, intVariable.Get());
					intVariable.Set(395);
					Assert.AreEqual(395, intVariable.Get());
					intVariable.Delete();
					Assert.Catch(typeof(ArgumentException), () => intVariable.Set(3f));
				}

			}

		}

		
	}

}