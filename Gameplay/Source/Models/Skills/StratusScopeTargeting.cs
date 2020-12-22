using System;
using System.Linq;

namespace Stratus.Gameplay
{
	[Serializable]
	public class StratusScopeTargeting : StratusRangeTargeting
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum Type { Single, Radius, Line, Group }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public Type scope = Type.Single;
		public float length;
		public float width;

	}

}