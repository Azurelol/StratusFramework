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
	public abstract class StratusAIDecorator : StratusAIBehavior
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusAIBehavior child { private set; get; }
		public static Color color => StratusGUIStyles.Colors.saffron;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnDecoratorStart(Arguments args);
		protected abstract bool OnChildEnded(Arguments args, Status status);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnStart(Arguments args)
		{
			this.OnDecoratorStart(args);
		}

		protected override Status OnUpdate(Arguments agent)
		{
			return Status.Running;
		}

		protected override void OnEnd(Arguments args)
		{
		}

		//------------------------------------------------------------------------/
		// Method
		//------------------------------------------------------------------------/
		public void Set(StratusAIBehavior child)
		{
			this.child = child;
		}

		protected void StartChild(Arguments args)
		{
			if (this.child == null)
				throw new ArgumentNullException($"There's no child for the decorator {fullName}");

			//Trace.Script($"Starting {child.fullName}");
			this.child.Start(args, this.OnChildEnded);
		}
	}

	/// <summary>
	/// A decorator that checks before the child is run
	/// </summary>
	public abstract class PreExecutionDecorator : StratusAIDecorator
	{
		protected abstract bool OnDecoratorCanChildExecute(Arguments args);

		protected override void OnDecoratorStart(Arguments args)
		{
			if (this.OnDecoratorCanChildExecute(args))
				this.child.Start(args, OnChildEnded);
			else
				this.End(args, Status.Failure);
		}

		protected override bool OnChildEnded(Arguments args, Status status)
		{
			this.End(args, status);
			return true;
		}
	}

	/// <summary>
	/// A decorator that checks after the child has run
	/// </summary>
	public abstract class PostExecutionDecorator : StratusAIDecorator
	{
		//protected abstract void OnPostExecutionDecoratorChildEnded(Arguments args, Status status);

		protected override void OnDecoratorStart(Arguments args)
		{
			this.StartChild(args);
		}

		//protected override bool OnChildEnded(Arguments args, Status status)
		//{
		//  this.OnPostExecutionDecoratorChildEnded(args, status);
		//  this.End(args, status);
		//  return true;
		//}

	}

	public abstract class PostExecutionRepeatingDecorator : PostExecutionDecorator
	{
		protected abstract bool OnRepeatingDecoratorChildEnded(Arguments args, Status status);
		protected override bool OnChildEnded(Arguments args, Status status)
		{
			bool restart = OnRepeatingDecoratorChildEnded(args, status);
			if (restart)
			{
				this.StartChild(args);
				return false;
			}

			this.End(args, this.child.status);
			return true;
		}


	}

}
