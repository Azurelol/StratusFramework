using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns true if the list is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool Empty<T>(this ICollection<T> collection)
		{
			return collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the list not null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static bool NotNullOrEmpty<T>(this ICollection<T> collection)
			where T : class
		{
			return collection != null && collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the array is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool NotEmpty<T>(this ICollection<T> collection)
		{
			return collection != null && collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the list is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool Empty<T>(this Stack<T> collection)
		{
			return collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the array is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool NotEmpty<T>(this Stack<T> stack)
		{
			return stack.Count > 0;
		}

		/// <summary>
		/// Returns true if the list is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool Empty<T>(this Queue<T> collection)
		{
			return collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the array is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool NotEmpty<T>(this Queue<T> stack)
		{
			return stack.Count > 0;
		}

		/// <summary>
		/// Pushes all the given elements onto the stack
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="values"></param>
		public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> values)
		{
			foreach (var element in values)
			{
				stack.Push(element);
			}
		}

		/// <summary>
		/// Pushes all the given elements onto the stack
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="values"></param>
		public static void PushRange<T>(this Stack<T> stack, params T[] values)
		{
			stack.PushRange((IEnumerable<T>)values);
		}

		/// <summary>
		/// Enqueues all the given elements onto the queue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="values"></param>
		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> values)
		{
			foreach (var element in values)
			{
				queue.Enqueue(element);
			}
		}

		/// <summary>
		/// Enqueues all the given elements onto the queue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="values"></param>
		public static void EnqueueRange<T>(this Queue<T> queue, params T[] values)
		{
			queue.EnqueueRange((IEnumerable<T>)values);
		}

		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			return stack.Count > 0 ? stack.Pop() : default;
		}

		public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;


	}
}