using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;

namespace Stratus
{
	/// <summary>
	/// Specifies what suffixes to add to the save file name
	/// </summary>
	public enum StratusSaveDataSuffixFormat
	{
		Incremental,
		SystemTime
	}

	/// <summary>
	/// A required attribute that specifies the wanted folder path and name for a savedata asset
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class StratusSaveDataAttribute : Attribute
	{
		/// <summary>
		/// The path inside the relative path where you want this data stored
		/// </summary>
		public string folder { get; set; }
		/// <summary>
		/// What naming convention to use for a save file of this type
		/// </summary>
		public string namingConvention { get; set; } // = typeof(SaveData).DeclaringType.Name;                                                 
		/// <summary>                                                 
		/// What suffix to use for a save file of this type
		/// </summary>
		public StratusSaveDataSuffixFormat suffix { get; set; } = StratusSaveDataSuffixFormat.Incremental;
		/// <summary>
		/// What extension format to use for the save file
		/// </summary>
		public string saveExtension { get; set; } = ".save";
		/// <summary>
		/// What extension format to use for the save data file
		/// </summary>
		public string saveDataExtension { get; set; } = ".savedata";

		public class MissingException : Exception
		{
			public MissingException(string className) : base("The class declaration for " + className + " is missing the [SaveData] attribute, which provides the path information needed in order to construct the asset.")
			{
				// Fill later?
				this.HelpLink = "http://msdn.microsoft.com";
				this.Source = "Exception_Class_Samples";
			}
		}
	}

	public interface IStratusSaveSystem
	{
		void RefreshSaveFiles();
		void ClearSaveFiles();
		void LoadAllSaves(bool force = false);
		int saveLimit { get; }
		bool HasSaveAtIndex(int index);
		bool unlimitedSaves { get; }
	}

	public interface IStratusSaveSystem<SaveType> : IStratusSaveSystem
		where SaveType : StratusSave, new()
	{
		StratusSortedList<int, SaveType> saves { get; }
		bool Save(SaveType save, string fileName);
		bool Save(SaveType save);
		void SaveAsync(SaveType save, Action onFinished);
		SaveType Load(string fileName, string folderName = null);
		SaveType GetSaveAtIndex(int index);
	}


	/// <summary>
	/// An abstract class for handling runtime save-data. Useful for player profiles, etc...
	/// </summary>
	public abstract class StratusSaveSystem<SingletonType, SaveType, SerializerType>
		: StratusSingleton<SingletonType>, IStratusSaveSystem<SaveType>
		where SingletonType : StratusSaveSystem<SingletonType, SaveType, SerializerType>
		where SaveType : StratusSave, new()
		where SerializerType : StratusSerializer<SaveType>, new()
	{
		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Serializer used
		/// </summary>

		private static readonly SerializerType serializer = new SerializerType();

		/// <summary>
		/// Whether the save data system is being debugged
		/// </summary>
		public abstract bool debug { get; }

		/// <summary>
		/// The attribute containing data about this class
		/// </summary>
		private static StratusSaveDataAttribute attribute //; { get; } = AttributeUtility.FindAttribute<SaveDataAttribute>(typeof(T));
		{
			get
			{
				if (attribute_ == null)
				{
					var type = typeof(SingletonType);
					attribute_ = type.GetAttribute<StratusSaveDataAttribute>();
					if (attribute_ == null)
						throw new StratusSaveDataAttribute.MissingException(type.Name);
				}
				return attribute_;
			}
		}
		private static StratusSaveDataAttribute attribute_;

		/// <summary>
		/// The default naming convention
		/// </summary>
		public static string namingConvention { get; } = attribute.GetProperty<string>(nameof(StratusSaveDataAttribute.namingConvention));

		/// <summary>
		/// The folder inside the relative path to this asset
		/// </summary>
		public static string folder { get; } = attribute.GetProperty<string>(nameof(StratusSaveDataAttribute.folder));

		/// <summary>
		/// The extension used by the save file
		/// </summary>
		public static string saveExtension { get; } = attribute.GetProperty<string>(nameof(StratusSaveDataAttribute.saveExtension));

		/// <summary>
		/// The extension used by this save data
		/// </summary>
		public static StratusSaveDataSuffixFormat suffix { get; } = attribute.GetProperty<StratusSaveDataSuffixFormat>(nameof(StratusSaveDataAttribute.suffix));

		/// <summary>
		/// The path to the directory being used by this save data, as specified by the [SaveData] attribute
		/// </summary>
		public static string saveDirectoryPath => StratusIO.CombinePath(rootSaveDirectoryPath, folder);

		/// <summary>
		/// The persistent data path that Unity is using
		/// </summary>
		public static string rootSaveDirectoryPath => Application.persistentDataPath;

		/// <summary>
		/// Returns all instances of the save data from the path
		/// </summary>
		public string[] saveFilePaths
		{
			get
			{
				if (_saveFilePaths == null)
				{
					RefreshSaveFiles();
				}
				return _saveFilePaths;
			}
		}
		private string[] _saveFilePaths;

		/// <summary>
		/// Loaded saves
		/// </summary>
		public StratusSortedList<int, SaveType> saves { get; private set; } 

		/// <summary>
		/// Whether saves have been already loaded
		/// </summary>
		public bool savesLoaded { get; private set; }

		/// <summary>
		/// The number of save files present in the specified folder for save data
		/// </summary>
		public int saveCount => saveFilePaths != null ? saveFilePaths.Length : 0;

		/// <summary>
		/// The currently loaded save
		/// </summary>
		public SaveType latestSave { get; private set; }

		/// <summary>
		/// The character used to separate directories in Unity
		/// </summary>
		public static char directorySeparatorChar { get; } = '/';

		/// <summary>
		/// Default parameters for time stamps
		/// </summary>
		public static readonly StratusTimestampParameters timestampParameters = new StratusTimestampParameters();

		/// <summary>
		/// The maximum amount of saves allowed. If 0, the saves are unlimited.
		/// </summary>
		public virtual int saveLimit => 100;

		/// <summary>
		/// If there's unlimited saving allowed
		/// </summary>
		public bool unlimitedSaves => saveLimit == 0;

		//--------------------------------------------------------------------------------------------/
		// Utility Events
		//--------------------------------------------------------------------------------------------/
		public event Action<SaveType> onSaveAsyncStarted;
		public event Action<SaveType> onSaveAsyncEnded;

		//--------------------------------------------------------------------------------------------/
		// Utility Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Composes a default save data name
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ComposeDefaultName(string namingConvention, StratusSaveDataSuffixFormat format, int count)
		{
			string name = namingConvention;
			switch (format)
			{
				case StratusSaveDataSuffixFormat.Incremental:
					name += "_" + count;
					break;
				default:
					break;
			}
			return name;
		}

		/// <summary>
		/// Generates an unique save file name based on naming convention and existing saves
		/// </summary>
		/// <returns></returns>
		public string GenerateSaveFileName()
		{
			string result = namingConvention;
			switch (suffix)
			{
				case StratusSaveDataSuffixFormat.Incremental:
					RefreshSaveFiles();
					result += $"{saveCount}";
					break;
				case StratusSaveDataSuffixFormat.SystemTime:
					result += $"{StratusIO.GetTimestamp(timestampParameters)}";
					break;
			}
			return result;
		}

		/// <summary>
		/// Generates a save name for this save. By default it will be the time stamp
		/// </summary>
		/// <returns></returns>
		protected virtual string GenerateSaveName()
		{
			return StratusIO.GetTimestamp();
		}

		//--------------------------------------------------------------------------------------------/
		// Abstract
		//--------------------------------------------------------------------------------------------/
		protected override void OnInitialize()
		{
			saves = new StratusSortedList<int, SaveType>(x => x.index);
			RefreshSaveFiles();
			OnSaveSystemInitialize();
		}

		protected abstract void OnSaveSystemInitialize();
		protected abstract void OnCreateSave(SaveType save);
		protected abstract void OnBeforeSave(SaveType save);

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Searches for all recognized save files in the default path
		/// </summary>
		public void RefreshSaveFiles()
		{
			if (!Directory.Exists(saveDirectoryPath))
			{
				return;
			}

			// Look at the files matching the extension in the given folder
			List<string> saves = new List<string>();
			var files = Directory.GetFiles(saveDirectoryPath);
			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i];
				string fileExtension = Path.GetExtension(file);
				if (fileExtension == saveExtension)
				{
					saves.Add(file);
				}
			}
			_saveFilePaths = saves.ToArray();
			if (debug)
			{
				if (saveCount > 0)
				{
					this.Log($"Found {saveCount} save files at {saveDirectoryPath}!");
				}
				else
				{
					this.Log($"Found no save files at {saveDirectoryPath}...");
				}
			}
		}

		/// <summary>
		/// Loads all save files, building a dictionary of them
		/// by their save name
		/// </summary>
		public void LoadAllSaves(bool force = false)
		{
			if (savesLoaded && !force)
			{
				return;
			}

			saves.Clear();
			foreach (var saveFilePath in saveFilePaths)
			{
				SaveType save = Load(saveFilePath);
				if (save != null)
				{
					if (save.hasValidIndex)
					{
						if (debug)
						{
							this.Log($"Adding save at index {save.index}");
						}
						saves.Add(save.index, save);
					}
					else
					{
						this.LogError($"Invalid index for save at {save}");
					}
				}
			}
			if  (debug)
			{
				this.Log($"Loaded {saves.Count} saves...");
			}
			savesLoaded = true;
		}

		/// <summary>
		/// Clears all found save files (searches for them first)
		/// </summary>
		public void ClearSaveFiles()
		{
			RefreshSaveFiles();
			if (saveFilePaths.IsValid())
			{
				for (int i = 0; i < saveFilePaths.Length; i++)
				{
					string file = saveFilePaths[i];
					StratusIO.DeleteFile(file);
				}
				if (debug)
				{
					this.Log($"Deleted {saveFilePaths.Length} save files!");
				}
			}
			else if (debug)
			{
				this.LogWarning("No save files were found to be deleted.");
			}
		}

		/// <summary>
		/// Reveals the default save file directory path
		/// </summary>
		public void RevealSaveFiles()
		{
			StratusIO.Open(saveDirectoryPath);
		}

		/// <summary>
		/// Returns true if there's a save at the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool HasSaveAtIndex(int index)
		{
			LoadAllSaves();
			return saves.ContainsKey(index);
		}

		/// <summary>
		/// Returns the save at the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SaveType GetSaveAtIndex(int index)
		{
			if (!savesLoaded)
			{
				this.LogError("No saves loaded yet");
				return null;
			}
			return saves[index];
		}

		/// <summary>
		/// Creates a save with the given name on a generated file.
		/// This calls derived classes construction, so its best
		/// to create saves purely through this method
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SaveType CreateSave(Action<SaveType> onCreated = null)
		{
			SaveType save = new SaveType();
			OnCreateSave(save);
			onCreated?.Invoke(save);
			return save;
		}

		/// <summary>
		/// Saves the data to the default path in the application's persistent path
		/// using the specified name
		/// </summary>
		public bool Save(SaveType save, string fileName = null)
		{
			// No name provided
			if (save.name == null)
			{
				save.name = GenerateSaveName();
			}

			// No filename provided
			if (fileName.IsNullOrEmpty())
			{
				fileName = GenerateSaveFileName();
				if (debug)
				{
					this.Log($"Generated save filename {fileName}");
				}
			}

			// Create the directory if missing
			if (!Directory.Exists(saveDirectoryPath))
			{
				Directory.CreateDirectory(saveDirectoryPath);
			}

			// Compose the save file path
			string filePath = ComposePath(fileName);

			// Invoke a function before saving
			OnBeforeSave(save);

			// Now serialize the save
			Serialize(save, filePath);
			if (debug)
			{
				StratusDebug.Log($"Saved data at path {filePath}");
			}

			latestSave = save;
			return true;
		}

		/// <summary>
		/// Saves new data with a generated save name
		/// </summary>
		/// <param name="saveData"></param>
		public bool Save(SaveType saveData)
		{
			if (!saveData.hasValidIndex)
			{
				this.LogError("No index has been assigned for this save");
				return false;
			}

			if (saveData.serialized)
			{
				return Save(saveData, saveData.fileName);
			}

			// If not serialized
			string fileName = GenerateSaveFileName();
			if (debug)
			{
				this.Log($"Generated save filename {fileName}");
			}

			return Save(saveData, fileName);
		}

		/// <summary>
		/// Saves data asynchronously, at runtime
		/// </summary>
		/// <param name="save"></param>
		public void SaveAsync(SaveType save, Action onFinished)
		{
			if (!Application.isPlaying)
			{
				this.LogError("Cannot save asynchronously outside of playmode...");
				return;
			}

			IEnumerator routine()
			{
				if (debug)
				{
					this.Log($"Asynchronous save on {save} started");
				}
				onSaveAsyncStarted?.Invoke(save);
				yield return new WaitForEndOfFrame();
				Save(save);
				onFinished?.Invoke();
				onSaveAsyncEnded?.Invoke(save);
				if (debug)
				{
					this.Log($"Asynchronous save on {save} ended");
				}
			}

			StratusCoroutineRunner.Run(routine());
		}

		/// <summary>
		/// Saves the current save data
		/// </summary>
		public bool SaveCurrent()
		{
			if (latestSave != null)
			{
				if (latestSave.serialized)
				{
					if (debug)
					{
						StratusDebug.Log($"Saved latest save...");
					}
					Save(latestSave);
				}
			}
			return false;
		}

		/// <summary>
		/// Loads a save data file with a given name from the given folder and returns it
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public SaveType Load(string fileName, string folderName = null)
		{
			string filePath = ComposePath(fileName, folderName);
			SaveType save = Deserialize(filePath);
			if (save == null)
			{
				this.LogError($"Failed to load save from {filePath}");
			}
			else
			{
				if (debug)
				{
					this.Log($"Loaded save from {fileName} at '{filePath}'");
				}
				latestSave = save;
			}
			return save;
		}

		///// <summary>
		///// Loads save data from the given file, generates if not present
		///// </summary>
		///// <param name="fileName"></param>
		///// <param name="folderName"></param>
		///// <returns></returns>
		//public SaveType LoadOrCreate(string fileName, string folderName = null)
		//{
		//	string filePath = ComposePath(fileName, folderName);
		//	if (StratusIO.FileExists(filePath))
		//	{
		//		return Load(fileName, folderName);
		//	}
		//	if (debug)
		//	{
		//		StratusDebug.Log($"Creating data at path {filePath}");
		//	}
		//	return CreateSave(null, fileName);
		//}

		/// <summary>
		/// Deletes the save file at the default folder
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="folderName"></param>
		public bool Delete(string fileName, string folderName = null)
		{
			if (!Exists(fileName, saveDirectoryPath))
				return false;

			var fileNameExt = fileName + saveExtension;
			var filePath = saveDirectoryPath + fileNameExt;
			StratusIO.DeleteFile(ComposePath(fileName, folderName));
			File.Delete(filePath);
			if (debug)
			{
				StratusDebug.Log($"Deleted data at path {filePath}");
			}
			return true;
		}

		/// <summary>
		/// Deletes the save's serialization (its files in disk)
		/// </summary>
		/// <param name="save"></param>
		/// <returns></returns>
		public bool Delete(SaveType save)
		{
			return save.DeleteSerialization();
		}

		/// <summary>
		/// Performs the serialization operation
		/// </summary>
		/// <param name="saveData"></param>
		/// <param name="filePath"></param>
		private void Serialize(SaveType saveData, string filePath)
		{
			//saveData.Serialize(filePath, serializer);
			// Update the time to save at
			saveData.date = DateTime.Now.ToString();
			// Call a customized function before writing to disk
			saveData.OnBeforeSerialize();
			// Write to disk
			serializer.Serialize(saveData, filePath);
			// Note that it has been saved
			saveData.OnAnySerialization(filePath);
			saveData.OnAfterSerialize();
		}

		/// <summary>
		/// Performs the deserialization operation
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>

		public static SaveType Deserialize(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException("The file was not found!");

			SaveType saveData = serializer.Deserialize(filePath);
			saveData.OnAfterDeserialize();
			saveData.OnAnySerialization(filePath);
			return saveData;
		}

		public static string ComposePath(string fileName, string folder = null)
		{
			if (folder != null)
			{
				return StratusIO.CombinePath(saveDirectoryPath, folder, StratusIO.ChangeExtension(fileName, saveExtension));
			}
			return StratusIO.CombinePath(saveDirectoryPath, StratusIO.ChangeExtension(fileName, saveExtension));
		}

		public static bool Exists(string fileName, string folder = null) => StratusIO.FileExists(ComposePath(fileName, folder));





	}

}