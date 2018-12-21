using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		public static Predicate<T> ToPredicate<T>(this Func<T, bool> func)
		{
			return new Predicate<T>(func);
		}

		public static Func<T, bool> ToFunc<T>(this Predicate<T> predicate)
		{
			return new Func<T, bool>(predicate);
		}

	}
}