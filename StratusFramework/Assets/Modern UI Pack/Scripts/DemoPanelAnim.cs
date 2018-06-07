using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DemoPanelAnim : MonoBehaviour {

	[Header("PANEL SETTINGS")]
	public List<GameObject> panels = new List<GameObject>();
	public int currentPanelIndex = 0;
	public GameObject currentPanel;
	private CanvasGroup canvasGroup;
    public Text topTitleText;

	[Header("ANIMATION SETTINGS")]
    private bool fadeOut = false;
    private bool fadeIn = false;
    [Range(0, 10)] public float fadeFactor = 8f;
    [Range(0, 10)] public float growRate = 0.5f;

    void Update ()
	{
		if (fadeOut)
			canvasGroup.alpha -= fadeFactor * Time.deltaTime;
		if (fadeIn) 
		{
			canvasGroup.alpha += fadeFactor * Time.deltaTime;
			currentPanel.transform.localScale += Vector3.one * growRate * Time.deltaTime;
		}
	}

	public void newPanel(int newPage)
	{
		if (newPage != currentPanelIndex)
			StartCoroutine ("ChangePage", newPage);
	}

    public void changeTopTitle(string newTitle)
    {
        topTitleText.text = newTitle;
    }

	public IEnumerator ChangePage (int newPage)
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
		currentPanelIndex = newPage;
		currentPanel = panels [currentPanelIndex];
		currentPanel.SetActive (true);
		canvasGroup = currentPanel.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		currentPanel.transform.localScale = Vector3.one * 0.9f;

		while (canvasGroup.alpha < 1f || currentPanel.transform.localScale.x < 1f)
		{
			yield return 0;
		}

		canvasGroup.alpha = 1f;
		currentPanel.transform.localScale = Vector3.one;
		fadeIn = false;

		yield return 0;
	}
}
