using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;

namespace Stratus
{
	/// <summary>
	/// Provides an easy way to hold a reference member within a class.
	/// Currently powered through Ludiq's awesome UnityMember!
	/// </summary>
	[Serializable]
	public class StratusMemberField
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public UnityMember member = new UnityMember();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public Type type => member.type;
		public bool isAssigned => member.isAssigned;
		public bool isReflected => member.isReflected;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusMemberField()
		{
			this.member = new UnityMember();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/    
		public object Get()
		{
			return member.Get();
		}

		public T Get<T>()
		{
			return member.Get<T>();
		}

		public void Set(object value)
		{
			member.Set(value);
		}

		public void Set<T>(T value)
		{
			member.Set(value);
		}

		public object Invoke(params object[] arguments)
		{
			return member.Invoke(arguments);
		}

		public T Invoke<T>(params object[] arguments)
		{
			return member.Invoke<T>(arguments);
		}

		public void SetTarget(UnityEngine.Object target)
		{
			this.member.target = target;
		}

	}

}