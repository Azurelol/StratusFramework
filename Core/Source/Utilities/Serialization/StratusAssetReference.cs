using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    [Serializable]
    public abstract class StratusAssetReference<AssetType> 
        where AssetType : class
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        [StratusDropdown(nameof(assetNames))]
        public string label;

        /// <summary>
        /// The asset in question. It will be queried the first time this propery is accessed.
        /// </summary>
        public AssetType asset
        {
            get
            {
                if (!queried || IsNull(_asset))
                {
                    if (label.IsValid())
                    {
                        _asset = GetAsset(label);
                    }
                    else
                    {
                        Debug.LogError("Empty or null label found in asset reference. This is not allowed...");
                    }

                    queried = true;
                }
                return _asset;
            }
        }
        private AssetType _asset = null;
        private bool queried = false;

        protected virtual bool IsNull(AssetType asset) => asset == null;

        /// <summary>
        /// The names of all available assets of this type
        /// </summary>
        private string[] assetNames => GetAvailableAssetNames();

        public override string ToString()
        {
            return label;
        }

        protected abstract string[] GetAvailableAssetNames();
        protected abstract AssetType GetAsset(string label);
    }

    [Serializable]
    public abstract class StratusUnityAssetReference<AssetType>
        : StratusAssetReference<AssetType>
        where AssetType : UnityEngine.Object
    {
        protected override bool IsNull(AssetType asset)
        {
            return Stratus.OdinSerializer.Utilities.UnityExtensions.SafeIsUnityNull(asset);
        }
    }

}