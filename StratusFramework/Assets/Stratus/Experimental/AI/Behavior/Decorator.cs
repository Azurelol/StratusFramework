using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus.AI
{
  /// <summary>
  /// Decorator, also known as conditionals in other Behavior Tree systems, 
  /// are attached to either a Composite or a Task node and define whether or not a branch in the 
  /// tree, or even a single node, can be executed.
  /// </summary>
  public abstract class Decorator : Behavior
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Behavior child { private set; get; }
    public static Color color => StratusGUIStyles.Colors.saffron;

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnDecoratorStart(Arguments args);

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnStart(Arguments args)
    {
      this.OnDecoratorStart(args);      
    }

    protected override void OnEnd(Arguments args)
    {
    }

    //------------------------------------------------------------------------/
    // Method
    //------------------------------------------------------------------------/
    public void Set(Behavior child)
    {
      this.child = child;
    }

    //protected void OnDecoratorChildEnded(Arguments args, Status status)
    //{
    //  if (this.OnDecoratorCanChildExecute(args))
    //    this.StartChild(args);
    //  else
    //    this.End(args, this.child.status);
    //}
  }

  /// <summary>
  /// A decorator that checks before the child is run
  /// </summary>
  public abstract class PreExecutionDecorator : Decorator
  {
    protected abstract bool OnDecoratorCanChildExecute(Arguments args);

    protected override void OnStart(Arguments args)
    {
      this.OnDecoratorStart(args);
      if (this.OnDecoratorCanChildExecute(args))
        this.child.Start(args, OnChildEnded);
      else
        this.End(args, Status.Failure);
    }

    protected override Status OnUpdate(Arguments agent)
    {
      return Status.Running;
    }

    private bool OnChildEnded(Arguments args, Status status)
    {
      this.End(args, status);
      return true;
    }
  }

  /// <summary>
  /// A decorator that checks after the child has run
  /// </summary>
  public abstract class PostExecutionDecorator : Decorator
  {
    protected abstract bool OnDecoratorChildEnded(Arguments args, Status status);

    protected override void OnStart(Arguments args)
    {
      base.OnStart(args);
      this.StartChild(args);
    }

    protected override Status OnUpdate(Arguments args)
    {
      return Status.Running;
    }

    private bool OnChildEnded(Arguments args, Status status)
    {
      bool restart = OnDecoratorChildEnded(args, status);
      //Trace.Script($"Restart {child.fullName}? {restart}");
      if (restart)
      {
        this.StartChild(args);
        return false;
      }

      this.End(args, this.child.status);
      return true;
    }

    private void StartChild(Arguments args)
    {
      if (this.child == null)
        throw new ArgumentNullException($"There's no child for the decorator {fullName}");

      //Trace.Script($"Starting {child.fullName}");
      this.child.Start(args, this.OnChildEnded);
    }


  }

}
