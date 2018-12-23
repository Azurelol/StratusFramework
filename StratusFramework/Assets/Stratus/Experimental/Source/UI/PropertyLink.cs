/******************************************************************************/
/*!
@file   PropertyLink.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// A link which allows a property to be modified through a slider
    /// </summary>
    public class PropertyLink : Link
    {
      enum PropertyType { Number, Boolean, Invalid }
      //public UnityEvent Property;
      public NavigationAxis Axis;
      public float Increment = 1.0f;
      public float Minimum = 0.0f;
      public float Maximum = 100.0f;

      Slider Slider { get { return GetComponentInChildren<Slider>(); } }
      Text ValueText { get { return transform.Find("Value").GetComponent<Text>(); } }
      //PropertyType Type = PropertyType.Invalid;

      protected override void OnActivate()
      {

      }

      protected override void OnSelect()
      {

      }

      protected override void OnDeselect()
      {

      }

      protected override void OnStart()
      {
        //ValidateType();
        //UpdateVisual();
      }

      protected override void OnNavigate(Navigation dir)
      {
        //Trace.Script("Navigating!", this);
        if (Axis == NavigationAxis.Horizontal)
        {
          if (dir == UI.Navigation.Right)
          {
            //this.ApplyIncrement();
          }
          else if (dir == UI.Navigation.Left)
          {
            //this.ApplyDecrement();
          }
        }
        else if (Axis == NavigationAxis.Vertical)
        {
          if (dir == UI.Navigation.Up)
          {
            //this.ApplyIncrement();
          }
          else if (dir == UI.Navigation.Down)
          {
            //this.ApplyDecrement();
          }
        }
      }

      protected override void OnConfirm()
      {

      }

      protected override void OnCancel()
      {
        this.Deactivate();
      }
      
      /*
      void ApplyIncrement()
      {
        if (Tracing) Trace.Script("Applying increment to '" + Property.name + "'", this);

        if (Type == PropertyType.Number)
        {
          var currentValue = Property.Get<float>();
          var newValue = currentValue + this.Increment;
          if (newValue > Maximum) newValue = Maximum;
          Property.Set(newValue);
        }
        else if (Type == PropertyType.Boolean)
        {
          Property.Set(true);
        }

        UpdateVisual();
      }

      void ApplyDecrement()
      {
        if (Tracing) Trace.Script("Applying decrement to '" + Property.name + "'", this);

        if (Type == PropertyType.Number)
        {
          var currentValue = Property.Get<float>();
          var newValue = currentValue - this.Increment;
          if (newValue < Minimum) newValue = Minimum;
          Property.Set(newValue);
        }
        else if (Type == PropertyType.Boolean)
        {
          Property.Set(false);
        }

        UpdateVisual();
      }

      void UpdateVisual()
      {
        ValueText.text = Convert.ToString(Property.Get());
      }

      void ValidateType()
      {
        var type = Property.type;
        if (type == typeof(float) || type == typeof(int))
        {
          Type = PropertyType.Number;
        }
        else if (type == typeof(bool))
        {
          Type = PropertyType.Boolean;
        }
      }
      */


    }

  } 
}