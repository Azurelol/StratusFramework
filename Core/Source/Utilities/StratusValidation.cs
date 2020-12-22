using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// A validation message
	/// </summary>
	public class StratusValidation
	{
		public bool valid { get; protected set; }
		public string message { get; protected set; }

		public StratusValidation()
		{
			this.valid = true;
			this.message = string.Empty;
		}

		public StratusValidation(bool valid, string message)
		{
			this.valid = valid;
			this.message = message;
		}

		public StratusValidation(Exception exception)
		{
			this.valid = false;
			this.message = exception.Message;
		}

		public override string ToString()
		{
			return message != null ? $"{valid} ({message})" : $"{valid}";
		}

		public static implicit operator bool(StratusValidation validation) => validation.valid;
		public static implicit operator StratusValidation(bool valid) => new StratusValidation(valid, null);
	}

	/// <summary>
	/// A validation of a value
	/// </summary>
	public class StratusValidation<T> : StratusValidation
		where T : class
	{
		public T value { get; private set; }

		public StratusValidation(T value) : base()
		{
			this.value = value;
		}

		public StratusValidation(Exception exception) : base(exception)
		{
		}

		public StratusValidation(bool valid, string message) : base(valid, message)
		{
		}

		public StratusValidation(T value, bool valid, string message) : this(valid, message)
		{
			this.value = value;
		}
	}
}