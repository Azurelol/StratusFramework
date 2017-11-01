/******************************************************************************/
/*!
@file   ObjectTransformDispatcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ObjectTransformDispatcher 
  */
  /**************************************************************************/
  public class ObjectTransformEvent : Triggerable
  { 
    public enum TransformType { Translate, Rotate, Scale }    
    public TransformType Type;
    public float Duration = 1.0f;
    public Ease Ease;
    public Vector3 Value;
    [Tooltip("Whether the value is an offset from the current value")]
    public bool Offset = false;
    Vector3 PreviousValue;    

    /**************************************************************************/
    /*!
    @brief  Initializes the ObjectTransformDispatcher.
    */
    /**************************************************************************/
    protected override void OnAwake()
    {
      
    }

    protected override void OnTrigger()
    {
      this.Transform(this.Value);
    }

    /// <summary>
    /// Interpolates to the specified transformation.
    /// </summary>
    public void Transform(Vector3 value)
    {
      var seq = Actions.Sequence(this);
      switch (Type)
      {
        case TransformType.Translate:
          PreviousValue = this.transform.localPosition;
          if (this.Offset) value += PreviousValue;
          Actions.Property(seq, () => this.transform.localPosition, value, this.Duration, this.Ease);
          break;
        case TransformType.Rotate:
          PreviousValue = this.transform.rotation.eulerAngles;
          if (this.Offset) value += PreviousValue;
          Actions.Property(seq, () => this.transform.rotation.eulerAngles, value, this.Duration, this.Ease);
          break;
        case TransformType.Scale:
          PreviousValue = this.transform.localScale;
          if (this.Offset) value += PreviousValue;
          Actions.Property(seq, () => this.transform.localScale, value, this.Duration, this.Ease);
          break;
      }
    }

    /// <summary>
    /// Reverts to the previous transformation.
    /// </summary>
    public void Revert()
    {
      this.Transform(this.PreviousValue);
    }



  }

}