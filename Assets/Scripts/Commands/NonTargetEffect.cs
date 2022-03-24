using RPG.Units;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Commands
{
	public class NonTargetEffect : BaseCommandSkill
	{
		private IEnumerable<Status> _statuses;

		public override void OnUpdate(float delta){}
		public override void OnStart()
		{
			Source.Animator.SetTrigger(AnimationKey);
			Source.State.Statuses.AddStatuses(_statuses);
		}
		public override void OnEnd()
		{
			Source.State.Statuses.RemoveStatuses(_statuses);
		}

		public override BaseCommandEffect Clone()
		{
			var effect = new NonTargetEffect(_statuses, AnimationKey, new EffectData(ID, Duration, Sprite));
			effect.Source = Source; effect.Target = Target;
			return effect;
		}

		public override string Serialize()
		{
			throw new System.NotImplementedException();
		}

		public override void Deserialize(string code)
		{
			throw new System.NotImplementedException();
		}

		public NonTargetEffect(IEnumerable<Status> statuses, string animationKey, EffectData effect) : base(animationKey, effect) 
		{
			_statuses = statuses;
		}
	}
}
