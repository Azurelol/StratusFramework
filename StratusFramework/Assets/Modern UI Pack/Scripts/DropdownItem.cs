using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownItem : MonoBehaviour {

	[Header("RESOURCES")]
	public CustomDropdown mainScript;
	public string itemText;
	public Sprite itemImage;

	[Header("OBJECTS")]
	public Text itemTextObj;
	public Image itemImageObj;

	[Header("SETTINGS")]
	public bool SetImageFromScript = false;
	public bool SetTextFromScript = true;

	void Start () 
	{
		if (SetImageFromScript == false) 
		{
			itemImage = itemImageObj.sprite;
		}

		if (SetTextFromScript == false) 
		{
			itemText = itemTextObj.text;
		}

		itemTextObj.text = itemText;
		itemImageObj.sprite = itemImage;
	}

	public void ItemClick ()
	{
		if (mainScript.rememberSelection == true) 
		{
			PlayerPrefs.SetString (mainScript.DropdownID + "SelectedText", itemText);
			PlayerPrefs.SetString (mainScript.DropdownID + "SelectedImage", itemImageObj.sprite.name);
		}

		mainScript.selectedText.text = itemText;
		mainScript.selectedImage.sprite = itemImage;

        // WORK IN PROGRESS

       // if (mainScript.enableNotification == true)
       // {
       //     GameObject go = Instantiate(mainScript.notificationPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
       //     transform.root.GetComponent<Canvas>();
       //     go.transform.SetParent(transform.root, false);
       // }

	//	Debug.Log ("Clicked succesfully. Dropdown ID: " + mainScript.DropdownID);
	}
}