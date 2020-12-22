using UnityEngine;
using System.Collections.Generic;

namespace Stratus.AI
{
	/// <summary>
	/// Branches with multiple children in a behavior tree
	/// are called composite beaviors. We make more interesting,
	/// intelligent behaviors by combining simpler behaviors together.
	/// </summary>
	public abstract class StratusAIComposite : StratusAIBehavior, IServiceSupport
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[OdinSerializer.OdinSerialize]
		public List<StratusAIService> services = new List<StratusAIService>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		List<StratusAIService> IServiceSupport.services => this.services;
		public IList<StratusAIBehavior> children { private set; get; }
		public abstract StratusAIBehavior currentChild { get; }
		public bool hasChildren => children.NotNullOrEmpty();
		public static Color color => StratusGUIStyles.Colors.valencia;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnCompositeStart(Arguments args);
		protected abstract bool OnCompositeSetNextChild(Arguments args);
		protected abstract bool OnCompositeChildEnded(Arguments args, Status status);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public override void Update(Arguments args)
		{
			foreach (var service in this.services)
				service.Execute(args.agent);
			base.Update(args);
		}

		protected override void OnStart(Arguments args)
		{
			this.OnCompositeStart(args);
			this.OnCompositeSetNextChild(args);
		}

		protected override Status OnUpdate(Arguments args)
		{
			return Status.Running;
		}

		protected override void OnEnd(Arguments args)
		{
			foreach (var child in children)
			{
				child.Reset();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Set(IList<StratusAIBehavior> children)
		{
			this.children = children;
		}

		//private bool OnChildEnded(Arguments args, Status status)
		//{
		//  this.OnCompositeChildEnded(args, status);
		//  return true;
		//}

	}
}