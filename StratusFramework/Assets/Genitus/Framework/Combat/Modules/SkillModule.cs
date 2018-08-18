//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Stratus;

//namespace Genitus
//{
//  [SerializeField]
//  public class SkillModule<Resource> : CombatControllerModule 
//    where Resource : Character.ResourceModel    
//  {
//    //----------------------------------------------------------------------/
//    // Fields
//    //----------------------------------------------------------------------/
//    private Dictionary<Skill, SkillInstance> skillsMap = new Dictionary<Skill, SkillInstance>();
//    private static Skill.ValidateEvent validationEvent = new Skill.ValidateEvent();

//    //----------------------------------------------------------------------/
//    // Properties
//    //----------------------------------------------------------------------/
//    public SkillInstance defaultSkill { get; private set; }
//    public List<SkillInstance> skills { get; private set; } = new List<SkillInstance>();

//    //----------------------------------------------------------------------/
//    // Messages
//    //----------------------------------------------------------------------/
//    public override void OnTimeStep(float step)
//    {
//      foreach (var skill in this.skills)
//      {
//        //skill.resource.Upd
//        //skill.ReduceCooldown(step);
//      }
//    }

//    protected override void OnInitialize()
//    {      
//    }

//    //----------------------------------------------------------------------/
//    // Methods
//    //----------------------------------------------------------------------/
//    /// <summary>
//    /// Sets the available skills for this module
//    /// </summary>
//    /// <param name="attackSkill"></param>
//    public void Set(Character character)
//    {
//      this.skills = new List<SkillInstance>();
//      this.skillsMap = new Dictionary<Skill, SkillInstance>();

//      //foreach(var skill in character.skills)
//      //{
//      //  this.Add(skill);
//      //}
//      //this.defaultSkill = new SkillInstance(character.defaultSkill);
//    }

//    /// <summary>
//    /// Adds a skill.
//    /// </summary>
//    /// <param name="skill"></param>
//    public void Add(Skill skill)
//    {
//      //SkillInstance instance = new SkillInstance(skill);
//      //this.skills.Add(instance);
//      //this.skillsMap.Add(skill, instance);
//    }

//    /// <summary>
//    /// Validates whether a skill can be cast, by checking both its cooldown and
//    /// whether the combatcontroller has enough stamina to cast it.
//    /// </summary>
//    /// <param name="skill">The skill which to validate.</param>
//    /// <param name="user">The caster of the skill.</param>
//    /// <returns>True if the skill is both present and can be cast by the user,
//    ///          other wise false.</returns>
//    public bool Validate(CombatController user, Skill skill)
//    {
//      SkillInstance skillInstance = skillsMap[skill];

//      // If the skill could not be found or is on cooldown
//      //if (skillInstance == null || skillInstance.onCooldown)
//      //  return false;

//      // Now check whether the user can use the skill, by dispatching an event
//      // If there's something to validate that it can be used, it will be changed      
//      validationEvent.valid = false;
//      this.controller.gameObject.Dispatch<Skill.ValidateEvent>(validationEvent);
//      // By this point the listener will have changed this value (design patterns ho!)
//      return validationEvent.valid;
//    }
    

//    /// <summary>
//    /// Prints all available skills to the console.
//    /// </summary>
//    public void Print()
//    {
//      foreach (var skill in skills)
//      {
//        Trace.Script(skill.data.name + " is available!");
//      }
//    }
//  }

//}