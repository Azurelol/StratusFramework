using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Stratus
{
	public static partial class StratusEditorUtility
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static Assembly audioImporterAssembly { get; } = typeof(AudioImporter).Assembly;
		public static Type audioUtilityClass { get; } = audioImporterAssembly.GetType("UnityEditor.AudioUtil");
		private static MethodInfo playAudioClipMethod { get; } = audioUtilityClass.GetMethod(
				"PlayClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] {	typeof(AudioClip)},
				null);

		private static MethodInfo stopAllAudioClipsMethod = audioUtilityClass.GetMethod(
				"StopAllClips",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] { },
				null);


		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// Plays the given audio clip within the editor
		/// </summary>
		/// <param name="clip"></param>
		public static void PlayAudioClip(AudioClip clip)
		{
			playAudioClipMethod.Invoke(null, new object[] { clip} );
		}

		/// <summary>
		/// Stops all playing audio clips in the editor
		/// </summary>
		public static void StopAllAudioClips()
		{
			stopAllAudioClipsMethod.Invoke(null, new object[] { });
		}
	}
}