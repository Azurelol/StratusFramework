namespace Stratus.Gameplay
{
	/// <summary>
	/// A serialization of characters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusCharacterCollection<T>
		where T : StratusCharacter, new()
	{
		public T[] characters;

		public int characterCount => characters.LengthOrZero();
		public StratusSortedList<string, T> charactersByName
		{
			get
			{
				if (_membersByName == null)
				{
					_membersByName = new StratusSortedList<string, T>(x => x.name);
					_membersByName.AddRange(characters);
				}
				return _membersByName;
			}
		}
		private StratusSortedList<string, T> _membersByName;

		public T GetCharacter(string name)
		{
			if (!charactersByName.ContainsKey(name))
			{
				return null;
			}
			return charactersByName[name];
		}
	}

}