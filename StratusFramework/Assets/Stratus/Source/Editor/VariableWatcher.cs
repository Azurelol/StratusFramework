/******************************************************************************/
/*!
@file   VariableWatcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  public class VariableWatcher : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The left panel
    /// </summary>
    Rect LeftPanel;

    /// <summary>
    /// The right panel
    /// </summary>
    Rect RightPanel;

    //------------------------------------------------------------------------/
    // Menu
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Tools/Variable Watcher")]
    public static void Open()
    {

    }
  }
}

#endif