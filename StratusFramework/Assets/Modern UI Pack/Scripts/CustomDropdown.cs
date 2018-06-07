using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CustomDropdown : MonoBehaviour {
	[Header("ANIMATORS")]
	public Animator dropdownAnimator;

	[Header("OBJECTS")]
	public GameObject fieldTrigger;
	public Text selectedText;
	public Image selectedImage;
    public GameObject notificationPrefab;

    [Header("PLACEHOLDER")]
	public string customText;
	public Sprite customIcon;

	[Header("SETTINGS")]
	[Tooltip("IMPORTANT! EVERY DORPDOWN MUST HAVE A DIFFERENT ID")]
	public int DropdownID = 0;
    public HeightSize heightSize;
	public bool customPlaceholder;
	public bool rememberSelection = true;
    public bool enableIcon = true; // WIP
    private bool enableNotification = false;

    //public bool darkTrigger = true;

    private bool isOn;
	private string sText;
	private string sImage;

    public enum HeightSize
    {
        SMALL,
        MEDIUM,
        BIG
    }
	
	public GameObject OptionTemplate;
	private class Option {
		public String Name;
		public Sprite Icon;
		public GameObject Go;
	}
	public UnityEvent OnSelectedOptionChanged;
	private Option SelectedOption = null;
	private List<Option> _Options = new List<Option>();


    void Awake()
	{
		if (rememberSelection == true)
		{
			sText = PlayerPrefs.GetString (DropdownID + "SelectedText");
			sImage = PlayerPrefs.GetString (DropdownID + "SelectedImage");
		}

		if (customPlaceholder == true)
		{
			selectedText.text = customText;
			selectedImage.sprite = customIcon;
		}

		else 
		{
			selectedText.text = sText;
			//	selectedImage.sprite = 
		}

        if (enableIcon == false)
        {
            selectedImage.enabled = false;
        }

		InsertOptionsAlreadyPresent();
    }

	public void Animate ()
	{
		if (isOn == true) 
		{
            if (heightSize == HeightSize.SMALL)
            {
                dropdownAnimator.Play("Out Small");
            }

            else if (heightSize == HeightSize.MEDIUM)
            {
                dropdownAnimator.Play("Out Medium");
            }

            else if (heightSize == HeightSize.BIG)
            {
                dropdownAnimator.Play("Out Big");
            }

            isOn = false;
			fieldTrigger.SetActive (false);
		}

		else
		{
            if (heightSize == HeightSize.SMALL)
            {
                dropdownAnimator.Play("In Small");
            }

            else if (heightSize == HeightSize.MEDIUM)
            {
                dropdownAnimator.Play("In Medium");
            }

            else if (heightSize == HeightSize.BIG)
            {
                dropdownAnimator.Play("In Big");
            }

            isOn = true;
			fieldTrigger.SetActive (true);
		}
	}

	private void InsertOptionsAlreadyPresent()
	{
		var itemsContainer = transform.Find("Content").Find("Items");
		for (int i = 0; i < itemsContainer.childCount; i++)
		{
			var item = itemsContainer.GetChild(i);
			var dropdrownItem = item.GetComponent<DropdownItem>();
			var itemName = dropdrownItem.itemText;
			var itemIcon = dropdrownItem.itemImage;

			var itemButton = item.GetComponent<Button>();
			itemButton.onClick.AddListener(() => SelectedOptionChanged(itemName));

			var option = new Option() {
				Name = itemName,
				Icon = itemIcon,
				Go = item.gameObject
			};
			_Options.Add(option);
		}
	}

	public void SelectedOptionChanged(string name)
	{
		var option = _Options.Find(o => o.Name == name);
		if (option == null)
			return;

		SelectedOption = option;
		OnSelectedOptionChanged.Invoke();
	}

	public IEnumerable<String> GetOptions()
	{
		return _Options.Select(o => o.Name).ToList();
	}

	public String GetSelectedOption()
	{
		return SelectedOption == null ? null : SelectedOption.Name;
	}

	public void ClearOptions()
	{
		foreach (var option in _Options)
			Destroy(option.Go);

		_Options.Clear();
	}

	public void AddOption(string name, Sprite icon = null)
	{
		icon = icon == null ? customIcon : icon;

		var optionGo = GameObject.Instantiate(OptionTemplate);
		optionGo.SetActive(true);
		var rect = optionGo.transform;
		optionGo.transform.parent = transform.Find("Content").Find("Items");
		var rectTransform = optionGo.GetComponent<RectTransform>();
		rectTransform.localPosition = Vector3.zero;
		rectTransform.localScale = Vector3.one;

		var dropdrownItem = optionGo.GetComponent<DropdownItem>();
		dropdrownItem.itemText = name;
		dropdrownItem.itemImage = icon;

		var itemButton = optionGo.GetComponent<Button>();
		itemButton.onClick.AddListener(() => SelectedOptionChanged(name));

		var option = new Option() {
			Name = name,
			Icon = icon,
			Go = optionGo
		};
		_Options.Add(option);
	}

	public void RemoveOption(string name)
	{
		var option = _Options.Find(o => o.Name == name);
		if (option == null)
			return;

		Destroy(option.Go);
		_Options.Remove(option);

		selectedImage.sprite = customIcon;
		selectedText.text = customText;
	}

	public void SetSelectedOption(string name)
	{
		var option = _Options.Find(o => o.Name == name);
		if (option == null)
			return;

		SelectedOption = option;

		var previewContainer = transform.Find("Content").Find("Main");
		selectedImage.sprite = option.Icon;
		selectedText.text = option.Name;

		OnSelectedOptionChanged.Invoke();		
	}
}