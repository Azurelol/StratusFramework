using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class RendererEvent : Triggerable
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum Type
    {
      Renderer,
      Material
    }

    public enum MaterialModification
    {
      Color, 
      Float,
      Integer
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    public Type type = Type.Renderer;
    // Renderer
    public new Renderer renderer;
    // Material
    public Material material;
    public Color color;
    public string propertyName;
    public float floatValue;
    public int integerValue;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {

    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      ModifyMaterial();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    void ModifyMaterial()
    {
      material.color = color;
      material.SetFloat(propertyName, floatValue);
      material.SetInt(propertyName, integerValue);
      
    }



  }
}