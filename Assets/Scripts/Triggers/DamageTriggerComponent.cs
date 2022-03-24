using RPG.Commands;
using RPG.Units.Player;
using UnityEngine;

namespace RPG.Triggers
{
    public class DamageTriggerComponent : TriggerComponent
	{
		private DamageEffect _effect;

		[SerializeField]
		private string _effectID;

		protected override void OnTriggerEnter(Collider other)
		{
			var player = other.GetComponent<PlayerUnit>();
			if (player == null) return;

			var effect = _effect.Clone();
			effect.Target = player;
			player.State.Effects.AddEffect(effect);
		}

        protected override void Start()
        {
			base.Start();
			_effect = Assistants.ConstructEntityExtensions.CreateCommandEffect<DamageEffect>(_effectID);
        }
    }
}
