using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  public class MaterialEvent : Triggerable
  {    
    public enum Type
    {
      [Tooltip("The main material's color")]
      Color,
      SetColor,
      SetFloat,
      SetInteger,
      SetTexture,
      [Tooltip("Interpolates properties between materials")]
      Lerp = 16      
    }

    public Material material;
    public Type type;
    public string propertyName;
    public float floatValue;
    public int integerValue;
    public Color color = Color.white; 
    public Texture texture;
    public Material material2;
    public float duration = 1.0f;

    private Material originalMaterial;
    public static Dictionary<string, Material> restoredMaterials = new Dictionary<string, Material>();

    protected override void OnAwake()
    {
      originalMaterial = new Material(material);
    }

    protected override void OnReset()
    {

    }

    private void OnDestroy()
    {
      // If the material has been previously restored, we don't want to do it again
      // This is so that we can ensure that we restore the material?
      bool hasBeenRestored = restoredMaterials.ContainsKey(material.name);
      if (hasBeenRestored)
        return;

      // Restore the original material
      material.CopyPropertiesFromMaterial(originalMaterial);
      restoredMaterials.Add(material.name, material);
    }

    protected override void OnTrigger()
    {     

      IEnumerator routine = null;
      switch (type)
      {
        case Type.Color:
          routine = Routines.Lerp(material.color, color, duration, (Color val) => { material.color = val; }, Color.Lerp);
          break;
        case Type.SetFloat:
          routine = Routines.Lerp(material.GetFloat(propertyName), floatValue, duration, (float val) => { material.SetFloat(propertyName, val); }, Routines.Lerp);
          break;
        case Type.SetInteger:
          routine = Routines.Lerp(material.GetInt(propertyName), integerValue, duration, (float val) => { material.SetInt(propertyName, Mathf.CeilToInt(val)); }, Routines.Lerp);
          break;
        case Type.SetTexture:
          routine = Routines.Call(() => { material.SetTexture(propertyName, texture); }, duration);
          break;
        case Type.Lerp:
          routine = Routines.Lerp((float t) => { material.Lerp(material, material2, t); }, duration);
          break;
        default:
          break;
      }
      
      this.StartCoroutine(routine, "Interpolate");
    }
  }

}