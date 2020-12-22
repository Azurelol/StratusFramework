using Stratus.Dependencies.Ludiq.Reflection;
using Stratus.Dependencies.TypeReferences;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

namespace Stratus.Analytics
{
	public class AnalyticsCollector : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Tooltip("What schema is being used")]
		public AnalyticsSchema schema;

		[Tooltip("The member that will be inspected")]
		[Filter(typeof(Vector2), typeof(Vector3), typeof(int), typeof(float), typeof(bool),
				Methods = false, Properties = true, NonPublic = true, ReadOnly = true,
				Static = true, Inherited = true, Fields = true)]
		public UnityMember member;

		// Conditions: When the data should be collected
		[Header("Condition")]
		public Condition condition;
		// Event
		[Tooltip("The scope of the event")]
		public StratusEvent.Scope eventScope;
		[ClassExtends(typeof(Stratus.StratusEvent), Grouping = ClassGrouping.ByNamespace)]
		[Tooltip("What type of event will have this data be collected")]
		public ClassTypeReference eventType;
		// Timer
		[Tooltip("How often the members are polled for their current value")]
		[Range(0f, 3f)]
		public float onTimer = 1f;
		// Messages
		public MessageType onMessage;
		//public TriggerLifecycleEvent onLifecycleEvent;
		public EventTriggerType onEventTriggerType;

		//[Header("Rules")]
		//public bool useRules;
		//[Tooltip("The maximum amount of times ")]
		//public int repetitions = 0;
		[HideInInspector]
		[SerializeField]
		public Analysis.Attribute attribute;

		private bool debug = false;
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public UnityEngine.EventSystems.EventTrigger eventTrigger { get; private set; }
		public StratusEventProxy eventProxy { get; private set; }
		public GameObject targetGameObject { get; set; }
		public Transform targetTransform { get; set; }
		public object latestValue { get; set; }
		public bool hasValue => this.latestValue != null;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			//if (Application.isPlaying)

			switch (this.condition)
			{
				case Condition.Timer:
					StratusUpdateSystem.Add(this.onTimer, this.Submit, this);
					break;

				case Condition.Nessage:
					switch (this.onMessage)
					{
						case MessageType.LifecycleEvent:
							// proxy here?
							break;
						case MessageType.EventTriggerType:
							//eventTrigger = this.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
							//eventTrigger.triggers.Add(new UnityEngine.EventSystems.EventTrigger.Entry())
							break;
					}
					break;

				case Condition.Event:
					this.eventProxy = StratusEventProxy.Construct(this.gameObject, this.eventScope, this.eventType, this.OnEvent, true, this.debug);
					break;
				default:
					break;
			}
		}

		private void OnDestroy()
		{

			switch (this.condition)
			{
				case Condition.Timer:
					StratusUpdateSystem.Remove(this);
					break;
				case Condition.Nessage:
					break;
				case Condition.Event:
					Destroy(this.eventProxy);
					break;
				default:
					break;
			}
		}

		private void OnEnable()
		{
			AnalyticsEngine.Connect(this);
		}

		private void OnDisable()
		{
			AnalyticsEngine.Disconnect(this);
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnEvent<T>(T e) where T : Stratus.StratusEvent
		{
			this.Submit();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Submits the current data point of the target
		/// </summary>
		public void Submit()
		{
			if (!this.Collect())
			{
				return;
			}

			StratusDebug.Log($"Submitting {this.latestValue}", this);
			AnalyticsEngine.Submit(new AnalyticsPayload(this.attribute, this.latestValue, Time.realtimeSinceStartup));
			//if (target.hasValue)

		}

		/// <summary>
		/// Records the current value of the given member
		/// </summary>
		/// <returns></returns>
		public bool Collect()
		{
			if (!this.member.isAssigned)
			{
				return false;
			}

			this.latestValue = this.member.Get();
			return true;
		}

	}

}