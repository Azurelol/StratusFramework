/******************************************************************************/
/*!
@file   ActionPropertyFloat.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Reflection;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionPropertyFloat 
  */
  /**************************************************************************/
  public class ActionPropertyFloat : ActionPropertyGeneric<float>
  {
    public ActionPropertyFloat(object target, PropertyInfo property, float endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyFloat(object target, FieldInfo field, float endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }
    public override float ComputeCurrentValue(float easeVal)
    {
      var currentVal = this.InitialValue + this.Difference * easeVal;
      //Stratus.Trace.Script("currentVal = " + currentVal);
      return currentVal;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyVector2 
  */
  /**************************************************************************/
  public class ActionPropertyVector2 : ActionPropertyGeneric<Vector2>
  {
    public ActionPropertyVector2(object target, PropertyInfo property, Vector2 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector2(object target, FieldInfo field, Vector2 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector2 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }

  }

  /**************************************************************************/
  /*!
  @class ActionPropertyVector3
  */
  /**************************************************************************/
  public class ActionPropertyVector3 : ActionPropertyGeneric<Vector3>
  {
    public ActionPropertyVector3(object target, PropertyInfo property, Vector3 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector3(object target, FieldInfo field, Vector3 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector3 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyVector4
  */
  /**************************************************************************/
  public class ActionPropertyVector4 : ActionPropertyGeneric<Vector4>
  {
    public ActionPropertyVector4(object target, PropertyInfo property, Vector4 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector4(object target, FieldInfo field, Vector4 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector4 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyVector4
  */
  /**************************************************************************/
  public class ActionPropertyColor : ActionPropertyGeneric<Color>
  {
    public ActionPropertyColor(object target, PropertyInfo property, Color endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyColor(object target, FieldInfo field, Color endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() {
      this.Difference = this.EndValue - this.InitialValue;      
    }

    public override Color ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyQuaternion
  */
  /**************************************************************************/
  public class ActionPropertyQuaternion : ActionPropertyGeneric<Quaternion>
  {
    public ActionPropertyQuaternion(object target, PropertyInfo property, Quaternion endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyQuaternion(object target, FieldInfo field, Quaternion endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override Quaternion ComputeCurrentValue(float easeVal) { return new Quaternion(); }
    public override void SetCurrent()
    {
      //Debug.Log("Setting!");
      Set(Quaternion.LerpUnclamped(InitialValue, EndValue, Time.time * Time.deltaTime));
    }

    public override void SetLast()
    {
      Set(EndValue);
    }
  }


  /**************************************************************************/
  /*!
  @class ActionPropertyBool
  */
  /**************************************************************************/
  public class ActionPropertyBool : ActionPropertyGeneric<bool>
  {
    public ActionPropertyBool(object target, PropertyInfo property, bool endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyBool(object target, FieldInfo field, bool endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override void SetCurrent() { }
    public override bool ComputeCurrentValue(float easeVal) { return false; }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyInt
  */
  /**************************************************************************/
  public class ActionPropertyInt : ActionPropertyGeneric<int>
  {
    float CurrentValue;

    public ActionPropertyInt(object target, PropertyInfo property, int endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyInt(object target, FieldInfo field, int endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override int ComputeCurrentValue(float easeVal)
    {
      this.CurrentValue = this.InitialValue + this.Difference * easeVal;
      return Mathf.CeilToInt(this.CurrentValue);
    }

  }
}