using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System;

namespace Stratus.UI
{
	public abstract class StratusDataInspectorWindow<T, DataType> : StratusCanvasWindow<T>, IStratusInputLayerProvider
		where T : StratusCanvasWindow<T>
		where DataType : class, new()
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private StratusRuntimeInspector runtimeInspector;
		[SerializeField]
		private StratusLayoutTextElementGroupBehaviour dataCategoryLayout;
		[SerializeField]
		protected StratusLayoutTextElementGroupBehaviour buttonLayout;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public abstract IStratusPrefs<DataType> prefs { get; }
		protected StratusLabeledObject[] dataCategories { get; private set; }
		public bool hasDataCategories => dataCategories != null;
		protected StratusArrayNavigator<StratusLabeledObject> dataCategoryNavigator { get; set; }
		private int lastDataCategoryIndex { get; set; }

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected virtual StratusLabeledObject[] GetDataCategories(DataType data) => null;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
		}

		private void Start()
		{
		}

		protected override void OnWindowOpen()
		{
			if (!buttonLayout.hasElements)
			{
				GenerateButtons(this.buttonLayout);
			}
			Inspect();
		}

		protected override void OnWindowClose()
		{
			lastDataCategoryIndex = hasDataCategories ? dataCategoryNavigator.currentIndex : 0;
		}

		protected override StratusInputUILayer GetInputLayer()
		{
			var inputLayer = base.GetInputLayer();
			inputLayer.actions.onSubmit = ApplyData;
			inputLayer.actions.onNavigate = RedirectNavigationToControl;
			inputLayer.actions.onNext = NextDataCategory;
			inputLayer.actions.onPrevious = PreviousDataCategory;
			return inputLayer;
		}

		//------------------------------------------------------------------------/
		// Methods: Inspect
		//------------------------------------------------------------------------/
		public void Inspect()
		{
			GenerateCategories();
			InspectCategory(dataCategoryNavigator.current);
		}

		public void NextDataCategory()
		{
			dataCategoryNavigator.Next();
		}

		public void PreviousDataCategory()
		{
			dataCategoryNavigator.Previous();
		}

		//------------------------------------------------------------------------/
		// Methods: Data Management
		//------------------------------------------------------------------------/
		public void ApplyData()
		{
			this.Log("Applying data changes...");
			prefs.Save();
		}

		public void ResetData()
		{
			this.Log("Resetting data to default...");
			prefs.Reset();
			Inspect();
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void OnDataCategoryChanged(StratusLabeledObject arg1, int arg2)
		{
			this.Log($"Category changed to {arg1.label}");
			InspectCategory(arg1);
		}

		private void InspectCategory(StratusLabeledObject category)
		{
			this.Log($"Now inspecting {category.label}");
			dataCategoryLayout.elements[dataCategoryNavigator.previousIndex].Highlight(false);			
			dataCategoryLayout.elements[dataCategoryNavigator.currentIndex].Highlight(true);
			runtimeInspector.Inspect(category.target);
			runtimeInspector.Select();
		}

		protected virtual void GenerateButtons(StratusLayoutTextElementGroupBehaviour buttonLayout)
		{
			buttonLayout.Set(
				GenerateApplyButton(),
				GenerateResetButton(),
				GenerateBackButton());
		}

		protected virtual StratusLayoutTextElementEntry GenerateApplyButton() => new StratusLayoutTextElementEntry("Apply", ApplyData);
		protected virtual StratusLayoutTextElementEntry GenerateResetButton() => new StratusLayoutTextElementEntry("Reset to Defaults", ResetData);
		protected virtual StratusLayoutTextElementEntry GenerateBackButton() => new StratusLayoutTextElementEntry("Back", Close);

		private void GenerateCategories()
		{
			dataCategories = GetDataCategories(prefs.data);
			if (dataCategories == null)
			{
				dataCategories = new StratusLabeledObject[] { new StratusLabeledObject(prefs.dataTypeName, prefs.data) };
			}
			dataCategoryNavigator = new StratusArrayNavigator<StratusLabeledObject>(dataCategories, lastDataCategoryIndex);
			dataCategoryNavigator.onIndexChanged += this.OnDataCategoryChanged;

			List<StratusLayoutTextElementEntry> entries = new List<StratusLayoutTextElementEntry>();
			for (int i = 0; i < dataCategories.Length; i++)
			{
				StratusLabeledObject category = dataCategories[i];
				entries.Add(new StratusLayoutTextElementEntry(category.label, () =>
					{
						dataCategoryNavigator.NavigateToElement(category);
					}
					));
			}
			dataCategoryLayout.Set(entries);
		}

		private void RedirectNavigationToControl(Vector2 dir)
		{
			runtimeInspector.SendInputToDrawer(dir);
		}
	}

}