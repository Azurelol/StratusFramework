/******************************************************************************/
/*!
@file   StartUp.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// Default start-up for the Stratus Framework. Loads editor settings.
  /// </summary>
  [InitializeOnLoad]
  public class StartUp
  {              
    static StartUp()
    {
      Trace.Reset();
      LoadPreferences();
    }    

    static void LoadPreferences()
    {
      PreferencesWindow.Load();
    }    

  }
}
