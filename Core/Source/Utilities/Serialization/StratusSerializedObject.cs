using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;

namespace Stratus
{
    /// <summary>
    /// Edits System.Object types in a completely generic way
    /// </summary>
    public class StratusSerializedObject : IStratusLogger
    {
        //------------------------------------------------------------------------/
        // Properties
        //------------------------------------------------------------------------/
        public Type type { get; private set; }
        public object target { get; private set; }
        public StratusSerializedField[] fields { get; private set; }
        public Dictionary<string, StratusSerializedField> fieldsByname { get; private set; } = new Dictionary<string, StratusSerializedField>();
        public bool debug { get; set; }

        //------------------------------------------------------------------------/
        // CTOR
        //------------------------------------------------------------------------/
        public StratusSerializedObject(object target)
        {
            this.target = target;
            this.type = target.GetType();
            this.GenerateFields();
        }

        public override string ToString()
        {
            return $"({type.GetNiceName()}) {target}";
        }

        //------------------------------------------------------------------------/
        // Methods
        //------------------------------------------------------------------------/
        public string Serialize()
        {
            string data = JsonUtility.ToJson(this.target);
            return data;
        }

        public void Deserialize(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this.target);
        }

        public string PrintHierarchy()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ToString());
            foreach(var field in fields)
            {
                sb.Append(field.PrintHierarchy(1));
            }
            return sb.ToString();
        }

        //------------------------------------------------------------------------/
        // Procedures
        //------------------------------------------------------------------------/
        private void GenerateFields()
        {
            FieldInfo[] fields = this.type.GetSerializedFields();
            List<StratusSerializedField> serializedFields = new List<StratusSerializedField>();

            // Backwards since we want the top-most declared classes first
            foreach (FieldInfo field in fields.Reverse())
            {
                StratusSerializedField property = new StratusSerializedField(field, this.target);
                this.fieldsByname.Add(property.name, property);
                serializedFields.Add(property);
            }

            this.fields = serializedFields.ToArray();
        }

        public static bool IsList(object o)
        {
            if (o == null)
            {
                return false;
            }

            return o is IList &&
             o.GetType().IsGenericType &&
             o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsArray(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        public static bool IsDictionary(object o)
        {
            if (o == null)
            {
                return false;
            }

            return o is IDictionary &&
             o.GetType().IsGenericType &&
             o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
    }

}