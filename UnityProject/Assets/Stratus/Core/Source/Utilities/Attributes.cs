using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace Utilities
  {
    public static class AttributeUtility
    {
      /// <summary>
      /// Finds an attribute of the specified type inside the class
      /// </summary>
      /// <typeparam name="AttributeType">The attribute class which was used in the class declaration</typeparam>
      /// <param name="type">The type of the class that was declared with the attribute</param>
      /// <returns></returns>
      public static AttributeType FindAttribute<AttributeType>(Type type) where AttributeType : Attribute
      {
        var attributes = (AttributeType[])type.GetCustomAttributes(typeof(AttributeType), true);
        if (attributes.Length > 0)
          return attributes[0];
        return null;
      }

      //public static ValueType GetAttributeValue<AttributeType, ValueType>()


    }  
  }
}
