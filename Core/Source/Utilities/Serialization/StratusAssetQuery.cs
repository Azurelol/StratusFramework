using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Stratus
{
	public class StratusAssetQuery : IStratusLogger
	{
		private Func<string[]> queryAssetNamesFunction;
		private string[] _assetNames;
		
		/// <summary>
		/// Whether assets have been queried
		/// </summary>
		public bool queried { get; private set; }

		/// <summary>
		/// The names of all assets found by the query
		/// </summary>
		public string[] assetNames
		{
			get
			{
				if (_assetNames == null || !updated)
				{
					Update();
				}
				return _assetNames;
			}
		}

		/// <summary>
		/// The count of assets found by the query
		/// </summary>
		public int assetCount => _assetNames.LengthOrZero();

		/// <summary>
		/// Whether this query is up to date
		/// </summary>
		public bool updated { get; private set; }

		public StratusAssetQuery(Func<string[]> queryFunction)
		{
			this.queryAssetNamesFunction = queryFunction;
			this.updated = false;
		}

		public virtual void Update()
		{
			this._assetNames = GetAssetNames();
			this.Log($"{_assetNames.Length} assets were found");
			this.updated = true;
		}

		protected virtual string[] GetAssetNames()
		{
			return queryAssetNamesFunction();
		}

		public void SetDirty() => updated = false;


	}

	public class StratusAssetQuery<AssetType> : StratusAssetQuery
		where AssetType : class
	{
		private StratusSortedList<string, AssetType> _assets;
		private Func<AssetType[]> getAssetsFunction;
		private Func<AssetType, string> keyFunction;

		/// <summary>
		/// The references to the assets found by the query
		/// </summary>
		private StratusSortedList<string, AssetType> assets
		{
			get
			{
				if (_assets == null || !updated)
				{
					Update();
				}
				return _assets;
			}
		}

		public StratusAssetQuery(Func<AssetType[]> getAssetsFunction, Func<AssetType, string> keyFunction)
			: base(null)
		{
			this.getAssetsFunction = getAssetsFunction;
			this.keyFunction = keyFunction;
		}

		public AssetType GetAsset(string label)
		{
			if (!assets.ContainsKey(label))
			{
				this.LogError($"Could not find asset named {label}");
				this.LogError($"Available assets: {assetNames.JoinToString()}");
				return null;
			}
			return assets[label]; ;
		}

		protected override string[] GetAssetNames()
		{
			return _assets.Keys.ToArray();
		}

		public override void Update()
		{
			AssetType[] values = getAssetsFunction();
			_assets = new StratusSortedList<string, AssetType>(keyFunction, values.Length);
			_assets.AddRange(values);

			base.Update();
		}
	}

	public class StratusUnityAssetQuery<AssetType> : StratusAssetQuery<AssetType>
		where AssetType : UnityEngine.Object
	{
		public StratusUnityAssetQuery(Func<AssetType[]> getAssetsFunction) : base(getAssetsFunction, x => x.name)
		{
		}
	}

}