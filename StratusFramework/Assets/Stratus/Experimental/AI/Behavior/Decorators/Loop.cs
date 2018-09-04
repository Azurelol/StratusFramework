using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  /// <summary>
  /// Bases its condition on wheher its loop counter has exceeded
  /// </summary>
  public class Loop : PostExecutionDecorator 
  { 
    public int counter = 3;
    public Counter currentCounter { get; set; }

    public override string description => "Bases its condition on wheher its loop counter has exceeded";

    protected override void OnDecoratorStart(Arguments args)
    {
      this.currentCounter = new Counter(this.counter);
    }

    protected override bool OnDecoratorChildEnded(Arguments args, Status status)
    {      
      if (status == Status.Failure)
        return false;

      this.currentCounter.Increment();
      return !this.currentCounter.isAtLimit;
    }

    //protected override bool OnDecoratorCanChildExecute(Arguments args)
    //{
    //  return !this.currentCounter.isAtLimit;
    //}

    //protected override bool OnDecoratorCanChildExecute(Arguments args)
    //{
    //  
    //}
  }

}