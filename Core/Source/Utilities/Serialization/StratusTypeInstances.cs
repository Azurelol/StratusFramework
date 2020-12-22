using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Used for managing default instances of the subclasses of a given class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusSubclassInstancer<T> where T : class
	{
		private Type baseType;
		private Type[] types;
		private Dictionary<Type, T> instances;

		public StratusSubclassInstancer()
		{
			baseType = typeof(T);
			types = Utilities.StratusReflection.GetSubclass<T>();
			instances = types.ToDictionary<Type, T>((Type t) => (T)Activator.CreateInstance(t));
		}

		public U Get<U>() where U : T
		{
			return (U)instances.GetValueOrNull(typeof(U));
		}

		public T Get(Type type)
		{
			return instances.GetValueOrNull(type);
		}

	}

}