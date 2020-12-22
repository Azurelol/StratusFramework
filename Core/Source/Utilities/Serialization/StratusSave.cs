using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Base class for saves
	/// </summary>
	public abstract class StratusSave : ISerializationCallbackReceiver, IStratusLogger, IDisposable
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The user-given name for this save
		/// </summary>
		public string name;

		/// <summary>
		/// The date at which this save was made
		/// </summary>
		public string date;

		/// <summary>
		/// The current total time of this save, in minutes.
		/// </summary>
		public int playtime;

		/// <summary>
		/// An optional description for the state of the game at this save
		/// </summary>
		public string description;

		/// <summary>
		/// The index of this save. For save systems with a limited amount of slots,
		/// this is its slot
		/// </summary>
		public int index = -1;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The filename this data uses
		/// </summary>
		public string fileName { get; private set; }

		/// <summary>
		/// The file path where this save is saved to
		/// </summary>
		public string filePath { get; private set; }

		/// <summary>
		/// Whether this data  has been saved to disk
		/// </summary>
		public bool serialized => filePath.IsValid();

		/// <summary>
		/// A saved snapshot
		/// </summary>
		public Texture2D snapshot { get; private set; }

		/// <summary>
		/// Whether this save has a valid index assigned
		/// </summary>
		public bool hasValidIndex => index >= 0;

		/// <summary>
		/// The path for the snapshot image file taken for this save
		/// </summary>
		public string snapshotFilePath
		{
			get
			{
				if (_snapshotFilePath == null)
				{
					_snapshotFilePath = StratusIO.ChangeExtension(filePath, snapshotEncoding.ToExtension());
				}
				return _snapshotFilePath;
			}
		}
		private string _snapshotFilePath;

		/// <summary>
		/// Whether an associated snapshot file is found for this save
		/// </summary>
		public bool snapshotExists => StratusIO.FileExists(snapshotFilePath);

		/// <summary>
		/// Whether a snapshot of this save has been loaded
		/// </summary>
		public bool snapshotLoaded => snapshot != null;

		/// <summary>
		/// The extension for the snapshot file
		/// </summary>
		public virtual StratusImageEncoding snapshotEncoding => StratusImageEncoding.JPG;

		/// <summary>
		/// Whether this save has been fully loaded
		/// </summary>
		public virtual bool loaded => true;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		public abstract void OnAfterDeserialize();
		public abstract void OnBeforeSerialize();
		protected abstract void OnDelete();

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		public override string ToString()
		{
			return $"{name}{(serialized ? $" ({filePath})" : string.Empty)}";
		}

		public virtual Dictionary<string, string> ComposeDetailedStringMap()
		{
			var details = new Dictionary<string, string>();
			details.Add(nameof(name), name);
			if (date.IsValid())
			{
				details.Add(nameof(date), date);
			}
			if (description.IsValid())
			{
				details.Add(nameof(description), description);
			}
			details.Add(nameof(playtime), $"{playtime.ToString()}m");
			return details;
		}

		/// <summary>
		/// Call this function after the save has been serialized externally
		/// </summary>
		public virtual void OnAfterSerialize()
		{
			if (!serialized)
			{
				this.Log("Not yet serialized");
				return;
			}

			if (snapshot != null)
			{
				SaveSnapshot();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked whenever this save is serialized/deserialized
		/// </summary>
		/// <param name="filePath"></param>
		public void OnAnySerialization(string filePath)
		{
			this.filePath = filePath;
			this.fileName = StratusIO.GetFileName(filePath);

			if (Application.isPlaying)
			{
				this.playtime += StratusTime.minutesSinceStartup;
			}
		}

		/// <summary>
		/// Deletes this save's files, if serialized previously
		/// </summary>
		/// <returns></returns>
		public bool DeleteSerialization()
		{
			if (!serialized)
			{
				return false;
			}

			if (!StratusIO.DeleteFile(filePath))
			{
				this.LogError($"Failed to delete save file at {filePath}");
				return false;
			}

			if (snapshotLoaded || snapshotExists)
			{
				StratusIO.DeleteFile(snapshotFilePath);
			}

			OnDelete();

			filePath = null;
			_snapshotFilePath = null;

			return true;
		}

		/// <summary>
		/// Loads this save, invoking the given action afterwards.
		/// </summary>
		/// <param name="onLoad"></param>
		/// <returns></returns>
		public virtual StratusValidation Load()
		{
			return true;
		}

		/// <summary>
		/// Loads this save asynchronously, invoking the given action afterwards.
		/// </summary>
		/// <param name="onLoad"></param>
		public virtual StratusValidation LoadAsync(Action onLoad)
		{
			onLoad?.Invoke();
			return true;
		}

		/// <summary>
		/// Loads the snapshot associated with this save, if present
		/// </summary>
		/// <returns></returns>
		public bool LoadSnapshot()
		{
			if (snapshotLoaded)
			{
				return true;
			}

			if (!snapshotExists)
			{
				return false;
			}

			snapshot = StratusIO.LoadImage2D(snapshotFilePath);
			return true;
		}

		/// <summary>
		/// Sets the snapshot to be associated with this save
		/// </summary>
		/// <returns></returns>
		public bool SetSnapshot(Texture2D snapshot)
		{
			if (snapshot == null)
			{
				this.LogError("No valid snapshot texture given");
				return false;
			}
			this.snapshot = snapshot;
			return true;
		}

		/// <summary>
		/// Saves the current snapshot, if there's one
		/// and this save has been already serialized
		/// </summary>
		/// <returns></returns>
		public bool SaveSnapshot()
		{
			if (snapshot == null)
			{
				this.LogError("No valid snapshot texture assigned");
				return false;
			}
			if (!serialized)
			{
				this.LogError("This save has not yet been serialized. Cannot save snapshot yet");
				return false;
			}
			return StratusIO.SaveImage2D(snapshot, snapshotFilePath, snapshotEncoding);
		}

		/// <summary>
		/// Unloads the snapshot associated with this save, if present
		/// </summary>
		/// <returns></returns>
		public void UnloadSnapshot()
		{
			UnityEngine.Object.Destroy(snapshot);
			snapshot = null;
		}

		/// <summary>
		/// Unloads all relevant data in memory for this save
		/// </summary>
		public virtual void Unload()
		{
			UnloadSnapshot();
		}

		//------------------------------------------------------------------------/
		// Disposable
		//------------------------------------------------------------------------/
		#region IDisposable Support
		public bool disposed { get; private set; }
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					Unload();
				}

				disposed = true;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}
		#endregion
	}

	/// <summary>
	/// Base class for saves that embed other data.
	/// By default, whenever this is serialized, so is the data
	/// When this is deserialized, the data is NOT loaded by default.
	/// This is due to the save itself being a manifest of sorts for the much larger data file.
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public abstract class StratusSave<DataType> : StratusSave
		where DataType : class, new()
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The data for this save
		/// </summary>
		public DataType data { get; private set; }

		/// <summary>
		/// The data serializer
		/// </summary>
		public static readonly StratusJSONSerializer<DataType> dataSerializer = new StratusJSONSerializer<DataType>();

		/// <summary>
		/// The file path for where the encapsulated data is saved to
		/// </summary>
		public string dataFilePath
		{
			get
			{
				if (_dataFilePath == null)
				{
					_dataFilePath = StratusIO.ChangeExtension(filePath, dataExtension);
				}
				return _dataFilePath;
			}
		}
		private string _dataFilePath;

		/// <summary>
		/// Whether a data file exists for this save
		/// </summary>
		public bool dataFileExists => StratusIO.FileExists(dataFilePath);

		/// <summary>
		/// The extension used for save data
		/// </summary>
		public virtual string dataExtension => ".savedata";

		/// <summary>
		/// Whether the data for the save is loaded
		/// </summary>
		public bool dataLoaded => data != null;

		public override bool loaded => dataLoaded;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSave(DataType data)
		{
			this.data = data;
		}

		public StratusSave()
		{
		}

		public override void OnAfterSerialize()
		{
			base.OnAfterSerialize();
			SaveData();
		}

		protected override void OnDelete()
		{
			if (dataFileExists)
			{
				StratusIO.DeleteFile(dataFilePath);
				_dataFilePath = null;
			}
		}

		public override StratusValidation Load()
		{
			return LoadData();
		}

		public override StratusValidation LoadAsync(Action onLoad)
		{
			return LoadDataAsync(onLoad);
		}

		public override void Unload()
		{
			base.Unload();
			UnloadData();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void ResetData()
		{
			SetData(new DataType());
		}

		public void SetData(DataType data)
		{
			this.data = data;
		}

		public StratusValidation LoadData()
		{
			if (dataLoaded)
			{
				return new StratusValidation(true, "Data already loaded");
			}

			if (!serialized)
			{
				return new StratusValidation(false, "Cannot load data before the save has been serialized");
			}

			try
			{
				data = dataSerializer.Deserialize(dataFilePath);
			}
			catch (Exception e)
			{
				return new StratusValidation(false, e.ToString());
			}

			if (data == null)
			{
				return new StratusValidation(false, $"Failed to deserialize data from {dataFilePath}");
			}

			return new StratusValidation(true, $"Loaded data file from {dataFilePath}");
		}

		public StratusValidation LoadDataAsync(Action onLoad)
		{
			if (!Application.isPlaying)
			{
				return new StratusValidation(false, "Cannot load data asynchronously outside of playmode...");
			}

			IEnumerator routine()
			{
				yield return new WaitForEndOfFrame();
				LoadData();
				onLoad?.Invoke();
			}

			StratusCoroutineRunner.Run(routine());
			return new StratusValidation(true, "Now loading data asynchronously...");
		}

		public void UnloadData()
		{
			data = null;
		}

		public bool SaveData()
		{
			if (!serialized)
			{
				this.LogError("Cannot load data before the save has been serialized");
				return false;
			}

			if (data == null)
			{
				this.LogError("No data to serialize! This could mean that this save was created yet no data previously assigned to it");
				return false;
			}

			this.Log($"Saving data to {dataFilePath}");
			dataSerializer.Serialize(data, dataFilePath);
			return true;
		}
	}
}