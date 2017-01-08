/******************************************************************************/
/*!
@file   ActionColor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionPropertyColor 
  */
  /**************************************************************************/  
  public abstract class ActionColor : ActionPropertyVector<Vector4>
  {
    public ActionColor(Vector4 value, float duration, Ease ease)
      : base(value, duration, ease)
    {
    }

    public override abstract void SetCurrent(float easeVal);
    public override abstract void SetLast();
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyColorSprite 
  */
  /**************************************************************************/
  public class ActionPropertyColorSprite : ActionColor
  {
    SpriteRenderer Property;

    public ActionPropertyColorSprite(SpriteRenderer property, Vector4 value, float duration, Ease ease)
      : base(value, duration, ease)
    {
      this.Property = property;
      this.InitialValue = this.Property.color;
      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      this.Property.color = this.InitialValue + this.Difference * easeVal;
    }

    public override void SetLast()
    {
      this.Property.color = this.EndValue;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyColorMaterial 
  */
  /**************************************************************************/
  public class ActionPropertyColorMaterial : ActionColor
  {
    Renderer Property;

    public ActionPropertyColorMaterial(Renderer property, Vector4 value, float duration, Ease ease)
      : base(value, duration, ease)
    {
      this.Property = property;
      this.InitialValue = this.Property.material.color;
      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      this.Property.material.color = this.InitialValue + this.Difference * easeVal;
    }

    public override void SetLast()
    {
      this.Property.material.color = this.EndValue;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyColorMaskableGraphic 
  */
  /**************************************************************************/
  public class ActionPropertyColorMaskableGraphic : ActionColor
  {
    MaskableGraphic Property;

    public ActionPropertyColorMaskableGraphic(MaskableGraphic property, Vector4 value, float duration, Ease ease)
      : base(value, duration, ease)
    {
      this.Property = property;
     
      if (this.Property.material)
        this.InitialValue = this.Property.material.color;
      else
        this.InitialValue = this.Property.color;

      this.Difference = this.EndValue - this.InitialValue;
    }

    public override void SetCurrent(float easeVal)
    {
      if (this.Property.material)
        this.Property.material.color = this.InitialValue + this.Difference * easeVal;
      else
        this.Property.color = this.InitialValue + this.Difference * easeVal;
    }

    public override void SetLast()
    {
      if (this.Property.material)
        this.Property.material.color = this.EndValue;
      else
        this.Property.color = this.EndValue;
    }
  }

}