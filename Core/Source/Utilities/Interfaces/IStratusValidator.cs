using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// Interface for validating the settings of a behaviour
	/// </summary>
	public interface IStratusValidator
	{
		StratusObjectValidation Validate();
	}

	/// <summary>
	/// Interface for a component at the top level of a hierarchy of validators
	/// </summary>
	public interface IStratusValidatorAggregator
	{
		StratusObjectValidation[] Validate();
	}
}