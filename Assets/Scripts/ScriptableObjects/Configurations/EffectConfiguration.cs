using RPG.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.ScriptableObjects.Configurations
{
    public abstract class EffectConfiguration<T> : BaseEffectConfiguration where T : BaseCommandEffect
    {
        [SerializeField]
        protected List<EffectData> _mainDatas;

        public abstract T CreateEffect(string id);

		public override bool ContainsEffectWithID(string id) => _mainDatas.FirstOrDefault(t => t.Id == id).Id != null;
        public override BaseCommandEffect CreateAnyEffect(string id) => CreateEffect(id);
    }

    public abstract class BaseEffectConfiguration : GeneralConfiguration 
    {
        public abstract bool ContainsEffectWithID(string id);
        public abstract BaseCommandEffect CreateAnyEffect(string id);
    }
}

namespace RPG.ScriptableObjects
{
    public abstract class GeneralConfiguration : ScriptableObjectConfiguration { }
    public abstract class ScriptableObjectConfiguration : ScriptableObject { }
}
