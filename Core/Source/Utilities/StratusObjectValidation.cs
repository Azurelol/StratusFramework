using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// A validation message for the user
	/// </summary>
	public class StratusObjectValidation : StratusValidation
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// Maps to UnityEditor.MessageType
		/// </summary>
		public enum Level
		{
			Info = 1,
			Warning = 2,
			Error = 3
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public Level type { get; protected set; }
		public UnityEngine.Object target { get; protected set; }
		public Func<bool> onValidate { get; protected set; }
		public System.Action onSelect { get; protected set; }
		public bool hasContext { get; protected set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusObjectValidation(string message, Level type, UnityEngine.Object target, Func<bool> onValidate = null) : base(!string.IsNullOrEmpty(message), message)
		{
			this.type = type;
			this.target = target;
			this.onSelect = null;
			this.onValidate = onValidate;
			hasContext = target != null || onValidate != null;
		}

		public StratusObjectValidation(string message, Level type, System.Action onSelect, Func<bool> onValidate = null) : base(!string.IsNullOrEmpty(message), message)
		{
			this.type = type;
			this.target = null;
			this.onSelect = onSelect;
			this.onValidate = onValidate;
			hasContext = target != null || onValidate != null || onSelect != null;
		}

		public StratusObjectValidation(Level type, UnityEngine.Object target, Func<bool> onValidate = null)
		{
			this.type = type;
			this.target = target;
			this.onSelect = null;
			this.onValidate = onValidate;
			hasContext = target != null || onValidate != null;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static StratusObjectValidation[] Aggregate(IEnumerable<IStratusValidator> validators)
		{
			List<StratusObjectValidation> messages = new List<StratusObjectValidation>();
			foreach (var t in validators)
			{
				var msg = t.Validate();
				if (msg != null && msg.valid)
					messages.Add(msg);
			}
			return messages.ToArray();
		}

		public static StratusObjectValidation[] Aggregate(IEnumerable<IStratusValidatorAggregator> validators)
		{
			List<StratusObjectValidation> messages = new List<StratusObjectValidation>();
			foreach (var t in validators)
			{
				var msg = t.Validate();
				if (msg != null)
					messages.AddRange(msg);
			}
			return messages.ToArray();
		}

		public static StratusObjectValidation[] Aggregate(IStratusValidatorAggregator validator)
		{
			return validator.Validate();
		}

		public static StratusObjectValidation Generate(IStratusValidator validator)
		{
			return validator.Validate();
		}

		public static StratusObjectValidation NullReference(Behaviour behaviour, string description = null)
		{
			FieldInfo[] nullFields = Stratus.Utilities.StratusReflection.GetFieldsWithNullReferences(behaviour);
			if (nullFields.Empty())
				return null;

			string label = behaviour.GetType().Name;
			if (description != null)
				label += $" {description}";

			string msg = $"{label} has the following fields currently set to null:";
			foreach (var f in nullFields)
				msg += $"\n - <i>{f.Name}</i>";
			StratusObjectValidation validation = new StratusObjectValidation(msg, Level.Warning, behaviour);
			return validation;
		}

		/// <summary>
		/// Concatenates two validation messages together, provided they are to the same target.
		/// The level is raised to the highest of the two.
		/// </summary>
		/// <param name="other"></param>
		public void Add(StratusObjectValidation other)
		{
			if (other == null || other.target != this.target)
				return;

			message += other.message;
			if (other.type > this.type)
				this.type = other.type;
		}

		/// <summary>
		/// Concatenates the given message onto this validation.
		/// </summary>
		/// <param name="other"></param>
		public void Add(string message)
		{
			this.message += message;
		}

	}
}