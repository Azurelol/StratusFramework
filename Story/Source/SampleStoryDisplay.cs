using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Stratus.Gameplay.Story.Examples
{
	/// <summary>
	/// A simple ink story display that displays one line at a time
	/// </summary>
	public class SampleStoryDisplay : StratusStoryDefaultDisplay
	{
		protected override string GetMessage(ParsedLine line)
		{
			var msg = line.Find("Speaker");
			return msg != null ? msg.value : string.Empty;
		}

		protected override string GetSpeaker(ParsedLine line)
		{
			var speaker = line.Find("Message");
			return speaker != null ? speaker.value : string.Empty;
		}

		protected override Sprite GetCharacterPortrait(string character, string tag = null)
		{
			return null;
		}
	}
}