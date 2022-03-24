using RPG.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class UnitTriggerComponent : BaseTriggerComponent
    {
        public SimpleHandle<Collider, bool> OnTriggerCollisionEventHandler;

        protected override void OnTriggerEnter(Collider other) => OnTriggerCollisionEventHandler?.Invoke(other, true);

        protected override void OnTriggerExit(Collider other) => OnTriggerCollisionEventHandler?.Invoke(other, false);
    }
}
