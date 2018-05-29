using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(RendererEvent))] 
  public class RendererEventEditor : TriggerableEditor<RendererEvent>
  {
    RendererEvent rendererEvent => target as RendererEvent;

    protected override void OnTriggerableEditorEnable()
    {
    }

    //protected override void DrawDeclaredProperties()
    //{
    //  EditorGUILayout.BeginHorizontal();
    //  {
    //    if (GUILayout.Button("Renderer")) rendererEvent.type = RendererEvent.Type.Renderer;
    //    if (GUILayout.Button("Material")) rendererEvent.type = RendererEvent.Type.Material;
    //  }      
    //  EditorGUILayout.EndHorizontal();
    //}

  }

}