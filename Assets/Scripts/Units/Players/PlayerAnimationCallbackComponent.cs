using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class PlayerAnimationCallbackComponent : UnitAnimationCallbackComponent
    {
		private void Jump_UnityEvent(AnimationEvent @event)
        {
            Owner.RigidBody.AddForce(Vector3.up * Owner.State.Parameters[StatsType.ForceJump], ForceMode.Impulse);
        }
    }
}
