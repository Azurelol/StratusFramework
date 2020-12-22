using UnityEngine;
using System;
using UnityEngine.Serialization;
using Stratus;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
	public enum StratusCanvasWindowTransition
	{
		Toggle,
		Fade,
		None,
		Custom
	}

	public interface IStratusCanvasWindow : IStratusSceneViewIsolate
	{
		void Select();
		void Open();
		void Close();
		int sortingOrder { get; set; }
	}

	/// <summary>
	/// Arguments for opening a window
	/// </summary>
	public class StratusCanvasWindowOpenArguments
	{
		public IStratusCanvasWindow parent;
		public bool visibleOverParent;
		public float? transitionSpeed;
		public Action onOpened;
		public Action onClosed;
		public bool ignoreInpuLayerBlocking;

		public StratusCanvasWindowOpenArguments()
		{
		}

		public StratusCanvasWindowOpenArguments(Action onOpened, Action onClosed)
		{
			this.onOpened = onOpened;
			this.onClosed = onClosed;
		}

		public StratusCanvasWindowOpenArguments(IStratusCanvasWindow parent,
			bool visibleOverParent = false)
		{
			this.parent = parent;
			this.visibleOverParent = visibleOverParent;
		}
	}

	/// <summary>
	/// Arguments for closing a window
	/// </summary>
	public class StratusCanvasWindowCloseArguments
	{
		/// <summary>
		/// If recursive, will close parents recursively
		/// </summary>
		public bool recursive;
		/// <summary>
		/// Function to invoke after the transition is over
		/// </summary>
		public Action onFinished = null;

		public StratusCanvasWindowCloseArguments(Action onFinished)
		{
			this.onFinished = onFinished;
		}

		public StratusCanvasWindowCloseArguments()
		{
		}
	}

	/// <summary>
	/// Window class (for menus and the like) based around manipulating canvas groups
	/// </summary>	
	public abstract class StratusCanvasWindow<T> : StratusSingletonBehaviour<T>,
		IStratusCanvasWindow, IStratusInputLayerProvider
		where T : StratusCanvasWindow<T>
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public class RedirectInputEvent : StratusEvent { public StratusCanvasWindow<T> Window; }

		public class BaseEvent : StratusEvent
		{			
		}

		public class OpenEvent : BaseEvent
		{
			public Action onFinished { get; set; }
		}

		public class CloseEvent : BaseEvent
		{
			public StratusCanvasWindowCloseArguments args { get; set; }
		}

		public class ToggleEvent : BaseEvent
		{
			public Action onFinished { get; set; }
		}

		public class OpenedEvent : StratusEvent { }
		public class ClosedEvent : StratusEvent { }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The target canvas
		/// </summary>
		[FormerlySerializedAs("target")]
		public Canvas canvas;

		/// <summary>
		/// What transition to use
		/// </summary>
		public StratusCanvasWindowTransition transition;

		/// <summary>
		/// How long it takes to do the transition for this window
		/// </summary>
		[DrawIf(nameof(transition), StratusCanvasWindowTransition.Fade, ComparisonType.Equals)]
		private float _transitionSpeed = 0.3f;

		/// <summary>
		/// Whether the window is initially open
		/// </summary>
		[SerializeField]
		private bool initiallyOpen = true;

		/// <summary>
		/// Whether to push an input layer when this window is opened
		/// </summary>
		[SerializeField]
		private bool _pushInputLayer = false;

		/// <summary>
		/// Whether to close the window when cancel input is submitted
		/// </summary>
		[SerializeField]
		private bool closeOnCancelInput = false;

		/// <summary>
		/// The first selectable to be selected when this widow is opened
		/// </summary>
		[SerializeReference]
		private RectTransform _selectableOnOpen;

		/// <summary>
		/// How long it takes to open the window when opened during initialization
		/// (Awake)
		/// </summary>
		protected virtual float initializationDelay => 0.1f;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Routine executed during transitions
		/// </summary>
		private Coroutine transitionRoutine { get; set; }

		/// <summary>
		/// The rect transform used by this window
		/// </summary>
		public RectTransform rectTransform => GetComponentCached<RectTransform>();

		/// <summary>
		/// The input layer used by this canvas window
		/// </summary>
		private StratusInputUILayer inputLayer { get; set; }

		/// <summary>
		/// Whether this window is currently open
		/// </summary>
		public bool open { get; private set; }

		/// <summary>
		/// The canvas group for this window
		/// </summary>
		protected CanvasGroup canvasGroup
		{
			get
			{
				if (_canvasGroup == null && canvas != null)
				{
					_canvasGroup = this.gameObject.GetOrAddComponent<CanvasGroup>();
				}
				return _canvasGroup;
			}
		}
		private CanvasGroup _canvasGroup;

		/// <summary>
		/// Whether this window has initialized
		/// </summary>
		public bool initialized { get; private set; }

		/// <summary>
		/// Whether to push the input layer
		/// </summary>
		bool IStratusInputLayerProvider.pushInputLayer => _pushInputLayer;

		/// <summary>
		/// The selectable on open
		/// </summary>
		protected ISelectHandler selectableOnOpen { get; private set; }

		/// <summary>
		/// The current sorting order
		/// </summary>
		public int sortingOrder
		{
			get => canvas.sortingOrder;
			set => canvas.sortingOrder = value;
		}
		private int? previousSortingOrder;
		private bool? previousOverrideSorting;

		/// <summary>
		/// The current transition speed for opening/closing this window
		/// </summary>
		public float transitionSpeed
		{
			get
			{
				if (openingArguments != null)
				{
					if (openingArguments.transitionSpeed != null)
					{
						return openingArguments.transitionSpeed.Value;
					}
				}
				return _transitionSpeed;
			}
		}

		/// <summary>
		/// Opening arguments for this window
		/// </summary>
		private StratusCanvasWindowOpenArguments openingArguments { get; set; }

		/// <summary>
		/// Closing arguments for this window
		/// </summary>
		private StratusCanvasWindowCloseArguments closingArguments { get; set; }

		/// <summary>
		/// The current event system in the scene
		/// </summary>
		public static EventSystem eventSystem => StratusCanvasUtility.eventSystem;

		/// <summary>
		/// If this window is opened upon the request of another, that is its parent
		/// </summary>
		public IStratusCanvasWindow parentWindow => openingArguments?.parent;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected abstract void OnWindowAwake();
		protected abstract void OnWindowOpen();
		protected abstract void OnWindowClose();
		protected virtual void OnWindowReset() { }
		protected virtual StratusInputUILayer GetInputLayer()
		{
			StratusInputUILayer inputLayer = new StratusInputUILayer(GetType().GetNiceName());
			if (closeOnCancelInput)
			{
				inputLayer.actions.onCancel = Close;
			}
			inputLayer.onActive += this.OnInputLayerActivated;
			return inputLayer;
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			Initialize();
		}

		private void Start()
		{
			if (initiallyOpen)
			{
				TryPushInputLayer();
			}
		}

		protected override void OnDontDestroyOnLoad()
		{
			Canvas[] canvases = this.gameObject.GetComponentsInParent<Canvas>();
			Canvas parentCanvas = this.gameObject.GetComponentInParent<Canvas>();
			if (canvases.IsValid())
			{
				parentCanvas = canvases.Last();
			}
			DontDestroyOnLoad(parentCanvas != null ? parentCanvas.gameObject : this.gameObject);
		}

		private void Reset()
		{
			this.canvas = GetComponent<Canvas>();
			OnWindowReset();
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes to window-specific events
		/// </summary>
		private void Subscribe()
		{
			StratusScene.Connect<OpenEvent>(this.OnOpenEvent);
			StratusScene.Connect<CloseEvent>(this.OnCloseEvent);
			StratusScene.Connect<ToggleEvent>(this.OnToggleEvent);
		}

		private void OnOpenEvent(OpenEvent e) => this.OnOpen(e.onFinished);
		private void OnCloseEvent(CloseEvent e) => this.OnClose(e.args);
		private void OnToggleEvent(ToggleEvent e) => this.OnToggle(e.onFinished);

		private void OnInputLayerActivated(bool activated)
		{
			if (activated)
			{
				Select();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Requests this Window to open.
		/// </summary>
		public void Open(Action onFinished)
		{
			OnOpen(onFinished);
		}

		public void Open() => Open(null);

		public void Open(StratusCanvasWindowOpenArguments args, Action onFinished = null)
		{
			this.openingArguments = args;
			OnOpen(onFinished);
		}

		/// <summary>
		/// Requests this window and all successive parents be closed
		/// </summary>
		public void Close(StratusCanvasWindowCloseArguments args)
		{
			OnClose(args);
		}

		/// <summary>
		/// Requests this Window to close.
		/// </summary>
		public void Close() => Close(null);

		/// <summary>
		/// Closes this window and all its parents
		/// </summary>
		public void CloseRecursive() => Close(new StratusCanvasWindowCloseArguments()
		{
			recursive = true
		});

		/// <summary>
		/// Requests this window to open if closed, close if open
		/// </summary>
		public void Toggle(Action onFinished = null)
		{
			OnToggle(onFinished);
		}

		/// <summary>
		/// Requests this window to be opened/closed
		/// </summary>
		/// <param name="value"></param>
		public void Toggle(bool open, Action onFinished = null)
		{
			if (open)
			{
				this.Open(onFinished);
			}
			else
			{
				this.Close(new StratusCanvasWindowCloseArguments(onFinished));
			}
		}

		/// <summary>
		/// Selects this window, rather its first selectable
		/// </summary>
		public virtual void Select()
		{
			if (selectableOnOpen != null && open)
			{
				selectableOnOpen.OnSelect(null);
			}
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public static void OpenWindow() => instance.Open();

		public static void OpenWindow(IStratusCanvasWindow parent, Action onClosed = null)
		{
			OpenWindow(new StratusCanvasWindowOpenArguments(parent, true)
			{
				onClosed = onClosed
			});
		}

		public static void OpenWindow(StratusCanvasWindowOpenArguments args)
		{
			instance.Open(args, null);
		}

		public static void ClearSelectedObject()
		{
			eventSystem.SetSelectedGameObject(null);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void Initialize()
		{
			if (canvas == null)
			{
				this.LogError("No canvas has been set");
			}
			initialized = false;
			if (_selectableOnOpen != null)
			{
				selectableOnOpen = _selectableOnOpen.GetComponent<ISelectHandler>();
				if (selectableOnOpen == null)
				{
					this.LogError($"No selectable on {_selectableOnOpen}");
				}
			}
			this.inputLayer = GetInputLayer();
			this.Subscribe();
			this.OnWindowAwake();
			this.open = this.initiallyOpen;
			this.OnToggleTransition(initiallyOpen, () =>
			{
				initialized = true;
			});
		}

		private void OnOpen(Action onFinished = null)
		{
			if (this.open)
			{
				this.LogWarning("Window already open");
				return;
			}

			this.Log("Opening");

			switch (this.transition)
			{
				case StratusCanvasWindowTransition.Toggle:
					OnToggleTransition(true, onFinished);
					break;
				case StratusCanvasWindowTransition.Fade:
					OnCrossFadeTransition(true, onFinished);
					break;
				case StratusCanvasWindowTransition.Custom:
					break;
				case StratusCanvasWindowTransition.None:
					OnNoTransition(false, onFinished);
					break;
			}
		}

		private void OnClose(StratusCanvasWindowCloseArguments args)
		{
			if (!this.open)
			{
				this.LogWarning("Window already closed");
				return;
			}

			this.Log("Closing");
			closingArguments = args;

			switch (this.transition)
			{
				case StratusCanvasWindowTransition.Toggle:
					OnToggleTransition(false, args?.onFinished);
					break;
				case StratusCanvasWindowTransition.Fade:
					OnCrossFadeTransition(false, args?.onFinished);
					break;
				case StratusCanvasWindowTransition.Custom:
					break;
				case StratusCanvasWindowTransition.None:
					OnNoTransition(false, args?.onFinished);
					break;
			}
		}

		private void OnToggle(Action onFinished = null)
		{
			if (open)
			{
				Close(new StratusCanvasWindowCloseArguments(onFinished));
			}
			else
			{
				Open(onFinished);
			}
		}

		//------------------------------------------------------------------------/
		// Procedures: Transitions
		//------------------------------------------------------------------------/
		private void OnCrossFadeTransition(bool open, Action onFinished)
		{
			// Open
			if (open)
			{
				this.canvasGroup.CrossFade(false);
				this.canvas.enabled = true;
				this.transitionRoutine = this.StartCoroutine(this.canvasGroup.ComposeCrossFade(1f, true, true, transitionSpeed,
					() =>
					{
						this.OnTransition(true, onFinished);
					}));
			}
			// Close
			else
			{
				this.transitionRoutine = this.StartCoroutine(this.canvasGroup.ComposeCrossFade(0f, false, false, transitionSpeed,
				() =>
				{
					this.canvas.enabled = false;
					this.OnTransition(false, onFinished);
				}));
			}
		}

		private void OnToggleTransition(bool open, Action onFinished)
		{
			this.canvas.enabled = open;
			OnTransition(open, onFinished);
		}

		private void OnNoTransition(bool open, Action onFinished)
		{
			Invoke(() => OnTransition(open, onFinished), _transitionSpeed);
		}

		private void OnTransition(bool open, Action onFinished)
		{
			void invokeOnWindowOpen()
			{
				OnWindowOpen();
				onFinished?.Invoke();
				TryPushInputLayer();
				this.open = true;
				Select();
			}

			// Opening
			if (open)
			{
				if (openingArguments != null)
				{
					if (openingArguments.parent != null
						&& openingArguments.visibleOverParent)
					{
						previousSortingOrder = sortingOrder;
						previousOverrideSorting = canvas.overrideSorting;

						sortingOrder = openingArguments.parent.sortingOrder + 1;
						canvas.overrideSorting = true;
						this.Log($"Overriding sorting for child of {parentWindow}");
					}

					inputLayer.ignoreBlocking = openingArguments.ignoreInpuLayerBlocking;
					openingArguments.onOpened?.Invoke();
				}

				if (initialized)
				{
					invokeOnWindowOpen();
				}
				else
				{
					Invoke(invokeOnWindowOpen, initializationDelay);
				}

			}

			// Closing
			else
			{
				if (inputLayer.pushed)
				{
					inputLayer.PopByEvent();
				}

				if (initialized)
				{
					OnWindowClose();
				}

				if (openingArguments != null)
				{
					if (openingArguments.parent != null)
					{
						if (closingArguments != null && closingArguments.recursive)
						{
							openingArguments.parent.Close();
						}
						else
						{
							openingArguments.parent.Select();
						}
					}

					if (previousSortingOrder != null)
					{
						sortingOrder = previousSortingOrder.Value;
						canvas.overrideSorting = previousOverrideSorting.Value;
						previousSortingOrder = null;
						previousOverrideSorting = null;
					}

					openingArguments.onClosed?.Invoke();
					openingArguments = null;
				}

				closingArguments = null;
				this.open = false;
				onFinished?.Invoke();
			}

		}

		private void TryPushInputLayer()
		{
			if (_pushInputLayer)
			{
				inputLayer.PushByEvent();
			}

		}


	}
}