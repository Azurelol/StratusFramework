/******************************************************************************/
/*!
@file   Configuration.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Stratus 
{
  /// <summary>
  /// Provides in-editor configuration for the Stratus Framework.
  /// </summary>
  [CreateAssetMenu(fileName = "StratusConfiguration")]
  public class Configuration : ScriptableObject 
  {
    public static bool Trace = false;
    
  }
}
