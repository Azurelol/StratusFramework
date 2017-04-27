/******************************************************************************/
/*!
@file   ButtonDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

#if UNITY_EDITOR
using UnityEditor;

namespace MyNamespace
{
  namespace Stratus
  {

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : DecoratorDrawer
    {
      public override void OnGUI(Rect position)
      {
        var buttonAttribute = (attribute as ButtonAttribute);

        // check if the button is supposed to be enabled right now
        if (EditorApplication.isPlaying && !buttonAttribute.isActiveAtRuntime)
          GUI.enabled = false;
        if (!EditorApplication.isPlaying && !buttonAttribute.isActiveInEditor)
          GUI.enabled = false;

        // figure out where were drawing the button
        var pos = new Rect(position.x, position.y, position.width, position.height - EditorGUIUtility.standardVerticalSpacing);
        // draw it and if its clicked...
        if (GUI.Button(pos, buttonAttribute.ButtonLabel))
        {
          // tell the current game object to find and run the method we asked for!
          Selection.activeGameObject.BroadcastMessage(buttonAttribute.MethodName);
        }

        // make sure the GUI is enabled when were done!
        GUI.enabled = true;
      }

      public override float GetHeight()
      {
        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
      }
    }
  }

} 
#endif