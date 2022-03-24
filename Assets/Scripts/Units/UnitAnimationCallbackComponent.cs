
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class UnitAnimationCallbackComponent : UnitComponent
    {
        private void OnAnimationStart_UnityEvent(AnimationEvent data)
        {
            Owner.Move.LockMovement = true;
		}

		private void OnAnimationEnd_UnityEvent(AnimationEvent data)
        {
            if (Owner.State.InAir) return;
            Owner.Move.LockMovement = false;
        }

        private void OnSwitchWeaponType_UnityEvent(AnimationEvent data)
        {
#if UNITY_EDITOR
            if (bool.TryParse(data.stringParameter, out var temp)) throw new System.ApplicationException("Failed in parsing string to bool");
#endif

            bool.TryParse(data.stringParameter, out var result);
            var type = (WeaponStyleType)data.intParameter;
            //Owner.Set.WeaponType = type;//todo test
        }

        private void OnCollider_UnityEvent(AnimationEvent data)
        {
            Owner.Set.ColliderStateChange(data.stringParameter, data.intParameter == 1);
        }
	}
}
