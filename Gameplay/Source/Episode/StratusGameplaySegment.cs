using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	public interface IStratusGameplaySegment
	{
		string label { get; }
		string description { get; }
		Sprite image { get; }

		void Load();
		void Start();
		void End();

		event Action<IStratusGameplaySegment> onStarted;

		event Action<IStratusGameplaySegment> onEnded;
	}

	public interface IStratusGameplaySegment<T> : IStratusGameplaySegment
	{
		T value { get; }
	}

	/// <summary>
	/// Base class for gameplay segments
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusGameplaySegment : StratusScriptable,
		IStratusGameplaySegment
	{
		[SerializeField]
		private string _label;
		public string label => _label;

		[SerializeField, TextArea]
		private string _description;
		public string description => _description;

		[SerializeField]
		private Sprite _image;
		public Sprite image => _image;

		public event Action<IStratusGameplaySegment> onSetup;
		public event Action<IStratusGameplaySegment> onStarted;
		public event Action<IStratusGameplaySegment> onEnded;

		public override string ToString()
		{
			return label;
		}

		public virtual void Load()
		{
			onSetup?.Invoke(this);
		}

		public virtual void Start()
		{
			onStarted?.Invoke(this);
		}

		public virtual void End()
		{
			onEnded?.Invoke(this);
		}
	}

	/// <summary>
	/// Base class for gameplay segments
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public abstract class StratusGameplaySegment<T> : StratusGameplaySegment,
		IStratusGameplaySegment<T>
		where T : class, new()
	{
		public abstract class BaseEvent : StratusEvent
		{
			public BaseEvent(IStratusGameplaySegment<T> segment)
			{
				this.segment = segment;
			}

			public IStratusGameplaySegment<T> segment { get; private set; }
		}

		public class SetupEvent : BaseEvent
		{
			public SetupEvent(IStratusGameplaySegment<T> segment) : base(segment)
			{
			}
		}

		public class StartedEvent : BaseEvent
		{
			public StartedEvent(IStratusGameplaySegment<T> segment) : base(segment)
			{
			}
		}

		public class EndedEvent : BaseEvent
		{
			public EndedEvent(IStratusGameplaySegment<T> segment) : base(segment)
			{
			}
		}


		[SerializeField]
		public T value = new T();

		T IStratusGameplaySegment<T>.value => this.value;

		public StratusGameplaySegment()
		{
		}

		public override void Load()
		{
			base.Load();
			StratusScene.Dispatch<SetupEvent>(new SetupEvent(this));
		}

		public override void Start()
		{
			base.Start();
			StratusScene.Dispatch<StartedEvent>(new StartedEvent(this));
		}

		public override void End()
		{
			base.End();
			StratusScene.Dispatch<EndedEvent>(new EndedEvent(this));
		}
	}
}