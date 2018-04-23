/******************************************************************************/
/*!
@file   CollectionsExtensions.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;


namespace Stratus 
{
  public static partial class Extensions
  {   

    /// <summary>
    /// Copies every element of the list into the stack.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stack">The stack.</param>
    /// <param name="list">The list</param>
    public static void Copy<T>(this Stack<T> stack, List<T> list)
    {
      foreach(var element in list)
      {
        stack.Push(element);
      }
    }



  }
}
