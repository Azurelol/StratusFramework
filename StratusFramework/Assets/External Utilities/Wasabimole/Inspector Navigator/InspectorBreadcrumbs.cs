using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ---------------------------------------------------------------------------------------------------------------------------
// Inspector Breadcrumbs - © 2014, 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// This source file is part of Inspector Navigator
// ---------------------------------------------------------------------------------------------------------------------------
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.InspectorNavigator
{
	public class InspectorBreadcrumbs : MonoBehaviour
	{
        [HideInInspector]
        public int DataVersion;
		public ObjectReference CurrentSelection;
		public List<ObjectReference> Back = new List<ObjectReference> ();
		public List<ObjectReference> Forward = new List<ObjectReference> ();
	}
}
