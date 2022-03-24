using RPG.Managers;
using UnityEngine;
using Zenject;

namespace RPG.Triggers
{
	public abstract class TriggerComponent : BaseTriggerComponent
	{
		[Inject]
		protected TriggerManager _manager;
	}

	[RequireComponent(typeof(Collider))]
	public abstract class BaseTriggerComponent : MonoBehaviour
    {
        private Collider _collider;

		public bool Enable
		{
			get => _collider.enabled;
			set => _collider.enabled = value;
		}

		protected virtual void Start()
		{
			_collider = this.FindComponent<Collider>();
			_collider.isTrigger = true;
		}

		protected abstract void OnTriggerEnter(Collider other);

		protected virtual void OnTriggerExit(Collider other) { }
	}
}
