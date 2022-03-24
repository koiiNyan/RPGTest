using RPG.Commands;
using RPG.ScriptableObjects.Configurations;
using RPG.Units;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Effects
{
    [CreateAssetMenu(fileName = "NewNonTargetEffectConfiguration", menuName = "Scriptable Objects/Effects/Non Target Effect Configuration", order = 1)]
    public class NonTargetEffectConfiguration : EffectConfiguration<NonTargetEffect>
    {
        [SerializeField]
        protected string _animationKey = "Spell_PowerUp";
        
        [Space, SerializeField]
        protected List<float> _health;
        [SerializeField]
        protected List<float> _mana;
        [SerializeField]
        protected List<float> _stamina;
        [SerializeField]
        protected List<float> _hpRegInSec;
        [SerializeField]
        protected List<float> _mpRegInSec;
        [SerializeField]
        protected List<float> _spRegInSec;
        [SerializeField]
        protected List<float> _damage;
        [SerializeField]
        protected List<float> _armor;
        [SerializeField]
        protected List<float> _criticalChance;

        public override NonTargetEffect CreateEffect(string id)
        {
            int i = 0;
            for (; i < _mainDatas.Count; i++)
            {
                if (_mainDatas[i].Id != id) continue;

                break;
            }

            var list = new LinkedList<Status>();

            if (_health[i] != 0f) list.AddLast(new Status() { Type = StatsType.Health, Data = new StatusData(10001, _health[i]) });
            if (_mana[i] != 0f) list.AddLast(new Status() { Type = StatsType.Mana, Data = new StatusData(10001, _mana[i]) });
            if (_stamina[i] != 0f) list.AddLast(new Status() { Type = StatsType.Stamina, Data = new StatusData(10001, _stamina[i]) });
            if (_hpRegInSec[i] != 0f) list.AddLast(new Status() { Type = StatsType.HPRegInSec, Data = new StatusData(10001, _hpRegInSec[i]) });
            if (_mpRegInSec[i] != 0f) list.AddLast(new Status() { Type = StatsType.MPRegInSec, Data = new StatusData(10001, _mpRegInSec[i]) });
            if (_spRegInSec[i] != 0f) list.AddLast(new Status() { Type = StatsType.SPRegInSec, Data = new StatusData(10001, _spRegInSec[i]) });
            if (_damage[i] != 0f) list.AddLast(new Status() { Type = StatsType.Damage, Data = new StatusData(10001, _damage[i]) });
            if (_armor[i] != 0f) list.AddLast(new Status() { Type = StatsType.ArmorMult, Data = new StatusData(10001, _armor[i]) });
            if (_criticalChance[i] != 0f) list.AddLast(new Status() { Type = StatsType.CriticalChance, Data = new StatusData(10001, _criticalChance[i]) });

            return new NonTargetEffect(statuses: list, animationKey: _animationKey, _mainDatas[i]);
        }

        public Dictionary<string, float> GetAllDelays()
        {
            var dic = new Dictionary<string, float>(_mainDatas.Count);

            foreach (var data in _mainDatas) dic.Add(data.Id, data.Duration);
            return dic;
		}
    }
}
