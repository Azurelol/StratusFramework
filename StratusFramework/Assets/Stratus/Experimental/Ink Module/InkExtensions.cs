using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace Stratus.Modules.InkModule
{
  public static class InkExtensions 
  {
    public static void GoToStart(this Ink.Runtime.Story story)
    {
      story.ResetCallstack();
    }

    public static bool HasCurrentKnotBeenVisited(this Ink.Runtime.Story story)
    {
      if (story.state.currentPathString == null)
        return false;

      int count = story.state.VisitCountAtPathString(story.state.currentPathString);
      return count > 1;
      //Path currentPath = story.runtime.state.currentPath;
      //if (currentPath != null)
      //{
      //  Path.Component currentHead = currentPath.head;
      //  string key = story.runtime.state.currentPath.head.name;
      //  if (key != null)
      //  {
      //    int timesVisited = story.runtime.state.VisitCountAtPathString(key);
      //    if (timesVisited > 1)
      //      return true;
      //  }
      //}
      //return false;
    }
  }

}