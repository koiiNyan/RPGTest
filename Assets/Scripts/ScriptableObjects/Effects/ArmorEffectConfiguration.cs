using RPG.Commands;
using RPG.ScriptableObjects.Configurations;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Effects
{
    [CreateAssetMenu(fileName = "NewArmorEffectConfiguration", menuName = "Scriptable Objects/Effects/Armor Effect Configuration", order = 1)]
    public class ArmorEffectConfiguration : EffectConfiguration<ArmorEffect>
    {
        [SerializeField]
        protected List<float> _diminutionInSec;
        [SerializeField]
        protected List<float> _startGain;

        public override ArmorEffect CreateEffect(string id)
        {
            int i = 0;
            for (; i < _mainDatas.Count; i++)
            {
                if (_mainDatas[i].Id != id) continue;

                break;
            }

            return new ArmorEffect(diminutionInSec: _diminutionInSec[i], startGain: _startGain[i], _mainDatas[i]);
        }
    }
}