using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// Acts a proxy for collision events, observing another GameObject's collision messages
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class StratusCollisionProxy : StratusProxy
	{
		/// <summary>
		/// A callback consisting of the Collider information
		/// </summary>
		[System.Serializable]
		public class TriggerCallback : UnityEvent<Collider> { }

		/// <summary>
		/// A callback consisting of the Collision information
		/// </summary>
		[System.Serializable]
		public class CollisionCallback : UnityEvent<Collision> { }

		/// <summary>
		/// The type of collision message to check for
		/// </summary>
		public enum CollisionMessage
		{
			TriggerEnter,
			TriggerExit,
			TriggerStay,
			CollisionEnter,
			CollisionExit,
			CollisionStay
		}

		/// <summary>
		/// Arguments for constructing
		/// </summary>
		public class Arguments
		{
			public Collider target;
			public CollisionMessage message;
			public OnTriggerMessage onTrigger;
			public OnCollisionMessage onCollision;
			public bool debug = false;
			public bool persistent = true;

			public Arguments(Collider target, CollisionMessage message, OnTriggerMessage onTrigger)
			{
				this.target = target;
				this.message = message;
				this.onTrigger = onTrigger;
			}

			public Arguments(Collider target, CollisionMessage message, OnCollisionMessage onCollision)
			{
				this.target = target;
				this.message = message;
				this.onCollision = onCollision;
			}

			public Arguments(Collider target, CollisionMessage message, OnTriggerMessage onTrigger, OnCollisionMessage onCollision)
			{
				this.target = target;
				this.message = message;
				this.onTrigger = onTrigger;
				this.onCollision = onCollision;
			}
		}

		public delegate void OnTriggerMessage(Collider collider);
		public delegate void OnCollisionMessage(Collision collision);

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// 
		/// </summary>
		[Header("Collision")]
		[Tooltip("What type of collision message to listen to")]
		public CollisionMessage type;

		/// <summary>
		/// Subscribes to collision events on this proxy
		/// </summary>
		//[DrawIf(nameof(CollisionProxy.isTrigger), true, ComparisonType.Equals)]
		[SerializeField]
		public TriggerCallback triggerCallback = new TriggerCallback();

		/// <summary>
		/// Subscribes to collision events on this proxy
		/// </summary>
		//[DrawIf(nameof(CollisionProxy.isTrigger), true, ComparisonType.NotEqual)]
		[SerializeField]
		public CollisionCallback collisionCallback = new CollisionCallback();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes to collision events on this proxy
		/// </summary>
		public OnTriggerMessage onTrigger { get; set; }
		/// <summary>
		/// Subscribes to collision events on this proxy
		/// </summary>
		public OnCollisionMessage onCollision { get; set; }
		/// <summary>
		/// Whether this proxy is firing off triggers
		/// </summary>
		public bool isTrigger => (type == CollisionMessage.TriggerEnter || type == CollisionMessage.TriggerStay || type == CollisionMessage.TriggerExit);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void OnTriggerEnter(Collider other)
		{
			OnCollision(CollisionMessage.TriggerEnter, other, null);
		}

		private void OnTriggerStay(Collider other)
		{
			OnCollision(CollisionMessage.TriggerStay, other, null);
		}
		private void OnTriggerExit(Collider other)
		{
			OnCollision(CollisionMessage.TriggerExit, other, null);
		}

		private void OnCollisionEnter(Collision collision)
		{
			OnCollision(CollisionMessage.CollisionEnter, collision.collider, collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			OnCollision(CollisionMessage.CollisionExit, collision.collider, collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			OnCollision(CollisionMessage.CollisionStay, collision.collider, collision);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructs a proxy in order to observe another GameObject's trigger messages
		/// </summary>
		/// <param name="target"></param>
		/// <param name="type"></param>
		/// <param name="onTrigger"></param>
		/// <param name="persistent"></param>
		/// <returns></returns>
		public static StratusCollisionProxy Construct(Collider target, CollisionMessage type, OnTriggerMessage onTrigger, bool persistent = true)
		{
			var proxy = target.gameObject.AddComponent<StratusCollisionProxy>();
			proxy.type = type;
			proxy.onTrigger += onTrigger;
			proxy.persistent = persistent;
			return proxy;
		}

		/// <summary>
		/// Constructs a proxy in order to observe another GameObject's trigger messages
		/// </summary>
		/// <param name="target"></param>
		/// <param name="type"></param>
		/// <param name="onTrigger"></param>
		/// <param name="persistent"></param>
		/// <returns></returns>
		public static StratusCollisionProxy Construct(Collider target, CollisionMessage type, OnTriggerMessage onTrigger, OnCollisionMessage onCollision, bool persistent = true)
		{
			var proxy = target.gameObject.AddComponent<StratusCollisionProxy>();
			proxy.type = type;
			proxy.onTrigger += onTrigger;
			proxy.onCollision += onCollision;
			proxy.persistent = persistent;
			return proxy;
		}

		/// <summary>
		/// Constructs a proxy in order to observe another GameObject's collision messages
		/// </summary>
		public static StratusCollisionProxy Construct(Arguments arguments)
		{
			var proxy = arguments.target.gameObject.AddComponent<StratusCollisionProxy>();
			proxy.type = arguments.message;
			proxy.persistent = arguments.persistent;
			proxy.debug = arguments.debug;
			proxy.onCollision += arguments.onCollision;
			proxy.onTrigger += arguments.onTrigger;
			return proxy;
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void OnCollision(CollisionMessage type, Collider collider, Collision collision)
		{
			if (this.type != type)
			{
				return;
			}

			if (debug)
			{
				this.Log($"Collision of type {type} against {collider}");
			}

			if (isTrigger)
			{
				triggerCallback?.Invoke(collider);
				onTrigger?.Invoke(collider);
			}
			else
			{
				collisionCallback?.Invoke(collision);
				onCollision?.Invoke(collision);
			}
		}



	}

}