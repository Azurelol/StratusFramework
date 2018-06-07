using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoManager : MonoBehaviour {

	[Header("ANIMATORS")]
	public Animator canvasAnimator;

	[Header("PANELS")]
	public CustomDropdown PanelDropdownSelector;
	public List<GameObject> panels = new List<GameObject>();
	public GameObject currentPanel;
	private CanvasGroup canvasGroup;

	[Header("ANIMATION SETTINGS")]
	private bool fadeOut = false;
	private bool fadeIn = false;
	[Range(0, 10)]public float fadeFactor = 8f;

	void Start()
	{
		PanelDropdownSelector.ClearOptions();
		foreach (var panel in panels)
			PanelDropdownSelector.AddOption(panel.name);
		PanelDropdownSelector.OnSelectedOptionChanged.AddListener(OnPanelChange);
	}

	void OnPanelChange()
	{
		var panelSelected = PanelDropdownSelector.GetSelectedOption();
		var panel = panels.Find(p => p.name == panelSelected);
		StartCoroutine("ChangePage", panel);
	}

	void Update ()
	{
		if (fadeOut)
			canvasGroup.alpha -= fadeFactor * Time.deltaTime;
		if (fadeIn) 
		{
			canvasGroup.alpha += fadeFactor * Time.deltaTime;
		}
	}

	public void ChangePanel (GameObject newPanel) 
	{
	//	if (newPage != currentPanelIndex)
		//   StartCoroutine ("ChangePage", newPage);
	}

	public IEnumerator ChangePage (GameObject newPanel)
	{
		canvasGroup = currentPanel.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1f;
		fadeIn = false;
		fadeOut = true;

		while(canvasGroup.alpha > 0)
		{
			yield return 0;
		}
		currentPanel.SetActive(false);

		fadeIn = true;
		fadeOut = false;
		currentPanel = newPanel;
		currentPanel.SetActive (true);
		canvasGroup = currentPanel.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;

		while (canvasGroup.alpha <1f)
		{
			yield return 0;
		}

		canvasGroup.alpha = 1f;
		fadeIn = false;

		yield return 0;
	}
}
