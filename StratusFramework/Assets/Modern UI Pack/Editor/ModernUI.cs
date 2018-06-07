using System.Collections;
using UnityEditor;
using UnityEngine;

public class ModernUIEditor : EditorWindow {

	private static ModernUIEditor instance = null;

	[MenuItem("Tools/Modern UI Pack/Buttons/Basic")]
	static void CreateBasicButton()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Basic")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Basic Outline")]
	static void CreateBasicOutline()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Basic With Image")]
	static void CreateBasicWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Basic With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Basic Outline With Image")]
	static void CreateBasicOutlineWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Box Outline With Image")]
	static void CreateBoxOutlineWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Box Outline With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Box With Image")]
	static void CreateBoxWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Box With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Circle Outline With Image")]
	static void CreateCircleOutlineWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Circle Outline With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Circle With Image")]
	static void CreateCircleWithImage()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Circle With Image")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Rounded")]
	static void RoundedButton()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Buttons/Rounded Outline")]
	static void RoundedOutline()
	{
		Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Notifications/Fading Notification")]
	static void FadingNotification()
	{
		Instantiate(Resources.Load<GameObject>("Notifications/Fading Notification")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Notifications/Popup Notification")]
	static void PopupNotification()
	{
		Instantiate(Resources.Load<GameObject>("Notifications/Popup Notification")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Notifications/Slippery Notification")]
	static void SlipperyNotification()
	{
		Instantiate(Resources.Load<GameObject>("Notifications/Slippery Notification")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Notifications/Slipping Notification")]
	static void SlippingNotification()
	{
		Instantiate(Resources.Load<GameObject>("Notifications/Slipping Notification")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Bold")]
	static void RadialPBBold()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Bold")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Filled H")]
	static void RadialPBFilledH()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Filled H")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Filled V")]
	static void RadialPBFilledV()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Filled V")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Light")]
	static void RadialPBLight()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Light")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Regular")]
	static void RadialPBRegular()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Regular")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Radial PB Thin")]
	static void RadialPBThin()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Thin")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars/Standart PB")]
	static void StandartPB()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars/Standart PB")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Circle Glass")]
	static void CircleGlass()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Glass")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Circle Pie")]
	static void CirclePie()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Pie")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Circle Run")]
	static void CircleRun()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Run")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Circle Trapez")]
	static void CircleTrapez()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Trapez")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Standart Fastly")]
	static void StandartFastly()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Standart Fastly")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Standart Finish")]
	static void StandartFinish()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Standart Finish")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Progress Bars (Loop)/Standart Run")]
	static void StandartRun()
	{
		Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Standart Run")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Sliders/Gradient")]
	static void GradientSlider()
	{
		Instantiate(Resources.Load<GameObject>("Sliders/Gradient")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Sliders/Outline")]
	static void OutlineSlider()
	{
		Instantiate(Resources.Load<GameObject>("Sliders/Outline")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Sliders/Standart")]
	static void StandartSlider()
	{
		Instantiate(Resources.Load<GameObject>("Sliders/Standart")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Switches/Outline")]
	static void OutlineSwitch()
	{
		Instantiate(Resources.Load<GameObject>("Switches/Outline")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Switches/Standart")]
	static void StandartSwitch()
	{
		Instantiate(Resources.Load<GameObject>("Switches/Standart")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Toggles/Standart (Bold)")]
	static void StandartToggleBold()
	{
		Instantiate(Resources.Load<GameObject>("Toggles/Standart Toggle (Bold)")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Toggles/Standart (Light)")]
	static void StandartToggleLight()
	{
		Instantiate(Resources.Load<GameObject>("Toggles/Standart Toggle (Light)")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Toggles/Standart (Regular)")]
	static void StandartToggleRegular()
	{
		Instantiate(Resources.Load<GameObject>("Toggles/Standart Toggle (Regular)")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Tool Tips/Fading")]
	static void FadingToolTip()
	{
		Instantiate(Resources.Load<GameObject>("Tool Tips/Fading Tool Tip")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Tool Tips/Scaling")]
	static void ScalingToolTip()
	{
		Instantiate(Resources.Load<GameObject>("Tool Tips/Scaling Tool Tip")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Dropdowns/Standart")]
	static void StandartDropdown()
	{
		Instantiate(Resources.Load<GameObject>("Dropdowns/Standart Dropdown")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Dropdowns/Outline")]
	static void StandartDropdownOutline()
	{
		Instantiate(Resources.Load<GameObject>("Dropdowns/Outline Dropdown")).GetComponent<ModernUIEditor>();
	}

	[MenuItem("Tools/Modern UI Pack/Input Fields/Standart")]
	static void StandartInputField()
	{
		Instantiate(Resources.Load<GameObject>("Input Fields/Standart Input Field")).GetComponent<ModernUIEditor>();
	}

    [MenuItem("Tools/Modern UI Pack/Sliders/Radial Standart")]
    static void StandartRadialSlider()
    {
        Instantiate(Resources.Load<GameObject>("Sliders/Radial Standart")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Sliders/Radial Gradient")]
    static void GradientRadialSlider()
    {
        Instantiate(Resources.Load<GameObject>("Sliders/Radial Gradient")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Modal Windows/Only Exit Button")]
    static void OEBModal()
    {
        Instantiate(Resources.Load<GameObject>("Modal Windows/Only Exit Button")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Modal Windows/With Buttons")]
    static void WBModal()
    {
        Instantiate(Resources.Load<GameObject>("Modal Windows/With Buttons")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Buttons/Rounded Outline With Image")]
    static void ROWIButton()
    {
        Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline With Image")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Buttons/Rounded With Image")]
    static void RWIButton()
    {
        Instantiate(Resources.Load<GameObject>("Buttons/Rounded With Image")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Animated Icons/Hamburger to Exit")]
    static void AIHTE()
    {
        Instantiate(Resources.Load<GameObject>("Animated Icons/Hamburger to Exit")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Animated Icons/Heart Pop")]
    static void AIHP()
    {
        Instantiate(Resources.Load<GameObject>("Animated Icons/Heart Pop")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Animated Icons/Message Bubbles")]
    static void AIMB()
    {
        Instantiate(Resources.Load<GameObject>("Animated Icons/Message Bubbles")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Animated Icons/Switch")]
    static void AISW()
    {
        Instantiate(Resources.Load<GameObject>("Animated Icons/Switch")).GetComponent<ModernUIEditor>();
    }

    [MenuItem("Tools/Modern UI Pack/Animated Icons/Yes to No")]
    static void AIYTN()
    {
        Instantiate(Resources.Load<GameObject>("Animated Icons/Yes to No")).GetComponent<ModernUIEditor>();
    }

    public static void OnCustomWindow()
	{
		EditorWindow.GetWindow(typeof(ModernUIEditor));
	}
}
