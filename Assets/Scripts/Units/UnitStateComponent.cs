using Newtonsoft.Json.Linq;
using RPG.Assistants;
using RPG.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.Units
{
    [SaveData("state")]
    public abstract partial class UnitStateComponent : UnitComponent, IStateRecorder
    {
        [SerializeField]
        private UnitStatsAssistant _stats;

        [Space, SerializeField]
        private SideType _sideType;
        [SerializeField]
        private string _displayName;

        public event SimpleHandle<Unit> OnDyingEventHandler;

        #region Properties

        public bool InAir { get; set; } = true;
        public SideType Side => _sideType;
        public string DisplayName => _displayName;

        
        public IParamsAssistant Parameters => _stats;
        public IStatusAssistant Statuses => _stats;
        public IEffectAssistant Effects => _stats;

        #endregion

        private void Start()
        {
            var unit = this.FindComponent<Unit>();

            Parameters.OnDyingEventHandler += OnDying;

#if UNITY_EDITOR
            _stats.Construct(unit.name);
#else
            _stats.Construct();
#endif
        }

		private void OnDestroy()
		{
            Parameters.OnDyingEventHandler -= OnDying;
        }

		private void OnDying()
        {
            OnDyingEventHandler?.Invoke(Owner);
		}

#if UNITY_EDITOR
        [ContextMenu("Set Default Stats")]
        private void SetDefaultStats()
        {
            var field = _stats.GetType().GetField("_statsDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var dic = field.GetValue(_stats) as StatsDictionary;

            if (!dic.ContainsKey(StatsType.Health)) dic.Add(StatsType.Health, new Container());
            if (!dic.ContainsKey(StatsType.HPRegInSec)) dic.Add(StatsType.HPRegInSec, new Container());
            if (!dic.ContainsKey(StatsType.Mana)) dic.Add(StatsType.Mana, new Container());
            if (!dic.ContainsKey(StatsType.MPRegInSec)) dic.Add(StatsType.MPRegInSec, new Container());
            if (!dic.ContainsKey(StatsType.Stamina)) dic.Add(StatsType.Stamina, new Container());
            if (!dic.ContainsKey(StatsType.SPRegInSec)) dic.Add(StatsType.SPRegInSec, new Container());
        }
#endif
	}
}
