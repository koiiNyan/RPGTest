using RPG.Triggers;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Items
{
    public class WeaponComponent : EquipmentComponent
    {
        [SerializeField]
        private BattleTriggerComponent _trigger;
        [SerializeField]
        private WeaponStyleType _style;

        public WeaponStyleType Style => _style;

        public SimpleHandle<GameObject, EquipmentType> OnCollisionEnterEventHandler;

        public bool Collider
        {
            get
            {
                if (_trigger == null) return false;
                return _trigger.Enable;
            }
            set
            {
                if (_trigger != null) _trigger.Enable = value;
            }
        }

		private void Start()
		{
            if (_trigger == null) return;
            _trigger.OnCollisionEnterEventHandler += (obj) => OnCollisionEnterEventHandler?.Invoke(obj, Equipment);
		}
	}
}
