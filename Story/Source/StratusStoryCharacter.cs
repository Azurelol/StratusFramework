using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay.Story
{
	[Serializable]
	public class StratusStoryCharacterPortrait
	{
		public string label;
		public Sprite sprite;
	}

	[Serializable]
	public class StratusStoryCharacter
	{
		[SerializeField] private string _name;
		[SerializeField] private Sprite defaultPortrait;
		[SerializeField] private List<StratusStoryCharacterPortrait> portraits;
		[SerializeField] private Font _font;

		public string name => _name;
		public Font font => _font;

		private StratusSortedList<string, StratusStoryCharacterPortrait> portraitsByTag
		{
			get
			{
				if (_portraitsByTag == null)
				{
					_portraitsByTag = new StratusSortedList<string, StratusStoryCharacterPortrait>(x => x.label, portraits.Count);
					_portraitsByTag.AddRange(portraits);
				}
				return _portraitsByTag;
			}
		}
		private StratusSortedList<string, StratusStoryCharacterPortrait> _portraitsByTag;

		public Sprite GetPortrait(string label = null)
		{
			if (label.IsNullOrEmpty())
			{
				return defaultPortrait;
			}
			if (!portraitsByTag.ContainsKey(label))
			{
				return defaultPortrait;
			}
			return portraitsByTag[label].sprite;
		}
	}

	[Serializable]
	public class StratusStoryCharacterCollection<StoryCharacter>
		where StoryCharacter : StratusStoryCharacter, new()
	{
		[SerializeField]
		private Sprite defaultPortrait;
		[SerializeField]
		private List<StoryCharacter> characters = new List<StoryCharacter>();

		public StratusSortedList<string, StoryCharacter> charactersByName
		{
			get
			{
				if (_charactersByName == null)
				{
					_charactersByName = new StratusSortedList<string, StoryCharacter>(x => x.name, characters.Count);
					_charactersByName.AddRange(characters);
				}
				return _charactersByName;
			}
		}
		private StratusSortedList<string, StoryCharacter> _charactersByName;

		public Sprite GetPortrait(string character, string label = null)
		{
			if (!charactersByName.ContainsKey(character))
			{
				return defaultPortrait;
			}
			return charactersByName[character].GetPortrait(label);
		}
	}

	[Serializable]
	public class StratusStoryCharacterCollection 
		: StratusStoryCharacterCollection<StratusStoryCharacter>
	{
	}

}