using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public partial class StratusExtensionsTests
	{
		// A Test behaves as an ordinary method
		[Test]
		public void TestArrayExtensions()
		{
			// FindIndex
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				Assert.AreEqual(4, values.FindIndex(5));
			}

			// FindIndex, Find, Exists
			{
				string[] values = new string[] { "Hello", "There", "Brown", "Cow" };
				Assert.AreEqual(2, values.FindIndex("Brown"));
				Assert.AreEqual("Hello", values.Find((x) => x.Contains("llo")));
				Assert.IsTrue(values.Contains((x) => x.Equals("There")));
			}

			// Sort
			{
				float a = 1.25f, b = 2.5f, c = 3.75f, d = 5.0f;
				// Custom comparer
				{
					float[] values = new float[] { d, b, a, c };
					values.Sort((x, y) => x > y ? 1 : x < y ? -1 : 0);
					Assert.AreEqual(new float[] { a, b, c, d }, values);
				}
				// Default (from interface)
				{
					float[] values = new float[] { d, b, a, c };
					values.Sort();
					Assert.AreEqual(new float[] { a, b, c, d }, values);
				}
			}

			// Concat
			{
				int[] a = new int[] { 1, 3, 5 }, b = new int[] { 2, 4, 6 };
				Assert.AreEqual(new int[] { 1, 3, 5, 2, 4, 6 }, a.Concat(b));
			}

			// Truncate
			{
				int[] values = new int[] { 1, 2, 3 };
				Assert.AreEqual(new int[] { 2, 3 }, values.TruncateFront());
				Assert.AreEqual(new int[] { 1, 2 }, values.TruncateBack());
				Assert.AreEqual(new int[] { 1, 3 }, values.Truncate(2));
				Assert.AreEqual(new int[] { 1, 2 }, values.Truncate(3));
				Assert.AreEqual(new int[] { 2, 3 }, values.Truncate(1));
			}

			// Append, Prepend
			{
				int a = 1, b = 2, c = 3, d = 4;
				int[] first = new int[] { a, b };
				int[] second = new int[] { c, d };

				int[] third = first.Append(second);
				Assert.AreEqual(new int[] { a, b, c, d }, third);
				int[] fourth = first.Prepend(second);
				Assert.AreEqual(new int[] { c, d, a, b }, fourth);

				int[] fifth = first.AppendWhere((x) => x < 4, second);
				Assert.AreEqual(new int[] { a, b, c }, fifth);

				int[] sixth = first.PrependWhere((x) => x > 3, second);
				Assert.AreEqual(new int[] { d, a, b }, sixth);

			}
		}

		[Test]
		public void TestEnumerableExtensions()
		{
			// Typenames
			{
				object[] values1 = new object[]
				{
					"Hello",
					1,
					2.5f
				};
				Type[] values1Types = new Type[]
				{
				typeof(string),
				typeof(int),
				typeof(float),
				};
				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1.TypeNames());
				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1Types.TypeNames());
			}

			// Names 
			{
				string a = "ABCD", b = "EDFG";
				TestDataObject[] values = new TestDataObject[]
				{
					new TestDataObject(a, 1),
					new TestDataObject(b, 2),
				};
				Assert.AreEqual(new string[] { a, b, }, values.ToStringArray());
				Assert.AreEqual(new string[] { a, b, }, values.ToStringArray(x => x.name));
			}

			// Duplicate Keys
			{
				int[] values = new int[] { 1, 2, 3, 3, 4 };
				Assert.True(values.HasDuplicateKeys());
				Assert.AreEqual(3, values.FindFirstDuplicate());
			}

			// Null
			{
				string a = "Hello", b = "Goodbye";
				string[] values = new string[] { a, null, b };
				Assert.AreEqual(new string[] { a, b }, values.TruncateNull());
			}

			// ForEach
			{
				int a = 1, b = 2, c = 3;
				int[] values = new int[] { a, b, c };
				List<int> values2 = new List<int>();
				values.ForEach((x) => values2.Add(x + 1));
				Assert.AreEqual(new int[] { a + 1, b + 1, c + 1 }, values2.ToArray());
			}

			// Find First
			{
				string a = "12", b = "34", c = "56";
				string[] values = new string[] { a, b, c };
				Assert.AreEqual(b, values.FindFirst(x => x.Contains("3")));
			}

			// Clone
			{
				int[] original = new int[] { 1, 2, 3, 4, 5 };
				int[] copy = (int[])original.Clone();
				Assert.AreNotSame(original, copy);
			}

			// Min/Max/Sum
			{
				{
					int[] values = new int[] { 1, 3, 5, 7, 9 };
					Assert.AreEqual(1, values.Min());
					Assert.AreEqual(9, values.Max());
					Assert.AreEqual(1 + 3 + 5 + 7 + 9, values.Sum());
				}

				{
					TestDataObject min = new TestDataObject("A", 1);
					TestDataObject max = new TestDataObject("D", 12);
					Func<TestDataObject, int> selector = (x) => x.value;

					TestDataObject[] values = new TestDataObject[]
					{
						new TestDataObject("B", 3),
						min,
						new TestDataObject("C", 6),
						max,
						new TestDataObject("E", 5),
					};

					Assert.AreEqual(min.value, values.Min(selector));
					Assert.AreEqual(max.value, values.Max(selector));
					Assert.AreEqual(min, values.SelectMin(selector));
					Assert.AreEqual(max, values.SelectMax(selector));
				}
			}

			// To Dictionary
			{
				string a = "A", b = "B", c = "C";
				TestDataObject[] values = new TestDataObject[]
				{
					new TestDataObject(a, 1),
					new TestDataObject(b, 2),
					new TestDataObject(c, 3),
				};
				Dictionary<string, TestDataObject> dict = values.ToDictionary<string, TestDataObject>((x) => x.name);
				Assert.AreEqual(3, dict.Count);
				Assert.AreEqual(1, dict[a].value);
				Assert.AreEqual(2, dict[b].value);
				Assert.AreEqual(3, dict[c].value);
			}

			// Convert
			{
				int[] a = new int[] { 1, 2, 3, };
				string[] b = a.ToArray<int, string>((x) => x.ToString());
				Assert.AreEqual(new string[] { "1", "2", "3" }, b);
			}
		}

		[Test]
		public void TestCollectionsExtensions()
		{
			{
				// Empty
				{
					List<string> values = new List<string>();
					Assert.True(values.Empty());
					Assert.False(values.NotEmpty());

					values.Add("Boo");
					Assert.True(values.NotEmpty());
					Assert.False(values.Empty());

					values.Clear();
					Assert.False(values.NotNullOrEmpty());
				}

				// Stack: Push Range
				{
					Stack<int> values = new Stack<int>();
					values.PushRange(1, 2, 3);
					Assert.AreEqual(new int[] { 3, 2, 1 }, values.ToArray());
				}

				// Queue : Enqueue Range
				{
					Queue<int> values = new Queue<int>();
					values.EnqueueRange(1, 2, 3);
					Assert.AreEqual(new int[] { 1, 2, 3 }, values.ToArray());
				}
			}
		}

		[Test]
		public void TestListExtensions()
		{
			// Remove Null
			{
				List<string> values = new List<string>
				{
					null
				};
				Assert.AreEqual(1, values.RemoveNull());
			}

			// Add Range
			{
				string a = "12", b = "34", c = "56";
				List<string> values = new List<string>();
				values.AddRange(a, b, c);
				Assert.AreEqual(new string[] { a, b, c }, values.ToArray());
			}

			// ForEach RemoveInvalid
			{
				List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
				values.ForEachRemoveInvalid(
					(tdo) => tdo.value += 1,
					(tdo) => tdo.value < 5);
				Assert.True(values.Count == 1);
				Assert.True(values.First().name == "A" && values.First().value == 4);
			}

			// First/Last 
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

				Assert.AreEqual(1, values.First());
				Assert.AreEqual(9, values.Last());

				values = values.TruncateFront();
				Assert.AreEqual(2, values.First());

				values = values.TruncateBack();
				Assert.AreEqual(8, values.Last());
			}

			// Remove Invalid
			{
				List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
				values.RemoveInvalid((tdo) => tdo.value < 5);
				Assert.True(values.Count == 1);
				Assert.True(values.First().name == "A");
			}

			// Clone
			{
				List<int> original = new List<int>() { 1, 2, 3, 4, 5 };
				List<int> copy = original.Clone();
				Assert.AreNotSame(original, copy);
			}

			// Add Range Where
			{
				TestDataObject a = new TestDataObject("A", 1);
				TestDataObject b = new TestDataObject("B", 2);

				List<TestDataObject> values = new List<TestDataObject>();
				values.AddRangeWhere((x) => x.value > 1, a, b);
				Assert.AreEqual(1, values.Count);
				Assert.AreEqual(b, values.First());
			}

			// Add Range Unique
			{
				TestDataObject a = new TestDataObject("A", 1);

				List<TestDataObject> values = new List<TestDataObject>
				{
					a
				};
				values.AddRangeUnique(a, a);
				Assert.AreEqual(a, values.First());
				Assert.AreEqual(1, values.Count);
			}

		}

		[Test]
		public void TestIListExtensions()
		{
			// Shuffle, Last Index
			{
				int[] values = new int[] { 1, 2, 3, 4, 5 };
				Assert.AreEqual(4, values.LastIndex());
				int[] shuffled = (int[])values.Clone();
				shuffled.Shuffle();
				Assert.AreNotEqual(values, shuffled);
			}

			// Swap
			{
				int[] values = new int[] { 1, 2, 3, 4, 5 };
				values.SwapAtIndex(0, 4);
				Assert.AreEqual(values, new int[] { 5, 2, 3, 4, 1 });
				values.Swap(1, 5);
				Assert.AreEqual(values, new int[] { 1, 2, 3, 4, 5 });
			}

			// First, Last, Random
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				Assert.AreEqual(1, values.First());
				Assert.AreEqual(9, values.Last());

				// You see, random can end up hitting the same index... since it's random
				int a = values.Random();
				int b = -1;
				for (int i = 0; i < values.Length; ++i)
				{
					b = values.Random();
					if (a != b)
					{
						break;
					}
				}
				Assert.AreNotEqual(a, b);
			}

			// Remove First/Last
			{
				List<int> values = new List<int>() { 1, 2, 3, 4 };
				values.RemoveFirst();
				values.RemoveLast();
				Assert.AreEqual(2, values.First());
				Assert.AreEqual(3, values.Last());
			}

			// Has Index, AtIndexOrDefault
			{
				TestDataObject a = new TestDataObject("A", 1);
				TestDataObject[] values = new TestDataObject[]
				{
					a
				};

				Assert.False(values.HasIndex(-1));
				Assert.True(values.HasIndex(0));
				Assert.False(values.HasIndex(1));

				Assert.AreEqual(a, values.AtIndexOrDefault(0));
				Assert.AreEqual(a, values.AtIndexOrDefault(5, a));

			}
		}

		[Test]
		public void TestDictionaryExtensions()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();

			TestDataObject a = new TestDataObject("A", 3);
			TestDataObject b = new TestDataObject("B", 5);
			TestDataObject c = new TestDataObject("C", 7);

			Func<TestDataObject, string> keyFunction = (x) => x.name;

			// Add Range 
			{
				TestDataObject[] range = new TestDataObject[] { a, b, c };
				values.AddRange(keyFunction, range);
				Assert.AreEqual(3, values.Count);

				values.Clear();
				values.AddRange(keyFunction, a, b, c);
				Assert.AreEqual(3, values.Count);

				// Unique
				Assert.False(values.AddUnique(keyFunction(a), a));
				values.AddRangeUnique(keyFunction, range);
				Assert.AreEqual(3, values.Count);

				// Where
				values.Clear();
				values.AddRangeWhere(keyFunction, (x) => x.value < 6, range);
				Assert.AreEqual(2, values.Count);
				Assert.True(values.ContainsKey(a.name));
				Assert.True(values.ContainsKey(b.name));
			}

			// Try Invoke
			{
				int result = 0;
				result = values.TryInvoke(a.name, (tdo) => tdo.value);
				Assert.AreEqual(result, a.value);

				result = -1;
				values.TryInvoke("NULL", (tdo) => { result = tdo.value; });
				Assert.AreNotEqual(result, b.value);
			}
		}
	}
}