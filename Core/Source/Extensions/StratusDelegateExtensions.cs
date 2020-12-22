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

        public static bool TryInvoke(this Action action)
        {
            if (action != null)
            {
                action();
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1>(this Action<T1> action, T1 arg1)
        {
            if (action != null)
            {
                action(arg1);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2>(this Action<T1,T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action(arg1, arg2);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3);
                return true;
            }
            return false;
        }

        public static bool TryInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
                return true;
            }
            return false;
        }

    }
}