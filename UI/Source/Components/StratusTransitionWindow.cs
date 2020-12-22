using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Stratus.UI
{
	/// <summary>
	/// Provides various visual effects onto a screen-space UI overlay
	/// </summary>
	[StratusSingleton(instantiate = false, persistent = false)]
	public class StratusTransitionWindow : StratusCanvasWindow<StratusTransitionWindow>
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public abstract class BaseTransitionEvent : StratusEvent
		{
			/// <summary>
			/// How long to transition in and out
			/// </summary>
			public float speed = 0.5f;
			/// <summary>
			/// Action to invoke once the transition has started
			/// </summary>
			public Action onStarted;
			/// <summary>
			/// Action to invoke once the transition has ended
			/// </summary>
			public Action onEnded;
			/// <summary>
			/// When the transition starts, clears any selection on the event system
			/// </summary>
			public bool clearSelection = true;
			/// <summary>
			/// If the current transition blocks subsequent ones
			/// </summary>
			public abstract bool blocking { get; }
		}

		private class TransitionAction
		{
			public BaseTransitionEvent e;
			public Action start;
			public Action end;

			public bool started { get; set; }
			public bool canContinue => started && !e.blocking;

			public TransitionAction(BaseTransitionEvent e, Action start, Action end)
			{
				this.e = e;
				this.start = start;
				this.end = end;
			}
			public override string ToString()
			{
				return e.GetType().GetNiceName();
			}
		}

		/// <summary>
		/// A simple crossfade to black and back
		/// </summary>
		[Serializable]
		public class FadeEvent : BaseTransitionEvent
		{
			/// <summary>
			/// If set, how long the fade will hold. At the end of the duration
			/// the fade will end.
			/// </summary>
			public float duration = 0f;
			/// <summary>
			/// Whether to end the fade if there's an action provided
			/// </summary>
			public bool endOnAction;

			public override bool blocking => false;
		}

		/// <summary>
		/// Base class for modules managing transitions
		/// </summary>
		public abstract class Module : StratusCanvasGroup
		{
			public abstract void Select();
		}

		/// <summary>
		/// Presents a configurable loading screen
		/// </summary>
		[Serializable]
		public class LoadingScreenEvent : BaseTransitionEvent
		{
			/// <summary>
			/// Optional text for the loading screen
			/// </summary>
			public string title;
			/// <summary>
			/// Optional text for the loading screen
			/// </summary>
			public string description;
			/// <summary>
			/// Optional image for the loading screen
			/// </summary>
			public Sprite image;

			/// <summary>
			/// Whether to display a progress bar for this loading screen
			/// </summary>
			public bool progress = true;
			/// <summary>
			/// If no callback is provided, the set duration that the loading "will take"
			/// </summary>
			public float progressDuration = 1f;
			/// <summary>
			/// A progress callback that returns a percentage value (between 0 and 1)
			/// </summary>
			public Func<float> progressFunction;
			/// <summary>
			/// Whether to prompt the user to continue after loading has "finished"
			/// </summary>
			public bool promptContinue = true;

			public override bool blocking => true;

			public LoadingScreenEvent(Action onStarted, Action onEnded)
			{
				this.onStarted = onStarted;
				this.onEnded = onEnded;
			}
		}

		/// <summary>
		/// Presents a series of options during the transition
		/// </summary>
		[Serializable]
		public class MenuOptionsEvent : BaseTransitionEvent
		{
			/// <summary>
			/// The options to provide to the user
			/// </summary>
			public StratusLabeledAction[] options => _options.ToArray();
			private List<StratusLabeledAction> _options = new List<StratusLabeledAction>();
			/// <summary>
			/// Whether to append a continue option to this menu (which ends the transition)
			/// </summary>
			public bool appendContinueOption = true;
			/// <summary>
			/// If appending a continue option, its name
			/// </summary>
			public string continueLabel = "Continue";
			public override bool blocking => true;

			public void AddOption(string label, Action action)
			{
				_options.Add(new StratusLabeledAction(label, action));
			}

		}

		/// <summary>
		/// Components for loading screen
		/// </summary>
		[Serializable]
		public class LoadingScreenModule : Module
		{
			public Image imageComponent;
			public TextMeshProUGUI titleTextComponent;
			public TextMeshProUGUI descriptionTextComponent;
			public StratusProgressBar progressBar;
			public CanvasGroup continuePromptDisplay;

			public void Set(LoadingScreenEvent e)
			{
				imageComponent.sprite = e.image;
				imageComponent.enabled = imageComponent.sprite != null;

				titleTextComponent.text = e.title;
				titleTextComponent.enabled = titleTextComponent.text.IsValid();

				descriptionTextComponent.text = e.description;
				descriptionTextComponent.enabled = descriptionTextComponent.text.IsValid();
			}

			public void Reset()
			{
				imageComponent.sprite = null;
				titleTextComponent.text = descriptionTextComponent.text = string.Empty;
				titleTextComponent.enabled = descriptionTextComponent.enabled = imageComponent.enabled = false;

				progressBar?.Toggle(false);
				continuePromptDisplay?.CrossFade(false);
			}

			public override void Select()
			{
			}
		}

		/// <summary>
		/// Sets menu options
		/// </summary>
		[Serializable]
		public class MenuOptionsModule : Module
		{
			public StratusLayoutTextElementGroupBehaviour menu;

			public void Reset()
			{
				this.menu.Reset();
			}

			public void Set(MenuOptionsEvent e, StratusTransitionWindow screen)
			{
				List<StratusLayoutTextElementEntry> entries = new List<StratusLayoutTextElementEntry>();
				foreach (var option in e.options)
				{
					entries.Add(new StratusLayoutTextElementEntry(option));
				}
				if (e.appendContinueOption)
				{
					entries.Add(new StratusLayoutTextElementEntry(e.continueLabel, screen.Continue));
				}
				menu.Set(entries);
				Toggle(true);
			}

			public override void Select()
			{
				menu.Select();
			}
		}


		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField] private LoadingScreenModule loadingScreen = null;
		[SerializeField] private MenuOptionsModule menuOptions = null;

		//[SerializeField] private bool fadeIn = true;
		[SerializeField] private float fadeInDelay = 1f;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		private TransitionAction currentTransition { get; set; }
		private Queue<TransitionAction> transitions { get; set; }
		private bool continueInputEnabled { get; set; }
		private Action transitionSelectAction { get; set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
			transitions = new Queue<TransitionAction>();
			HideLoadingScreen();
			HideOptions();

			StratusScene.Connect<FadeEvent>(this.OnCrossFadeEvent);
			StratusScene.Connect<LoadingScreenEvent>(this.OnLoadingScreenEvent);
			StratusScene.Connect<MenuOptionsEvent>(this.OnMenuOptionsEvent);
		}

		protected override StratusInputUILayer GetInputLayer()
		{
			var layer = base.GetInputLayer();
			layer.actions.onSubmit = () =>
			{
				if (continueInputEnabled)
				{
					Continue();
				}
			};
			layer.actions.onCancel = null;
			layer.blocking = true;
			return layer;
		}

		protected override void OnWindowOpen()
		{
			if (initialized)
			{
				ProcessTransition();
			}
			else
			{
				Invoke(Close, fadeInDelay);
			}
		}

		protected override void OnWindowClose()
		{
		}

		public override void Select()
		{
			if (transitionSelectAction != null)
			{
				transitionSelectAction();
			}
		}

		//------------------------------------------------------------------------/
		// Window Management
		//------------------------------------------------------------------------/
		private bool ProcessTransition()
		{
			this.Log("Processing next transition");

			if (currentTransition != null)
			{
				this.Log($"Ending current transition: {currentTransition} ");
				currentTransition.end?.Invoke();
				currentTransition.e.onEnded?.Invoke();
				currentTransition = null;
				continueInputEnabled = false;
				transitionSelectAction = null;
			}

			if (transitions.Empty())
			{
				this.Log("No more transitions left...");
				return false;
			}


			currentTransition = transitions.Dequeue();
			this.Log($"Current transition now: {currentTransition}");
			currentTransition.start?.Invoke();
			currentTransition.e.onStarted?.Invoke();
			currentTransition.started = true;

			return true;
		}

		public void Continue()
		{
			this.Log("Continue...");
			if (!ProcessTransition())
			{
				Close();
			}
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void ProcessTransitionEvent(BaseTransitionEvent e, Action startAction, Action endAction)
		{
			this.Log($"Received transition {e.GetType().GetNiceName()}. Window open ? {open}");
			TransitionAction action = new TransitionAction(e, startAction, endAction);
			transitions.Enqueue(action);

			// If not opened yet
			if (!open)
			{
				if (e.clearSelection)
				{
					ClearSelectedObject();
				}

				OpenWindow(new StratusCanvasWindowOpenArguments()
				{
					transitionSpeed = e.speed
				});
			}
			// If already open
			else
			{
				if (currentTransition.canContinue)
				{
					this.Log("Moving onto next transition");
					Continue();
				}
			}

		}

		private void OnCrossFadeEvent(FadeEvent e)
		{
			ProcessTransitionEvent(e, () => SetFade(e), null);
		}

		private void OnLoadingScreenEvent(LoadingScreenEvent e)
		{
			ProcessTransitionEvent(e, () => SetLoadingScreen(e), HideLoadingScreen);
		}

		private void OnMenuOptionsEvent(MenuOptionsEvent e)
		{
			ProcessTransitionEvent(e, () => PresentMenuOptions(e), HideOptions);
		}

		//------------------------------------------------------------------------/
		// Procedures: Cross Fade
		//------------------------------------------------------------------------/
		private void SetFade(FadeEvent e)
		{
			continueInputEnabled = false;
			if (e.duration > 0.0f)
			{
				Invoke(Continue, e.duration);
			}
			else if (e.endOnAction)
			{
				this.Log("Continuing next frame (if there's no transitions queued up) ...");
				InvokeNextFrame(() =>
				{
					if (transitions.Empty())
					{
						Continue();
					}
				});
			}
			else
			{
				this.Log("Indefinite fade set...");
			}
		}

		//------------------------------------------------------------------------/
		// Procedures: Loading Screen
		//------------------------------------------------------------------------/
		private void SetLoadingScreen(LoadingScreenEvent e)
		{
			IEnumerator loadingRoutine()
			{
				// Wait a frame
				yield return new WaitForEndOfFrame();

				// Assign assets
				loadingScreen.Set(e);
				loadingScreen.Show();

				yield return new WaitForSeconds(e.speed);

				void onContinue()
				{
					// If a prompt is needed to continue past the screen, set it
					if (e.promptContinue)
					{
						continueInputEnabled = true;
						loadingScreen.continuePromptDisplay.CrossFade(true);
					}
					else
					{
						Continue();
					}
				}

				// If progress is required...
				if (e.progress && loadingScreen.progressBar != null)
				{
					loadingScreen.progressBar.ResetProgress();
					loadingScreen.progressBar.Toggle(true);

					// If a progress callback is provided, keep calling it to update the current progress
					if (e.progressFunction != null)
					{

					}
					// If no progress callback is provided, artificially set the progress by min value
					else
					{
						loadingScreen.progressBar.UpdateProgress(1f, e.progressDuration, onContinue);
					}
				}
				else
				{
					onContinue();
				}

			}

			this.StartCoroutine(loadingRoutine(), "Loading Screen");
		}

		private void HideLoadingScreen()
		{
			this.loadingScreen.Reset();
			this.loadingScreen.Hide();
		}

		//------------------------------------------------------------------------/
		// Procedures: Menu Options
		//------------------------------------------------------------------------/
		private void PresentMenuOptions(MenuOptionsEvent e)
		{
			IEnumerator optionsStarted()
			{
				yield return new WaitForEndOfFrame();
				transitionSelectAction = menuOptions.Select;
				continueInputEnabled = false;
				menuOptions.Set(e, this);
				menuOptions.menu.Select();
			}

			this.StartCoroutine(optionsStarted(), "Present Options", null);
		}

		private void HideOptions()
		{
			menuOptions.Toggle(false);
		}

		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/
		public static void FadeOut(float speed, float duration = 0f, Action onStarted = null, Action onEnded = null)
		{
			StratusScene.Dispatch<FadeEvent>(new FadeEvent()
			{
				speed = speed,
				duration = duration,
				onStarted = onStarted,
				onEnded = onEnded
			});
		}

		public static void FadeAction(float speed, Action action, bool endOnAction = true)
		{
			StratusScene.Dispatch<FadeEvent>(new FadeEvent()
			{
				speed = speed,
				onStarted = action,
				endOnAction = endOnAction
			});
		}

		public static void LoadingScreen(string title, string description, Sprite image, Action onStarted, Action onEnded)
		{
			StratusTransitionWindow.LoadingScreenEvent transition =	new StratusTransitionWindow.LoadingScreenEvent(onStarted, onEnded);
			transition.title = title;
			transition.description = description;
			transition.image = image;
			transition.DispatchToScene();
		}

	}
}