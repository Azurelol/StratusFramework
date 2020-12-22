using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;

namespace Stratus.Gameplay
{
	public class Stamina
	{
		public class ModifiedEvent : ValueEvent<float> { }
		public StratusVariableAttribute value;
		public float recoveryDelay;
	}

	public class StratusStaminaModule<Resource> : StratusCombatControllerModule
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private StratusCooldown staminaRecoveryCooldown;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The stamina of this character, consumed when activating skills and abilities
		/// </summary>
		public Stamina stamina { get; private set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnInitialize()
		{
			this.gameObject.Connect<Stamina.ModifiedEvent.GainEvent>(this.OnGainStaminaEvent);
			this.gameObject.Connect<Stamina.ModifiedEvent.LossEvent>(this.OnConsumeStaminaEvent);

			this.staminaRecoveryCooldown = new StratusCooldown(this.stamina.recoveryDelay);
		}

		public override void OnTimeStep(float step)
		{
			this.staminaRecoveryCooldown.Update(Time.deltaTime);
			Gain(step);
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Received when the character has consumed stamina
		/// </summary>
		/// <param name="e"></param>
		private void OnConsumeStaminaEvent(Stamina.ModifiedEvent.LossEvent e)
		{
			this.Consume(e.value);
		}

		/// <summary>
		/// Received when the character has gained stamina
		/// </summary>
		/// <param name="e"></param>
		private void OnGainStaminaEvent(Stamina.ModifiedEvent.GainEvent e)
		{
			this.Gain(e.value);
		}

		///// <summary>
		///// Received when a skill is being validated (if it can be performed)
		///// </summary>
		///// <param name="e"></param>
		//private void OnSkillValidateEvent(Skill.ValidateEvent e)
		//{
		//  if (this.stamina.value.current > e.skill.cost)
		//    e.valid = true;
		//}
		//
		///// <summary>
		///// Received wwhen a skill has been activated
		///// </summary>
		///// <param name="e"></param>
		//private void OnSkillActivationEvent(Skill.ActivationEvent e)
		//{
		//  this.stamina.value.Reduce(e.skill.cost);
		//}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Gains stamina
		/// </summary>
		/// <param name="value"></param>
		private void Gain(float value)
		{
			// Regain stamina, if not at maximum
			if (stamina.value.isAtMaximum)
				return;

			// Do not regain stamina while the stamina recovery is on cooldown
			if (this.staminaRecoveryCooldown.isActive)
				return;

			//Trace.Script("Gaining " + value + " stamina", this);
			this.stamina.value.Increase(value);
			//this.controller.gameObject.Dispatch<Stamina.ModifiedEvent>(new Stamina.ModifiedEvent() { value = value });
		}

		/// <summary>
		/// Consumes stamina. Whenever it does, reset the stamina recovery timer.
		/// </summary>
		/// <param name="value"></param>
		private void Consume(float value)
		{
			this.stamina.value.Decrease(value);
			this.staminaRecoveryCooldown.Activate();
			//this.controller.gameObject.Dispatch<Stamina.ModifiedEvent>(new Stamina.ModifiedEvent() { value = value });
		}

		/// <summary>
		/// Whether there's enough stamina to perform the action
		/// </summary>
		/// <param name="cost"></param>
		/// <returns></returns>
		public bool HasEnoughStamina(float cost)
		{
			return this.stamina.value.current >= cost;
		}


	}

}