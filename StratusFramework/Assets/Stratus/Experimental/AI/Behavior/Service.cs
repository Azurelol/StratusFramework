using System.Collections;
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Services attach to Composite nodes, and will execute at their defined frequency as long     
    /// as their branch is being executed. These are often used to make checks and to update the Blackboard. 
    /// These take the place of traditional Parallel nodes in other Behavior Tree systems
    /// </summary>
    public abstract class Service : Behavior
    {
    }
  }

}