using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public abstract class UnitComponent : MonoBehaviour
    {
        protected internal Unit Owner;

        protected virtual void Awake()
        {
            Owner = this.FindComponent<Unit>();
		}
    }
}
