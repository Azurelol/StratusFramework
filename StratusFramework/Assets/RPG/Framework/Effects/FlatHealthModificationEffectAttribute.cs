/******************************************************************************/
/*!
@file   FlatHealthModificationEffectAttribute.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Altostratus
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class FlatHealthModificationEffectAttribute : EffectAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public float Value = 100.0f;
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public override void OnInspect()
    {
      this.Value = EditorBridge.Field("Value", this.Value);
    }

  }

}