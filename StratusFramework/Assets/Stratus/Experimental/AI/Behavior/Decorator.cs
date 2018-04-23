using UnityEngine;
using Stratus;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// A branch in a tree with only a single child.
    /// </summary>
    public abstract class Decorator : Behavior
    {
      Behavior Child;
    }  

  }
}
