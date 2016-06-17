using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------------------------------------------
// Object Reference - © 2014, 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// This source file is part of Inspector Navigator
// ---------------------------------------------------------------------------------------------------------------------------
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.InspectorNavigator
{
    [System.Serializable]
    public enum ObjectType {
        None,
        Asset,
        Instance,
        Folder,
        Scene,
        ProjectSettings,
        TextAssets,
        InspectorBreadcrumbs,
        AssetStoreAssetInspector
    }

	[System.Serializable]
	public class ObjectReference
	{
		[SerializeField]
		public Object OReference;
        [SerializeField]
        public ObjectType OType;
		[SerializeField]
		public Vector3 CPosition;
		[SerializeField]
		public Quaternion CRotation;
		[SerializeField]
		public float CSize;
		[SerializeField]
		public bool COrtho;
	};
}