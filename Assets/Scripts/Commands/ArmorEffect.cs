using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Commands
{
    public class ArmorEffect : BaseCommandEffect//, IUpdateCommand, IStartCommand
    {
        public float DiminutionInSec { get; private set; }
        public float StartGain { get; private set; }

		public override void OnStart()
        {
            Target.State.Parameters.Armor += StartGain;
        }

        public override void OnUpdate(float delta)
        {
            Target.State.Parameters.Armor -= DiminutionInSec * delta;
        }

        public override void OnEnd() { }

        public ArmorEffect(float diminutionInSec, float startGain, EffectData data) : base(data)
        {
            DiminutionInSec = diminutionInSec; StartGain = startGain;
        }

        public override BaseCommandEffect Clone()
        {
            return new ArmorEffect(DiminutionInSec, StartGain, new EffectData(ID, Duration, Sprite));
        }

		public override string Serialize()
		{
			throw new System.NotImplementedException();
		}

		public override void Deserialize(string code)
		{
			throw new System.NotImplementedException();
		}
	}
}
