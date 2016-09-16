/******************************************************************************/
/*!
@file   Reflection.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Reflection;
using System.Linq;
 

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class Reflection 
  */
  /**************************************************************************/
  public static class Reflection
  {

    public static string[] getSubclassNames<ClassType>(bool includeAbstract = false)
    {
      string[] typeNames;
      Type[] types = Assembly.GetAssembly(typeof(ClassType)).GetTypes();
      typeNames = (from Type type in types where type.IsSubclassOf(typeof(ClassType)) && !type.IsAbstract select type.Name).ToArray();
      return typeNames;
    }

    public static Type[] getSubclass<ClassType>(bool includeAbstract = false)
    {
      if (includeAbstract)
        return (from Type type in Assembly.GetAssembly(typeof(ClassType)).GetTypes() where type.IsSubclassOf(typeof(ClassType)) select type).ToArray();

      return (from Type type in Assembly.GetAssembly(typeof(ClassType)).GetTypes() where type.IsSubclassOf(typeof(ClassType)) && !type.IsAbstract select type).ToArray();

    }

  }

}