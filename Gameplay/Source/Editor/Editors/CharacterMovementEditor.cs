using UnityEditor;

namespace Stratus.Gameplay
{
	[CustomEditor(typeof(StratusCharacterMovement), true)]
	public class CharacterMovementEditor : StratusBehaviourEditor<StratusCharacterMovement>
	{
		protected override void OnStratusEditorEnable()
		{
			// Whether to show jump properties
			//string[] jumpProperties = new string[]
			//{
			//  nameof(CharacterMovement.jumpSpeed),
			//  nameof(CharacterMovement.jumpCurve),
			//  nameof(CharacterMovement.fallCurve),
			//  nameof(CharacterMovement.jumpApex),
			//  nameof(CharacterMovement.groundDetection),
			//  nameof(CharacterMovement.groundLayer),
			//  nameof(CharacterMovement.airControl),
			//};
			//AddConstraint(()=> target.supportsJump, jumpProperties);

			this.AddConstraint(() => this.target.supportsJump && this.target.hasGroundCast,
		nameof(StratusCharacterMovement.groundCollider),
		nameof(StratusCharacterMovement.groundCastFrequency));

			this.AddPropertyChangeCallback(nameof(StratusCharacterMovement.locomotion), this.OnLocomotionChange);
		}

		private void OnLocomotionChange()
		{
			this.target.SetComponents();
		}

	}

}