﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationExample : MonoBehaviour {

	[Header("OBJECT")]
	public GameObject notificationObject;
	public Animator notificationAnimator;

	[Header("OBJECT")]
	public Text titleObject;
	public Text descriptionObject;

	[Header("VARIABLES")]
	public string titleText;
	public string descriptionText;
	public string animationNameIn;
	public string animationNameOut;

	private bool isPlayed = false;

	void Start()
	{
		notificationObject.SetActive (false);
	}

	public void ShowNotification () 
	{
		notificationObject.SetActive (true);
		titleObject.text = titleText;
		descriptionObject.text = descriptionText;

		notificationAnimator.Play (animationNameIn);
		notificationAnimator.Play (animationNameOut);
	
	}
}
