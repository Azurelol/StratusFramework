/******************************************************************************/
/*!
@file   ActionTransform.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionPropertyVector 
  */
  /**************************************************************************/
  public abstract class ActionPropertyVector<T> : ActionProperty
  {
    //Transform Property;
    protected T Difference;
    protected T InitialValue;
    protected T EndValue;

    public ActionPropertyVector(T value, float duration, Ease ease)
      : base(duration, ease)
    {
      EndValue = value;
      Duration = duration;
      EaseType = ease;
    }

    public override float Interpolate(float dt)
    {
      this.Elapsed += dt;
      var timeLeft = this.Duration - this.Elapsed;      

      // If done updating
      if (timeLeft <= dt)
      {
        this.Finished = true;
        this.SetLast();
        return dt;
      }

      var easeVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
      this.SetCurrent(easeVal);
      return timeLeft;
      //this.Set(this.InitialValue + this.Difference * easeVal);

    }

    // To set the value every interpolation tick
    public abstract void SetCurrent(float easeVal);
    public abstract void SetLast();

  }

  /**************************************************************************/
  /*!
  @class ActionPropertyTransform
  */
  /**************************************************************************/
  public abstract class ActionPropertyTransform : ActionPropertyVector<Vector3>
  {
    protected Transform Property;
    
    public ActionPropertyTransform(Transform property, Vector3 value, float duration, Ease ease)
      : base(value, duration, ease)
    {
      Property = property;
    }

    public override abstract void SetCurrent(float easeVal);
    public override abstract void SetLast();
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyTransformPosition
  */
  /**************************************************************************/
  public class ActionPropertyTransformPosition : ActionPropertyTransform
  {
    public ActionPropertyTransformPosition(Transform property, Vector3 value, float duration, Ease ease)
      : base(property, value, duration, ease)
    {
      this.InitialValue = property.position;
      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      this.Property.position = this.InitialValue + this.Difference * easeVal;
      //Debug.Log("Current = '" + this.Property.position + "'");
    }

    public override void SetLast()
    {
      this.Property.position = this.EndValue;
    }

  }

  /**************************************************************************/
  /*!
  @class ActionPropertyTransformRotation
  */
  /**************************************************************************/
  public class ActionPropertyTransformRotation : ActionPropertyTransform
  {
    public ActionPropertyTransformRotation(Transform property, Vector3 value, float duration, Ease ease)
      : base(property, value, duration, ease)
    {
      this.InitialValue = property.rotation.eulerAngles;
      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      this.Property.rotation = Quaternion.Euler(this.InitialValue + this.Difference * easeVal);
     //Debug.Log("Current = '" + this.Property.rotation + "'");
    }

    public override void SetLast()
    {
      Trace.Script("Done!");
      this.Property.rotation = Quaternion.Euler(this.EndValue);
    }

  }

  /**************************************************************************/
  /*!
  @class ActionPropertyTransformScale
  */
  /**************************************************************************/
  public class ActionPropertyTransformScale : ActionPropertyTransform
  {
    public ActionPropertyTransformScale(Transform property, Vector3 value, float duration, Ease ease)
      : base(property, value, duration, ease)
    {
      this.InitialValue = property.localScale;
      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      this.Property.localScale = this.InitialValue + this.Difference * easeVal;
    }

    public override void SetLast()
    {
      this.Property.localScale = this.EndValue;
    }

  }




}