using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;

namespace Stratus
{
  [Serializable]
  public class MemberSetterField 
  {
    public enum ValueType
    {
      Static,
      Dynamic,
    }

    // Common fields
    [Filter(typeof(float), typeof(int), typeof(bool), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Color), Fields = true, Properties = true, Public = true, Gettable = false, Extension = false, Methods = false, Settable = true, Inherited = true)]
    public UnityMember member;
    [Tooltip("Over how long to interpolate this property to the specified value")]
    public float duration;
    [Tooltip("The type of easing to be used for the interpolation")]
    public Ease ease;
    [Tooltip("What type of value is being used for the field")]
    public ValueType valueType = ValueType.Static;
    

    // Different value types    
    [SerializeField]
    private int intValueStatic;
    [SerializeField] [Filter(typeof(int), Fields = true, Properties = true, Inherited = true)]
    private UnityMember intValueDynamic;

    [SerializeField]
    private float floatValueStatic;
    [SerializeField] [Filter(typeof(float), Fields = true, Properties = true, Inherited = true)]
    private UnityMember floatValueDynamic;

    [SerializeField]
    private bool boolValueStatic;
    [SerializeField] [Filter(typeof(bool), Fields = true, Properties = true, Inherited = true)]
    private UnityMember boolValueDynamic;

    [SerializeField]
    private Vector2 vector2ValueStatic;
    [SerializeField] [Filter(typeof(Vector2), Fields = true, Properties = true, Inherited = true)]
    private UnityMember vector2ValueDynamic;

    [SerializeField]
    private Vector3 vector3ValueStatic;
    [SerializeField] [Filter(typeof(Vector3), Fields = true, Properties = true, Inherited = true)]
    private UnityMember vector3ValueDynamic;

    [SerializeField]
    private Vector4 vector4ValueStatic;
    [SerializeField] [Filter(typeof(Vector4), Fields = true, Properties = true, Inherited = true)]
    private UnityMember vector4ValueDynamic;

    [SerializeField]
    private Color colorValueStatic = Color.white;
    [SerializeField] [Filter(typeof(Color), Fields = true, Properties = true, Inherited = true)]
    private UnityMember colorValueDynamic;

    [Tooltip("Whether a consecutive call will interpolate the property back to its intitial value")]
    public bool toggle;

    // The enumerated type of this property
    [SerializeField] 
    private ActionProperty.Types memberType = ActionProperty.Types.None;
    // Saved values for toggling
    private object previousValue;
    // Interpolate function used
    private IEnumerator interpolateRoutine;
    // All runtime instances for setter fields
    private static Dictionary<MemberSetterField, int> instances;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public int intValue => (valueType == ValueType.Dynamic) ? intValueDynamic.Get<int>() : intValueStatic;
    public float floatValue => (valueType == ValueType.Dynamic) ? floatValueDynamic.Get<float>() : floatValueStatic;
    public bool boolValue => (valueType == ValueType.Dynamic) ? boolValueDynamic.Get<bool>() : boolValueStatic;
    public Vector2 vector2Value => (valueType == ValueType.Dynamic) ? vector2ValueDynamic.Get<Vector2>() : vector2ValueStatic;
    public Vector3 vector3Value => (valueType == ValueType.Dynamic) ? vector3ValueDynamic.Get<Vector3>() : vector3ValueStatic;
    public Vector4 vector4Value => (valueType == ValueType.Dynamic) ? vector4ValueDynamic.Get<Vector4>() : vector4ValueStatic;
    public Color colorValue => (valueType == ValueType.Dynamic) ? colorValueDynamic.Get<Color>() : colorValueStatic;

    private static int created { get; set; } = 0;
    private int id { get; set; } = -1;
    private bool isRegistered => id > -1;
    private string identifier => $"{member.name} + {id}";

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public IEnumerator MakeInterpolateRoutine()
    {
      IEnumerator interpolator = null;
      switch (memberType)
      {
        case ActionProperty.Types.Integer:
          {
            int currentValue = member.Get<int>();
            bool shouldToggle = (toggle && currentValue == intValue);
            int nextValue = shouldToggle ? (int)previousValue : intValue;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (float val) => { property.Set(Mathf.CeilToInt(val)); }, Routines.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (float val) => { member.Set(Mathf.CeilToInt(val)); }, ease);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Float:
          {
            float currentValue = member.Get<float>();
            bool shouldToggle = (toggle && currentValue == floatValue);
            Trace.Script("Previous float " + previousValue + ", Current Float = " + currentValue + ", Float Value = " + floatValue + ", shouldToggle = " + shouldToggle);
            float nextValue = shouldToggle ? (float)previousValue : floatValue;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (float val) => { property.Set(val); }, Routines.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (float val) => { member.Set(val); }, ease);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Boolean:
          {
            bool currentValue = member.Get<bool>();
            bool shouldToggle = (toggle && currentValue == boolValue);
            bool nextValue = shouldToggle ? (bool)previousValue : boolValue;
            interpolator = Routines.Call(() => { member.Set(nextValue); }, duration);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Vector2:
          {
            Vector2 currentValue = member.Get<Vector2>();
            bool shouldToggle = (toggle && currentValue == vector2Value);
            Vector2 nextValue = shouldToggle ? (Vector2)previousValue : vector2Value;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (Vector2 val) => { property.Set(val); }, Vector2.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (Vector2 val) => { member.Set(val); }, ease);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Vector3:
          {
            Vector3 currentValue = member.Get<Vector3>();
            bool shouldToggle = (toggle && currentValue == vector3Value);
            Vector3 nextValue = shouldToggle ? (Vector3)previousValue : vector3Value;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (Vector3 val) => { property.Set(val); }, Vector3.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (Vector3 val) => { member.Set(val); }, ease);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Vector4:
          {
            Vector4 currentValue = member.Get<Vector4>();
            bool shouldToggle = (toggle && currentValue == vector4Value);
            Vector4 nextValue = shouldToggle ? (Vector4)previousValue : vector4Value;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (Vector4 val) => { property.Set(val); }, Vector4.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (Vector4 val) => { member.Set(val); }, ease);
            previousValue = currentValue;
          }
          break;
        case ActionProperty.Types.Color:
          {
            Color currentValue = member.Get<Color>();
            bool shouldToggle = (toggle && currentValue == colorValue);
            Color nextValue = shouldToggle ? (Color)previousValue : colorValue;
            //interpolator = Routines.Lerp(currentValue, nextValue, duration, (Color val) => { property.Set(val); }, Color.Lerp);
            interpolator = Routines.Interpolate(currentValue, nextValue, duration, (Color val) => { member.Set(val); }, ease);
            previousValue = currentValue;
          }
          break;
        default:
          break;
      }
      return interpolator;
    }
    
    /// <summary>
    /// Validates the type of this property in editor
    /// </summary>
    public void Validate()
    {
      if (member.isAssigned)
        memberType = ActionProperty.Deduce(member.type);
      else
        memberType = ActionProperty.Types.None;
    }

    /// <summary>
    /// Sets this property
    /// </summary>
    public void Set(MonoBehaviour owner)
    {
      if (!member.isAssigned)
        return;
            
      if (!isRegistered)
        Register();

      interpolateRoutine = MakeInterpolateRoutine(); // Routines.Lerp(interpolateFunc, duration);
      owner.StartCoroutine(interpolateRoutine, identifier);
      
    }

    private void Register()
    {
      id = created++;
      Debug.Log($"Boop from {id}");
    }

  }

}