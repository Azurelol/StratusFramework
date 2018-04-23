using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
  public class HelpBoxAttributeDrawer : DecoratorDrawer
  {
    public override float GetHeight()
    {
      var helpBoxAttribute = attribute as HelpBoxAttribute;
      if (helpBoxAttribute == null) return base.GetHeight();
      var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
      if (helpBoxStyle == null) return base.GetHeight();
      return Mathf.Max(40f, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.message), EditorGUIUtility.currentViewWidth) + 4);
    }

    public override void OnGUI(Rect position)
    {
      var helpBoxAttribute = attribute as HelpBoxAttribute;
      if (helpBoxAttribute == null) return;
      EditorGUI.HelpBox(position, helpBoxAttribute.message, GetMessageType(helpBoxAttribute.messageType));
    }
    
    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
      switch (helpBoxMessageType)
      {
        default:
        case HelpBoxMessageType.Info: return MessageType.Info;
        case HelpBoxMessageType.Warning: return MessageType.Warning;
        case HelpBoxMessageType.Error: return MessageType.Error;
      }
    }
  }

}