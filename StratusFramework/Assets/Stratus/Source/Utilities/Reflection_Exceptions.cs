/******************************************************************************/
/*!
@file   Reflection_Exceptions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note   Credit to Or Aviram: 
        https://forum.unity3d.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
*/
/******************************************************************************/
using System;

namespace Stratus
{
  namespace Utilities
  {
    public static partial class Reflection
    {
      /// <summary>
      /// An exception that is thrown whenever a field was not found inside of an object when using Reflection.
      /// </summary>
      [Serializable]
      public class FieldNotFoundException : Exception
      {
        public FieldNotFoundException() { }

        public FieldNotFoundException(string message) : base(message) { }

        public FieldNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected FieldNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
      }

      /// <summary>
      /// An exception that is thrown whenever a property was not found inside of an object when using Reflection.
      /// </summary>
      [Serializable]
      public class PropertyNotFoundException : Exception
      {        
        public PropertyNotFoundException() { }

        public PropertyNotFoundException(string message) : base(message) { }

        public PropertyNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected PropertyNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
      }

      /// <summary>
      /// An exception that is thrown whenever a field or a property was not found inside of an object when using Reflection.
      /// </summary>
      [Serializable]
      public class PropertyOrFieldNotFoundException : Exception
      {
        public PropertyOrFieldNotFoundException() { }

        public PropertyOrFieldNotFoundException(string message) : base(message) { }

        public PropertyOrFieldNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected PropertyOrFieldNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
      }

    } 
  }
}
