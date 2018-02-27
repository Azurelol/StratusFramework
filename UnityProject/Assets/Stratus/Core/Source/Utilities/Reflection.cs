/******************************************************************************/
/*!
@file   Reflection.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note   Credit to Or Aviram: 
        https://forum.unity3d.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
*/
/******************************************************************************/
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Stratus
{
  namespace Utilities
  {
    public static partial class Reflection
    {
      //----------------------------------------------------------------------/
      // Structs
      //----------------------------------------------------------------------/
      

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      private static Assembly[] _allAssemblies;
      public static Assembly[] AllAssemblies
      {
        get
        {
          if (_allAssemblies == null)
          {
            _allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
          }

          return _allAssemblies;
        }
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Gets all the types that have at least one attribute in the given assembly
      /// </summary>
      /// <param name="assembly"></param>
      /// <param name="attribute"></param>
      /// <returns></returns>
      public static IEnumerable<Type> GetAllTypesWithAttributeAsEnumerable(this Assembly assembly, Type attribute)
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (type.GetCustomAttributes(attribute.GetType(), true).Length > 0)
          {
            yield return type;
          }
        }
      }

      /// <summary>
      /// Get all the types that have at least one attribute in the given assembly
      /// </summary>
      /// <param name="assembly"></param>
      /// <param name="attribute"></param>
      /// <returns></returns>
      public static Type[] GetAllTypesWithAttribute(this Assembly assembly, Type attribute)
      {
        return assembly.GetAllTypesWithAttributeAsEnumerable(attribute).ToArray();
      }

      /// <summary>
      /// Finds the most nested object inside of an object.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="obj"></param>
      /// <param name="path"></param>
      /// <returns></returns>
      public static T GetNestedObject<T>(this object obj, string path)
      {
        foreach (string part in path.Split('.'))
        {
          obj = obj.GetFieldOrProperty<T>(part);
        }
        return (T)obj;
      }

      /// <summary>
      /// Gets a property or a field of an object by a name.
      /// </summary>
      /// <typeparam name="T">Type of the field/property.</typeparam>
      /// <param name="obj">Object the field/property should be found in.</param>
      /// <param name="name">Name of the field/property.</param>
      /// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
      /// <returns>The field/property.</returns>
      public static T GetFieldOrProperty<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        try
        {
          // Try getting the field. If the property wasn't found...
          return GetField<T>(obj, name, bindingFlags);
        }
        catch (FieldNotFoundException)
        {
          //...try getting the property. If that wasn't found as well, throw an exception
          try
          {
            return GetProperty<T>(obj, name, bindingFlags);
          }
          catch (PropertyNotFoundException)
          {
            throw new PropertyOrFieldNotFoundException("Couldn't find a filed nor a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
          }
        }

      }

      /// <summary>
      /// Gets a field inside of an object by a name.
      /// </summary>
      /// <typeparam name="T">Type of the field.</typeparam>
      /// <param name="obj">Object the field should be found in.</param>
      /// <param name="name">Name of the field.</param>
      /// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>
      /// <returns>The field.</returns>
      public static T GetField<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Try getting the field and returning it.
        FieldInfo field = obj.GetType().GetField(name, bindingFlags);
        if (field != null)
          return (T)field.GetValue(obj);

        // If a field couldn't be found. Throw an exception about it.
        throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
      }

      /// <summary>
      /// Gets a property inside of an object by a name.
      /// </summary>
      /// <typeparam name="T">Type of the property.</typeparam>
      /// <param name="obj">Object the property should be found in.</param>
      /// <param name="name">Name of the property.</param>
      /// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
      /// <returns>The property.</returns>
      public static T GetProperty<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Try getting the field and returning it.
        PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
        if (property != null)
          return (T)property.GetValue(obj, null);

        // If a field couldn't be found. Throw an exception about it.
        throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
      }      

      /// <summary>
      /// Sets a field or a property inside of an object by name.
      /// </summary>
      /// <typeparam name="T">Type of the field/property.</typeparam>
      /// <param name="obj">Object contaning the field/property.</param>
      /// <param name="name">Name of the field/property.</param>
      /// <param name="value">New value of the field/property.</param>
      /// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
      public static void SetFieldOrProperty<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        try
        {
          // Try getting the field. If the property wasn't found...
          SetField(obj, name, value, bindingFlags);
          return;
        }
        catch (FieldNotFoundException)
        {
          //...try getting the property. If that wasn't found as well, throw an exception
          try
          {
            SetProperty(obj, name, value, bindingFlags);
            return;
          }
          catch (PropertyNotFoundException)
          {
            throw new PropertyOrFieldNotFoundException("Couldn't find a filed nor a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
          }
        }

      }

      /// <summary>
      /// Sets a field inside of an object by name.
      /// </summary>
      /// <typeparam name="T">Type of the field.</typeparam>
      /// <param name="obj">Object contaning the field.</param>
      /// <param name="name">Name of the field.</param>
      /// <param name="value">New value of the field.</param>
      /// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>>
      public static void SetField<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Try getting the field and returning it.
        FieldInfo field = obj.GetType().GetField(name, bindingFlags);
        if (field != null)
        {
          field.SetValue(obj, value);
          return;
        }

        // If a field couldn't be found. Throw an exception about it.
        throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
      }

      /// <summary>
      /// Sets a property inside of an object by name.
      /// </summary>
      /// <typeparam name="T">Type of the property.</typeparam>
      /// <param name="obj">Object contaning the property.</param>
      /// <param name="name">Name of the property.</param>
      /// <param name="value">New value of the property.</param>
      /// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
      public static void SetProperty<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Try getting the field and returning it.
        PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
        if (property != null)
        {
          property.SetValue(obj, value, null);
          return;
        }

        // If a field couldn't be found. Throw an exception about it.
        throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
      }
      
      /// <summary>
      /// Gets all the properties and fields in obj of type T.
      /// </summary>
      /// <typeparam name="T">The type of the fields/properties.</typeparam>
      /// <param name="obj">Object to find the fields/properties in.</param>
      /// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
      /// <returns>The fields/properties found.</returns>
      public static IEnumerable<T> GetAllFieldsOrProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get the fields and the properties in the object.
        T[] fields = obj.GetAllFields<T>(bindingFlags).ToArray();
        T[] properties = obj.GetAllProperties<T>(bindingFlags).ToArray();

        // Only return the fields if fields were found.
        if (fields != null && fields.Length != 0)
        {
          // Loop through the fields and return each one.
          for (int i = 0; i < fields.Length; i++)
          {
            yield return fields[i];
          }
        }

        // Only return the properties if properties were found.
        if (properties != null && properties.Length != 0)
        {
          // Loop through the properties and return each one if they have the right type.
          for (int i = 0; i < properties.Length; i++)
          {
            yield return properties[i];
          }
        }
      }

      /// <summary>
      /// Gets all the properties and fields in obj.
      /// </summary>
      /// <param name="obj">Object to find the fields/properties in.</param>
      /// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
      /// <returns>The fields/properties found.</returns>
      public static IEnumerable GetAllFieldsOrProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get the fields and the properties in the object.
        object[] fields = obj.GetAllFields(bindingFlags).Cast<object>().ToArray();
        object[] properties = obj.GetAllProperties(bindingFlags).Cast<object>().ToArray();

        // Only return the fields if fields were found.
        if (fields != null && fields.Length != 0)
        {
          // Loop through the fields and return each one.
          for (int i = 0; i < fields.Length; i++)
          {
            yield return fields[i];
          }
        }

        // Only return the properties if properties were found.
        if (properties != null && properties.Length != 0)
        {
          // Loop through the properties and return each one if they have the right type.
          for (int i = 0; i < properties.Length; i++)
          {
            yield return properties[i];
          }
        }
      }

      /// <summary>
      /// Gets all the fields in obj of type T.
      /// </summary>
      /// <typeparam name="T">Type of the fields allowed.</typeparam>
      /// <param name="obj">Object to find the fields in.</param>
      /// <param name="bindingFlags">Filters of the fields allowed.</param>
      /// <returns>The fields found.</returns>
      public static IEnumerable<T> GetAllFields<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get all the properties.
        FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

        // If there are no properties, break.
        if (fields == null || fields.Length == 0)
          yield break;

        // If there are properties in the array, return each element.
        for (int i = 0; i < fields.Length; i++)
        {
          object currentValue = fields[i].GetValue(obj);

          if (currentValue.GetType() == typeof(T))
            yield return (T)currentValue;
        }
      }

      /// <summary>
      /// Gets all the fields in obj.
      /// </summary>
      /// <param name="obj">Object to find the fields in.</param>
      /// <param name="bindingFlags">Filters of the fields allowed.</param>
      /// <returns>The fields found.</returns>
      public static IEnumerable GetAllFields(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get all the properties.
        FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

        // If there are no properties, break.
        if (fields == null || fields.Length == 0)
          yield break;

        // If there are properties in the array, return each element.
        for (int i = 0; i < fields.Length; i++)
        {
          yield return fields[i].GetValue(obj);
        }
      }

      /// <summary>
      /// Gets all the properties in obj of type T.
      /// </summary>
      /// <typeparam name="T">Type of the properties allowed.</typeparam>
      /// <param name="obj">Object to find the properties in.</param>
      /// <param name="bindingFlags">Filters of the properties allowed.</param>
      /// <returns>The properties found.</returns>
      public static IEnumerable<T> GetAllProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get all the properties.
        PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

        // If there are no properties, break.
        if (properties == null || properties.Length == 0)
          yield break;

        // If there are properties in the array, return each element.
        for (int i = 0; i < properties.Length; i++)
        {
          object currentValue = properties[i].GetValue(obj, null);

          if (currentValue.GetType() == typeof(T))
            yield return (T)currentValue;
        }
      }

      /// <summary>
      /// Gets all the properties in obj.
      /// </summary>
      /// <param name="obj">Object to find the properties in.</param>
      /// <param name="bindingFlags">Filters of the properties allowed.</param>
      /// <returns>The properties found.</returns>
      public static IEnumerable GetAllProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      {
        // Get all the properties.
        PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

        // If there are no properties, break.
        if (properties == null || properties.Length == 0)
          yield break;

        // If there are properties in the array, return each element.
        for (int i = 0; i < properties.Length; i++)
        {
          yield return properties[i].GetValue(obj, null);
        }
      }
      
      /// <summary>
      /// Get the name of all classes derived from the given one
      /// </summary>
      /// <typeparam name="ClassType"></typeparam>
      /// <param name="includeAbstract"></param>
      /// <returns></returns>
      public static string[] GetSubclassNames<ClassType>(bool includeAbstract = false)
      {
        string[] typeNames;
        Type[] types = Assembly.GetAssembly(typeof(ClassType)).GetTypes();
        typeNames = (from Type type in types where type.IsSubclassOf(typeof(ClassType)) && !type.IsAbstract select type.Name).ToArray();
        return typeNames;
      }

      /// <summary>
      /// Get the name of all classes derived from the given one
      /// </summary>
      /// <param name="includeAbstract"></param>
      /// <returns></returns>
      public static string[] GetSubclassNames(Type baseType, bool includeAbstract = false)
      {
        string[] typeNames;
        Type[] types = Assembly.GetAssembly(baseType).GetTypes();
        typeNames = (from Type type in types where type.IsSubclassOf(baseType) && !type.IsAbstract select type.Name).ToArray();
        return typeNames;
      }

      /// <summary>
      /// Get an array of types of all the classes derived from the given one
      /// </summary>
      /// <param name="includeAbstract"></param>
      /// <returns></returns>
      public static Type[] GetSubclass<ClassType>(bool includeAbstract = false)
      {
        if (includeAbstract)
          return (from Type type in Assembly.GetAssembly(typeof(ClassType)).GetTypes() where type.IsSubclassOf(typeof(ClassType)) select type).ToArray();

        return (from Type type in Assembly.GetAssembly(typeof(ClassType)).GetTypes() where type.IsSubclassOf(typeof(ClassType)) && !type.IsAbstract select type).ToArray();

      }

      /// <summary>
      /// Get an array of types of all the classes derived from the given one
      /// </summary>
      /// <typeparam name="ClassType"></typeparam>
      /// <param name="includeAbstract"></param>
      /// <returns></returns>
      public static Type[] GetSubclass(Type baseType, bool includeAbstract = false)
      {
        if (includeAbstract)
          return (from Type type in Assembly.GetAssembly(baseType).GetTypes() where type.IsSubclassOf(baseType) select type).ToArray();

        return (from Type type in Assembly.GetAssembly(baseType).GetTypes() where type.IsSubclassOf(baseType) && !type.IsAbstract select type).ToArray();
      }


      /// <summary>
      /// Retrieves the name of this property / field as well as its owning object.
      /// Note: This is quite an expensive call so use sparingly.
      /// </summary>
      /// <param name="varExpr">A lambda expression capturing a reference to a field or property</param>
      /// <returns></returns>
      public static MemberReference GetReference<T>(Expression<Func<T>> varExpr)
      {
        // Slow, probs
        //var cast = varExpr as Expression<Func<object>>;
        return MemberReference.Construct(varExpr);

        //// Use expressions to find the underlying owner object
        //var memberExpr = varExpr.Body as MemberExpression;
        //var inst = memberExpr.Expression;
        //var targetObj = Expression.Lambda<Func<object>>(inst).Compile()();
        //
        //// Get the name of the variable
        //var variableName = memberExpr.Member.Name;
        //
        //var variableReference = new MemberReference();
        //variableReference.label = variableName;
        //variableReference.target = targetObj;
        //        
        //// Check if it's a property
        //var property = targetObj.GetType().GetProperty(variableName);
        //if (property != null)
        //{
        //  variableReference.property = property;
        //  variableReference.type = property.PropertyType;
        //  return variableReference;
        //}
        //
        //// Check if it's a field
        //var field = targetObj.GetType().GetField(variableName);
        //if (field != null)
        //{
        //  variableReference.field = field;
        //  variableReference.type = field.FieldType;
        //  return variableReference;
        //}
        //
        //// Invalid
        //throw new ArgumentException("The given variable is neither a property or a field!");        
      }

      /// <summary>
      /// A list containing all the subclasses deriving from a particular class
      /// </summary>
      public class ClassList : List<KeyValuePair<string, Type>> { }

      /// <summary>
      /// Generates a list of key-value pairs of classes that derive from this one
      /// </summary>
      /// <typeparam name="ClassType"></typeparam>
      /// <returns></returns>
      public static ClassList GenerateClassList<ClassType>(bool includeAbstract = true)
      {
        var list = new ClassList();

        var classes = Reflection.GetSubclass<ClassType>();
        foreach (var e in classes)
        {
          var name = e.FullName.Replace('+', '.');
          var type = e.ReflectedType;

          if (!includeAbstract && type.IsAbstract)
            continue;

          list.Add(new KeyValuePair<string, Type>(name, type));
        }
        return list;
      }     


      

    }
  }
}

