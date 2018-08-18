/******************************************************************************/
/*!
@file   PlayerParty.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Altostratus
{
  /// <summary>
  /// Data about the player's party.
  /// </summary>
  public class PlayerParty : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class ChangeCombatLeadEvent : Stratus.Event
    {
      public enum ChangeOrder { Previous, Next }
      public ChangeOrder order;
      public Character lead;
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
      public Character character;
      public bool active = true;

      public string name { get { return character.name; } }

      public PartyMember(Character character)
      {
        // Instantiate a copy of the character for progression?
        this.character = ScriptableObject.Instantiate(character);
      }
    }

    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    public Party party;
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
    public static PlayerParty current;

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
      PlayerParty.current = this;
      // Subscribe to events
      this.gameObject.Connect<Combat.StartedEvent>(this.OnCombatStartedEvent);
      this.gameObject.Connect<Combat.EndedEvent>(this.OnCombatEndedEvent);
    }

    private void OnDestroy()
    {
      PlayerParty.current = null;
    }

    void OnCombatStartedEvent(Combat.StartedEvent e)
    {

    }

    void OnCombatEndedEvent(Combat.EndedEvent e)
    {
      //this.gameObject.GetComponent<PlayerCamera>().Follow();
    }

    /// <summary>
    /// Imports the selected party to be the Player's party.
    /// </summary>
    public void Import()
    {
      if (party == null)
        Trace.Error("No party was set!", this, true);

      members.Clear();

      // Make a copy of the party
      foreach (var member in party.Members)
      {
        var partyMember = new PartyMember(member);
        Trace.Script("Added " + member.name, this);
        members.Add(partyMember);
      }

      Trace.Script("Imported '" + party.name + "' with the following members: " + members, this);
    }

    /// <summary>
    /// Adds a member to the party.
    /// </summary>
    /// <param name="partyMember">The character to add to the player party. </param>    
    public void Add(Character character)
    {
      members.Add(new PartyMember(character));
      if (debug) Trace.Script(character.name + " has joined the party!");
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