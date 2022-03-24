using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace RPG.Units.Player
{
	public class PlayerMoveComponent : UnitMoveComponent
	{
        private Vector3 _currentVelocity;

        [Inject]
        private CameraComponent _camera;

        [SerializeField, Range(1f, 10f), Tooltip("Скорость затухания инерции")]
        private float _velocityDamping = 3f;


        protected override void Update()
		{
            ref var movement = ref Owner.Input.MoveDirection;

            //РАСЧЕТ ПЕРЕМЕЩЕНИЯ
            if (movement.x == 0f && movement.z == 0f || LockMovement)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, _velocityDamping * TimeAssistant.GameDeltaTime);
            }
            else
            {
                var acceleration = _camera.transform.TransformDirection(movement) * TimeAssistant.GameDeltaTime * (InSprint
                    ? Owner.State.Parameters[StatsType.SprintAcceleration]
                    : Owner.State.Parameters[StatsType.MoveAcceleration]);

                _currentVelocity = Vector3.ClampMagnitude(_currentVelocity + acceleration, InSprint ? Owner.State.Parameters[StatsType.SprintVelocity] : Owner.State.Parameters[StatsType.MoveVelocity]);
            }

            //ПЕРЕМЕЩЕНИЕ
            transform.position += _currentVelocity * TimeAssistant.GameDeltaTime;

            //КАМЕРА
            if (Owner.Target != null)
            {
                transform.rotation = Quaternion.Euler(0f, _camera.PivotTransform.eulerAngles.y, 0f);
            }
            else if (movement.x == 0f)
            {
                if (movement.z > 0f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f), Owner.State.Parameters[StatsType.RotateSpeed] * Time.deltaTime);
                else if (movement.z < 0f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f), Owner.State.Parameters[StatsType.RotateSpeed] / 2f * Time.deltaTime);
            }

            //Отключаем спринт, если персонаж не бежит вперед
            if ((movement.z <= 0f || movement.x != 0f) && InSprint) OnSprint();

            if (LockMovement) return;

            UpdateAnimationStates(_currentVelocity);
            Owner.Animator.SetFloat("SprintSpeed", Mathf.Clamp(_currentVelocity.sqrMagnitude, 0.7f, 1.4f));
        }

        public void Binding(bool unbind = false)
        {
            var inputs = (PlayerInputComponent)Owner.Input;
            if (unbind)
            {
                inputs.SprintEventHandler -= OnSprint;
                inputs.CrouchEventHandler -= OnCrouch;
                inputs.JumpEventHandler -= OnJump;
                return;
            }

            inputs.SprintEventHandler += OnSprint;
            inputs.CrouchEventHandler += OnCrouch;
            inputs.JumpEventHandler += OnJump;
        }
    }
}
