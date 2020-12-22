using UnityEngine;

namespace Stratus
{
	public static class StratusActionExtensions
	{

		public static StratusActionDriver Actions(this GameObject go)
		{
			// Subscribe this GameObject to the ActionSpace
			return StratusActionSpace.Subscribe(go);
		}

	} 
}



