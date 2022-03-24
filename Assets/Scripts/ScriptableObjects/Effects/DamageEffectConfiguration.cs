using RPG.Commands;
using RPG.ScriptableObjects.Configurations;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Effects
{
    [CreateAssetMenu(fileName = "NewDamageEffectConfiguration", menuName = "Scriptable Objects/Effects/Damage Effect Configuration", order = 1)]
    public class DamageEffectConfiguration : EffectConfiguration<DamageEffect>
    {
        [SerializeField]
        protected List<float> _firstDamage;
        [SerializeField]
        protected List<float> _periodDamage;
        [SerializeField]
        protected List<float> _delay;

		public override DamageEffect CreateEffect(string id)
		{
            int i = 0;
            for(; i < _mainDatas.Count; i++)
            {
                if (_mainDatas[i].Id != id) continue;

                break;
			}

            return new DamageEffect(firstDamage: _firstDamage[i], periodDamage: _periodDamage[i], delay: _delay[i], _mainDatas[i]);
		}
    }
}
