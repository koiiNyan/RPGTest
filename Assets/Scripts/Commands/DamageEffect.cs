using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RPG.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Commands
{
    public class DamageEffect : BaseCommandEffect//, IUpdateCommand, IStartCommand 
    {
        [JsonProperty("CurrentDelay")]
        private float _currentDelay;

        [JsonIgnore]
        public float FirstDamage { get; private set; }
        [JsonIgnore]
        public float PeriodDamage { get; private set; }
        [JsonIgnore]
        public float Delay { get; private set; }



		public override void OnStart()
		{
            Target.State.Parameters.Health -= FirstDamage;
            _currentDelay = Delay;
		}

        public override void OnUpdate(float delta)
        {
            _currentDelay -= delta;

            if(_currentDelay <= 0f)
            {
                Target.State.Parameters.Health -= PeriodDamage;
                _currentDelay = Delay;
            }
        }

		public override void OnEnd() { }

		public DamageEffect(float firstDamage, float periodDamage, float delay, EffectData data) : base(data)
        {
            FirstDamage = firstDamage; PeriodDamage = periodDamage; Delay = delay;
        }

        public override BaseCommandEffect Clone()
        {
            return new DamageEffect(FirstDamage, PeriodDamage, Delay, new EffectData(ID, Duration, Sprite));
        }

		public override string Serialize()
		{
            return JsonConvert.SerializeObject(this);
		}

		public override void Deserialize(string code)
		{
			throw new System.NotImplementedException();
		}
	}
}
