/******************************************************************************/
/*!
@file   ProportionalHealthModificationAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class ProportionalHealthModificationAttribute 
  */
  /**************************************************************************/
  public abstract class ProportionalHealthModificationAttribute : EffectAttribute
  {
    [Range(0, 100)] public int Percentage = 100;

    public override void OnInspect()
    {
      this.Percentage = EditorBridge.Field("Percentage", this.Percentage);
    }

  }

}