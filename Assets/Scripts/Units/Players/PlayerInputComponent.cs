using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace RPG.Units.Player
{
    public class PlayerInputComponent : UnitInputComponent
    {
		[Inject]
		private PlayerControls _controls;

		public SimpleHandle ArmedEventHandler;
		public SimpleHandle RangeSetEventHandler;
		public SimpleHandle SprintEventHandler;
		public SimpleHandle CrouchEventHandler;
		public SimpleHandle JumpEventHandler;
		public SimpleHandle Skill1EventHandler;

		private void Update()
		{
			var direction = _controls.Unit.Move.ReadValue<Vector2>();
			_movement = new Vector3(direction.x, 0f, direction.y);
		}

		protected override void Awake()
		{
			base.Awake();
			_controls.Unit.MainAction.performed += (q) => CallSimpleHandle(nameof(MainEventHandler));
			_controls.Unit.LockTarget.performed += (q) => CallSimpleHandle(nameof(TargetEventHandler));
			_controls.Unit.AdditionalAction.performed += (q) => CallSimpleHandle(nameof(AdditionalEventHandler));
			_controls.Unit.Armed.performed += (q) => CallSimpleHandle(nameof(ArmedEventHandler));
			_controls.Unit.Sprint.performed += (q) => CallSimpleHandle(nameof(SprintEventHandler));
			_controls.Unit.Crouch.performed += (q) => CallSimpleHandle(nameof(CrouchEventHandler));
			_controls.Unit.Jump.performed += (q) => CallSimpleHandle(nameof(JumpEventHandler));
			_controls.Unit.Skill1.performed += (q) => CallSimpleHandle(nameof(Skill1EventHandler));
		}
	}
}