using RPG.Units;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace RPG.Units
{
    [SaveData("move")]
    public abstract partial class UnitMoveComponent : UnitComponent, IStateRecorder
    {
        protected const float с_blendCoef = 2f;
        private bool _inSprint;
        private bool _inCrouch;

        public bool LockMovement { get; set; }

        public bool InSprint 
        {
            get => _inSprint;
            set
            {
                if (value && Owner.Move.LockMovement) return;
                _inSprint = value;
                Owner.Animator.SetBool("Sprint", _inSprint);

                if (_inSprint && _inCrouch) InCrouch = false;
            }
        }

        public bool InCrouch 
        {
            get => _inCrouch;
            set
            {
                if (value && Owner.Move.LockMovement) return;
                _inCrouch = value;
                Owner.Animator.SetBool("Crouch", _inCrouch);

                if (_inCrouch && _inSprint) InSprint = false;
            }
        }


        protected abstract void Update();

        public void UpdateAnimationStates(Vector3 currentVelocity)
        {
            var inversVelocity = transform.InverseTransformVector(currentVelocity / с_blendCoef);

            Owner.Animator.SetBool("Moving", currentVelocity.sqrMagnitude > с_blendCoef);
            Owner.Animator.SetFloat("MoveSpeed", Mathf.Clamp(currentVelocity.magnitude, 0.1f, 1f));

            Owner.Animator.SetFloat("ForwardMove", Mathf.Clamp(inversVelocity.z, -1f, 1f));
            Owner.Animator.SetFloat("SideMove", Mathf.Clamp(inversVelocity.x, -1f, 1f));
        }

        protected void OnSprint() => InSprint = !InSprint;

        protected void OnCrouch() => InCrouch = !InCrouch;

        protected void OnJump()
        {
            if (Owner.State.InAir) return;
            LockMovement = true;
            Owner.Animator.SetTrigger("Jump");
        }
    }
}