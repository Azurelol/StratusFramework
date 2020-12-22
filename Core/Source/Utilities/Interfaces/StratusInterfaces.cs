using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Stratus.Interfaces
{
	/// <summary>
	/// Provides global access to interfaces
	/// </summary>
	public static class Global
	{
		/// <summary>
		/// Toggle for all behaviours inheriting from DebugToggle
		/// </summary>
		/// <param name="toggle"></param>
		public static void DebugToggle(bool toggle)
		{
			IStratusDebuggable[] toggles = FindInterfaces<IStratusDebuggable>();
			foreach (var t in toggles)
				t.Toggle(toggle);

			StratusDebug.Log($"Debug = {toggle} on {toggles.Length} behaviours.");
		}

		/// <summary>
		/// Toggle for specific behaviours implementing DebugToggle
		/// </summary>
		/// <param name="toggle"></param>
		public static void DebugToggle<T>(bool toggle) where T : IStratusDebuggable
		{
			T[] toggles = FindInterfaces<IStratusDebuggable>().OfType<T>().ToArray();
			foreach (var t in toggles)
				t.Toggle(toggle);

			StratusDebug.Log($"Debug = {toggle} on {toggles.Length} behaviours.");
		}

		/// <summary>
		/// Validates all loaded behaviours that implement Validator
		/// </summary>
		/// <param name="toggle"></param>
		public static StratusObjectValidation[] Validate()
		{
			IStratusValidator[] validators = FindInterfaces<IStratusValidator>();
			var messages = StratusObjectValidation.Aggregate(validators);
			StratusDebug.Log($"Validated {validators.Length} behaviours.");
			return messages.ToArray();
		}

		/// <summary>
		/// Validates all loaded behaviours that implement Validator
		/// </summary>
		/// <param name="toggle"></param>
		public static StratusObjectValidation[] ValidateAggregate()
		{
			IStratusValidatorAggregator[] validators = FindInterfaces<IStratusValidatorAggregator>();
			var messages = StratusObjectValidation.Aggregate(validators);
			StratusDebug.Log($"Validated {validators.Length} behaviours.");
			return messages.ToArray();
		}

		/// <summary>
		/// Finds any loaded components that implement the specified interfaces 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] FindInterfaces<T>() where T : class
		{
			return MonoBehaviour.FindObjectsOfType<MonoBehaviour>().OfType<T>().ToArray();
		}
	}





}