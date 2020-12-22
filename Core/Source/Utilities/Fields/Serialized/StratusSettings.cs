using System;

using UnityEngine;

namespace Stratus
{
	public interface IStratusPrefs
	{
		string dataTypeName { get; }
		bool Load();
		bool Save();
		void Reset();
	}

	public interface IStratusPrefs<DataType> : IStratusPrefs
		where DataType : class, new()
	{
		DataType data { get; }
	}

	public abstract class StratusUserSettings : IStratusLogger, IStratusPrefs
	{
		public string key { get; private set; }
		public abstract bool valid { get; }
		public abstract string dataTypeName { get; }

		public abstract bool Load();
		public abstract bool Save();
		public abstract void Reset();

		public StratusUserSettings(bool appendDataPath = false)			
		{
			string key = $"{GetType().GetNiceName()}";
			Initialize(key, appendDataPath);
		}

		public StratusUserSettings(string key, bool appendDataPath = false)
		{
			Initialize(key, appendDataPath);
		}

		private void Initialize(string key, bool appendDataPath = false)
		{
			if (appendDataPath)
			{
				key = $"{Application.dataPath}/{key}"; ;
			}

			this.key = key;
			if (!this.Load())
			{
				this.LogError($"Failed to load data from {key}");
			}
			
		}
	}

	/// <summary>
	/// Base class for settings
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public abstract class StratusUserSettings<DataType> : StratusUserSettings
		where DataType : class, new()
	{
		public DataType data { get; private set; }
		public override string dataTypeName => typeof(DataType).Name;
		public override bool valid => data != null;
		

		public StratusUserSettings(string key, bool appendDataPath = false)
			: base(key, appendDataPath)
		{
		}

		public StratusUserSettings(bool appendDataPath = false) : base(appendDataPath)
		{
		}

		public override bool Load()
		{
			this.data = Load(key);
			return valid;
		}

		public override bool Save()
		{
			return Save(this.key, this.data);
		}

		public override void Reset()
		{
			this.data = ConstructData();
		}

		public DataType Load(string key)
		{
			DataType data = null;

			if (PlayerPrefs.HasKey(key))
			{
				try
				{
					string serialization = GetPrefs(key);
					data = Deserialize(serialization);
				}
				catch (Exception e)
				{
					this.LogError($"Failed to deserialize data of type {typeof(DataType).Name}: {e}");
					data = ConstructData();
				}
			}
			else
			{
				this.Log("No data present. Constructing anew...");
				data = ConstructData();
			}
			return data;
		}

		protected virtual DataType ConstructData()
		{
			DataType data = new DataType();
			if (data == null)
			{
				this.LogError($"Failed to construct data of type {dataTypeName}");
			}
			return data;
		}

		public bool Save(string key, DataType data)
		{
			string serialization = Serialize(data);
			SetPrefs(key, serialization);
			return true;
		}

		protected virtual string Serialize(DataType data)
		{
			return StratusJSONSerializerUtility.Serialize(data);
		}

		protected virtual DataType Deserialize(string serialization)
		{
			return StratusJSONSerializerUtility.Deserialize<DataType>(serialization);
		}

		protected abstract string GetPrefs(string key);
		protected abstract void SetPrefs(string key, string serialization);
	}

	/// <summary>
	/// Settings meant to be used at runtime by a player
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public class StratusPlayerPrefs<DataType> : StratusUserSettings<DataType>,
		IStratusPrefs<DataType>
		where DataType : class, new()
	{
		public StratusPlayerPrefs(bool appendDataPath = false) : base(appendDataPath)
		{
		}

		public StratusPlayerPrefs(string key, bool appendDataPath = false) : base(key, appendDataPath)
		{
		}

		protected override string GetPrefs(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		protected override void SetPrefs(string key, string serialization)
		{
			PlayerPrefs.SetString(key, serialization);
		}
	}

	/// <summary>
	/// Settings meant to be used by an editor
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public class StratusEditorPrefs<DataType> : StratusUserSettings<DataType>
where DataType : class, new()
	{
		public StratusEditorPrefs(string key, bool appendDataPath = false) : base(key, appendDataPath)
		{
		}

		protected override string GetPrefs(string key)
		{
			string serialization = null;
#if UNITY_EDITOR
			serialization = UnityEditor.EditorPrefs.GetString(key);
#endif
			return serialization;
		}

		protected override void SetPrefs(string key, string serialization)
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetString(key, serialization);
#endif
		}
	}

	public class StratusPlayerSettingsSingleton<DataType> : StratusSingleton<StratusPlayerSettingsSingleton<DataType>>
		where DataType : class, new()
	{
		/// <summary>
		/// The data managing class
		/// </summary>
		public static IStratusPrefs<DataType> prefs
		{
			get
			{
				if (_prefs == null)
				{
					InitializeData();
				}
				return _prefs;
			}
		}

		/// <summary>
		/// The data accessed through the Prefs
		/// </summary>
		public static DataType data => prefs.data;

		private static StratusPlayerPrefs<DataType> _prefs;

		private static void InitializeData()
		{
			_prefs = new StratusPlayerPrefs<DataType>();
		}

		protected override void OnInitialize()
		{
		}

		public void ApplyData()
		{
			_prefs.Save();
		}

		public void ResetData()
		{
			_prefs.Reset();
		}
	}

}