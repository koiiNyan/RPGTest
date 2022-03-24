using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Triggers
{
    public class BattleTriggerComponent : BaseTriggerComponent
    {
		public SimpleHandle<GameObject> OnCollisionEnterEventHandler;

		protected override void Start()
		{
			base.Start();
			gameObject.layer = LayerMask.NameToLayer(Constants.BattleLayerName);
		}

		protected override void OnTriggerEnter(Collider other)
		{
			OnCollisionEnterEventHandler?.Invoke(other.gameObject);
		}
	}
}