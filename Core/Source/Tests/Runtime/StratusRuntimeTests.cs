using System;
using System.Collections;
using NUnit.Framework;
using Stratus.Examples;
using UnityEngine;
using UnityEngine.TestTools;

namespace Stratus.Tests
{
	public static partial class RuntimeTests
	{
		[UnityTest]
		public static IEnumerator TestStratusEvent()
		{
			// Create the object, add the component
			GameObject go = new GameObject("Test");
			EventsSample eventSample = go.AddComponent<EventsSample>();
			Assert.AreEqual(0, eventSample.sampleEventsReceived);
			yield return null;

			// Construct the event
			EventsSample.SampleEvent e = new EventsSample.SampleEvent()
			{
				number = 5
			};

			// Dispatch to game object
			eventSample.gameObject.Dispatch<EventsSample.SampleEvent>(e);
			Assert.AreEqual(1, eventSample.sampleEventsReceived);
			Assert.AreEqual(5, eventSample.latestEvent.number);

			// Dispatch to scene
			e.number = 14;
			StratusScene.Dispatch<EventsSample.SampleEvent>(e);
			Assert.AreEqual(2, eventSample.sampleEventsReceived);
			Assert.AreEqual(14, eventSample.latestEvent.number);
			yield return null;
		}

		[UnityTest]
		public static IEnumerator TestStratusActions()
		{
			GameObject go = new GameObject("Test");
			ActionsSample sample = go.AddComponent<ActionsSample>();
			yield return null;

			// Sequence
			{
				// Property
				StratusActionSet seq = StratusActions.Sequence(sample);
				int targetValue = 14;
				StratusActions.Property(seq, () => sample.integerValue, targetValue, sample.duration, StratusEase.Linear);
				yield return new WaitForSeconds(sample.duration);
				Assert.AreEqual(sample.integerValue, targetValue);

				// Delay, Call
				float initialValue = sample.floatValue;
				float finalValue = 3.5f;
				float duration = 1.75f;
				StratusActionSet seq2 = StratusActions.Sequence(sample);
				StratusActions.Delay(seq2, duration);
				StratusActions.Call(seq2, () => sample.floatValue = finalValue);
				Assert.AreEqual(initialValue, sample.floatValue);

				yield return new WaitForSeconds(duration + 0.05f);
				Assert.AreEqual(finalValue, sample.floatValue);
			}
		}

		[UnityTest]
		public static IEnumerator TestTransformExtensions()
		{
			GameObject go = new GameObject("Test");
			Transform transform = go.transform;
			yield return null;

			// Reset
			{
				Vector3 random = UnityEngine.Random.onUnitSphere;
				transform.localPosition = transform.localScale = random;
				transform.rotation = Quaternion.Euler(random);
				Assert.AreEqual(random, transform.localPosition);
				yield return null;

				go.transform.Reset();
				Assert.AreEqual(Vector3.zero, go.transform.localPosition);
				Assert.AreEqual(Vector3.one, go.transform.localScale);
				Assert.AreEqual(Quaternion.identity, go.transform.rotation);
				yield return null;
			}

			// Center
			{
				Vector3 parentPosition = new Vector3(5, 6, 7);
				GameObject parent = new GameObject("Parent");
				parent.transform.position = parentPosition;
				yield return null;
				transform.SetParent(parent.transform, true);
				Assert.AreNotEqual(transform.position, parentPosition);
				yield return null;
				transform.Center();
				Assert.AreEqual(transform.position, parentPosition);

			}



		}


		private static IEnumerator TestComponent<T>(params Func<T, float>[] actions) where T : MonoBehaviour
		{
			GameObject go = new GameObject("Stratus Test");
			T component = go.AddComponent<T>();
			yield return null;

			foreach (Func<T, float> action in actions)
			{
				float duration = action(component);
				yield return new WaitForSeconds(duration);
			}


		}
	}
}
