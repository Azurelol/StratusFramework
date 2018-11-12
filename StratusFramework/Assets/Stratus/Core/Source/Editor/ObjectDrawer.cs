using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using OdinSerializer;
using Stratus.Utilities;
using System.Linq;

namespace Stratus
{
  public partial class StratusSerializedSystemObject
  {
    /// <summary>
    /// Base class for all drawers
    /// </summary>
    public abstract class Drawer
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      public struct Arguments
      {
        public object target;
        public bool isChild;
      }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public string name { get; protected set; }
      public string displayName { get; protected set; }
      public Type type { get; protected set; }
      public abstract bool DrawEditorGUILayout(object target, bool isChild = false);
      public abstract bool DrawEditorGUI(Rect position, object target);
      public bool isDrawable { get; protected set; }
      public bool isPrimitive { get; protected set; }
      public float height { get; protected set; }
      public static float lineHeight => StratusEditorUtility.lineHeight;
      public static float labelWidth => StratusEditorUtility.labelWidth;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      internal void SetDisplayName(string name)
      {
        this.displayName = ObjectNames.NicifyVariableName(name);
      }
    }

    /// <summary>
    /// Base class for all drawers
    /// </summary>
    public abstract class ObjectDrawer : Drawer
    {
      public FieldInfo[] fields { get; private set; }
      public Dictionary<string, FieldInfo> fieldsByName { get; private set; } = new Dictionary<string, FieldInfo>();
      public int fieldCount => fields.Length;
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }

      public ObjectDrawer(Type type)
      {
        this.type = type;        
        MemberInfo[] members = OdinSerializer.FormatterUtilities.GetSerializableMembers(type, OdinSerializer.SerializationPolicies.Unity);
        this.fields = members.OfType<FieldInfo>().ToArray();
        this.fieldsByName.AddRange(this.fields, (FieldInfo field) => field.Name);
        this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
      }
    }

    public struct DrawCommand
    {
      public Drawer drawer;
      public FieldInfo field;
      public bool isField;

      public DrawCommand(Drawer drawer, FieldInfo field, bool isField)
      {
        this.drawer = drawer;
        this.field = field;
        this.isField = isField;
      }
    }

    //------------------------------------------------------------------------/
    // Properties: Static
    //------------------------------------------------------------------------/
    private static Dictionary<Type, ObjectDrawer> objectDrawers { get; set; } = new Dictionary<Type, ObjectDrawer>();
    private static Dictionary<FieldInfo, FieldDrawer> fieldDrawers { get; set; } = new Dictionary<FieldInfo, FieldDrawer>();
    private static Type[] customObjectDrawers { get; set; } = Reflection.GetSubclass<CustomObjectDrawer>(false);


  }

}