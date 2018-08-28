using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace AI
  {
    public abstract class Decorator : Behavior
    {
      Behavior child;
    }

    public interface IDecoratorSupport
    {
      List<Decorator> decorators { get; }
    }

  }
}
