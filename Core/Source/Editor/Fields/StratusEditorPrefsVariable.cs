using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
	/// <summary>
	/// A variable using EditorPrefs
	/// </summary>
	public class StratusEditorPrefsVariable : StratusPrefsVariable
	{
		public StratusEditorPrefsVariable(string key, VariableType type) : base(key, type)
		{
		}

		public StratusEditorPrefsVariable(System.Type key, VariableType type) : base(key, type)
		{
		}

		public override void Delete()
		{
			EditorPrefs.DeleteKey(key);
		}

		public override bool GetBool()
		{
			return EditorPrefs.GetBool(key);
		}

		public override float GetFloat()
		{
			return EditorPrefs.GetFloat(key);
		}

		public override int GetInt()
		{
			return EditorPrefs.GetInt(key);
		}

		public override string GetString()
		{
			return EditorPrefs.GetString(key);
		}

		public override void SetBool(bool value)
		{
			EditorPrefs.SetBool(key, value);
		}

		public override void SetFloat(float value)
		{
			EditorPrefs.SetFloat(key, value);
		}

		public override void SetInt(int value)
		{
			EditorPrefs.SetInt(key, value);
		}

		public override void SetString(string value)
		{
			EditorPrefs.SetString(key, value);
		}
	}

}