//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace Stratus.UI
//{
//	[CustomEditor(typeof(StratusScreenOverlay), true)]
//	public class ScreenOverlayEditor : StratusBehaviourEditor<StratusScreenOverlay>
//	{
//		protected override void OnStratusEditorEnable()
//		{
//			AddPropertyChangeCallback(nameof(StratusScreenOverlay.hideInEditor), ShowOverlay);
//			AddPropertyChangeCallback(nameof(StratusScreenOverlay.screen), ShowOverlay);
//		}

//		private void ShowOverlay()
//		{
//			if (target.screen)
//			{
//				if (target.hideInEditor)
//					target.screen.enabled = false;
//				else
//					target.screen.enabled = true;
//			}
//		}

//	}

//}