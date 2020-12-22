using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Data about the player's party.
	/// </summary>
	public class StratusPlayerPartyBehaviour : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public class ChangeCombatLeadEvent : Stratus.StratusEvent
		{
			public enum ChangeOrder { Previous, Next }
			public ChangeOrder order;
			public StratusCharacterScriptable lead;
		}

		//------------------------------------------------------------------------/
		// Subclasses
		//------------------------------------------------------------------------/
		/// <summary>
		/// Represents an instance of a character that's been instantiated
		/// for combat and progression.
		/// </summary> 
		[Serializable]
		public class PartyMember : StratusSerializable
		{
			public StratusCharacterScriptable character;
			public bool active = true;

			public string name { get { return character.name; } }

			public PartyMember(StratusCharacterScriptable character)
			{
				// Instantiate a copy of the character for progression?
				this.character = ScriptableObject.Instantiate(character);
			}
		}

		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		public StratusParty party;
		public List<PartyMember> members = new List<PartyMember>();
		public int combatRosterLimit = 4;
		public bool debug = false;

		private int leadIndex = 0;

		//----------------------------------------------------------------------/
		// Properties
		//----------------------------------------------------------------------/
		/// <summary>
		/// The current player party
		/// </summary>
		public static StratusPlayerPartyBehaviour current;

		/// <summary>
		/// The party members currently selected for combat.
		/// </summary>
		public List<PartyMember> combatRoster
		{
			get
			{
				var combatRoster = new List<PartyMember>();
				// Returns however many entries we can
				var limit = Mathf.Min(combatRosterLimit, members.Count);
				for (int i = 0; i < limit; ++i)
				{
					combatRoster.Add(members[i]);
				}
				return combatRoster;
			}
		}

		/// <summary>
		/// Returns the current leader (the first party member)
		/// </summary>
		public PartyMember lead
		{
			get
			{
				if (members.Count > 0)
					return members[0];
				return null;
			}
		}

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
		/// <summary>
		/// Initializes the PlayerParty.
		/// </summary>
		void Awake()
		{
			StratusPlayerPartyBehaviour.current = this;
			// Subscribe to events
			this.gameObject.Connect<StratusCombat.StartedEvent>(this.OnCombatStartedEvent);
			this.gameObject.Connect<StratusCombat.EndedEvent>(this.OnCombatEndedEvent);
		}

		private void OnDestroy()
		{
			StratusPlayerPartyBehaviour.current = null;
		}

		void OnCombatStartedEvent(StratusCombat.StartedEvent e)
		{

		}

		void OnCombatEndedEvent(StratusCombat.EndedEvent e)
		{
			//this.gameObject.GetComponent<PlayerCamera>().Follow();
		}

		/// <summary>
		/// Imports the selected party to be the Player's party.
		/// </summary>
		public void Import()
		{
			if (party == null) 
			{ 
				StratusDebug.LogErrorBreak("No party was set!", this);
				return;
			}


			members.Clear();

			// Make a copy of the party
			foreach (var member in party.Members)
			{
				var partyMember = new PartyMember(member);
				StratusDebug.Log("Added " + member.name, this);
				members.Add(partyMember);
			}

			StratusDebug.Log("Imported '" + party.name + "' with the following members: " + members, this);
		}

		/// <summary>
		/// Adds a member to the party.
		/// </summary>
		/// <param name="partyMember">The character to add to the player party. </param>    
		public void Add(StratusCharacterScriptable character)
		{
			members.Add(new PartyMember(character));
			if (debug) StratusDebug.Log(character.name + " has joined the party!");
		}

		/// <summary>
		/// Changes the current lead character in combat.
		/// </summary>
		/// <param name="order">In what order to change.</param>
		/// <returns></returns>
		public PartyMember ChangeLead(ChangeCombatLeadEvent.ChangeOrder order)
		{
			if (order == ChangeCombatLeadEvent.ChangeOrder.Next)
			{
				leadIndex++;
				leadIndex = leadIndex % combatRoster.Count;
			}
			else if (order == ChangeCombatLeadEvent.ChangeOrder.Previous)
			{
				leadIndex--;
				if (leadIndex < 0) leadIndex = combatRoster.Count - 1;
			}

			var lead = combatRoster[leadIndex];
			return lead;
		}

	}

}