using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPG.Assistants;
using RPG.Commands;
using RPG.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Units
{
    [Serializable]
    public class UnitStatsAssistant : IParamsAssistant, IStatusAssistant, IEffectAssistant
    {
        private float _health;
        private LinkedList<BaseCommandEffect> _effects = new LinkedList<BaseCommandEffect>();
#if UNITY_EDITOR
        private string _ownerName;
#endif


        #region Parameters

        [SerializeField]
        private StatsDictionary _statsDictionary;

        public event SimpleHandle OnDyingEventHandler;

        public float Health 
        {
            get => _health;
            set 
            {
                _health = value;
                if (_health <= 0f) OnDyingEventHandler.Invoke();
			}
        }

        public float Mana { get; set; }
        public float Stamina { get; set; }
        public float Armor { get; set; }//test todo

        public Container this[StatsType type]
        {
            get
            {
#if UNITY_EDITOR
                if (!_statsDictionary.ContainsKey(type))
                {
                    Editor.EditorExtensions.ConsoleLog($"Unit <b>{_ownerName}</b> do not have stats <b>{type}</b>", Editor.PriorityMessageType.Critical);
                    return null;
                }
#endif
                return _statsDictionary[type];
            }
		}

        public bool TryGetParameter(StatsType type, out Container value) => _statsDictionary.TryGetValue(type, out value);

        #endregion

        #region Statuses

        public void AddStatuses(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses) AddStatus(status);
		}

        public void AddStatus(Status status)
        {
#if UNITY_EDITOR
            if (!_statsDictionary.ContainsKey(status.Type))
            {
                Editor.EditorExtensions.ConsoleLog($"Unit <b>{_ownerName}</b> has not stats <b>{status.Type}</b>", Editor.PriorityMessageType.Low);
                return;
            }
#endif

            _statsDictionary[status.Type].AddStatus(status.Data);
        }

        public void RemoveStatuses(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses) RemoveStatus(status);
		}

        public void RemoveStatus(Status status)
        {
#if UNITY_EDITOR
            if (!_statsDictionary.ContainsKey(status.Type))
            {
                Editor.EditorExtensions.ConsoleLog($"Unit <b>{_ownerName}</b> has not stats <b>{status.Type}</b>", Editor.PriorityMessageType.Low);
                return;
            }
#endif

            _statsDictionary[status.Type].RemoveStatusByID(status.Data.Id);
        }

        #endregion

        #region Effects

        public event SimpleHandle<BaseCommandEffect> OnEffectEventHandler;

        public IReadOnlyCollection<BaseCommandEffect> GetAllEffects => _effects;

		public void AddEffect(BaseCommandEffect effect)
        {
            _effects.AddLast(effect);
            effect.OnStart();
            OnEffectEventHandler?.Invoke(effect);
        }

        public void AddEffects(IEnumerable<BaseCommandEffect> effects)
        {
            foreach (var effect in effects) AddEffect(effect);
        }

        public void RemoveEffect(BaseCommandEffect effect)
        {
            _effects.Remove(effect);
		}

        public void RemoveEffects(IEnumerable<BaseCommandEffect> effects)
        {
            foreach (var effect in effects) _effects.Remove(effect);
		}

        #endregion

#if UNITY_EDITOR
        public void Construct(string ownerName)
#else
        public void Construct() 
#endif
        {
#if UNITY_EDITOR
            _ownerName = ownerName;
#endif
            foreach (var pair in _statsDictionary) pair.Value.UpdateValue();

            Health = _statsDictionary[StatsType.Health].GetValue / 3f;//todo
            Mana = _statsDictionary[StatsType.Mana].GetValue / 3f;
            Stamina = _statsDictionary[StatsType.Stamina].GetValue / 3f;
        }

		public JObject GetSaveData()
		{
            var obj = new JObject();

            var dic = new Dictionary<StatsType, float>(_statsDictionary.Count);
            foreach (var pair in _statsDictionary) dic.Add(pair.Key, pair.Value);
            
            obj.Add("parameters", JObject.FromObject(dic));

            var effects = new JArray();
            foreach(var el in _effects)
            {
				var eff = new JObject
				{
					{ "id", el.ID },
					{ "source", el.Source?.CreateUniqueUnitCode() },
					{ "target", el.Target?.CreateUniqueUnitCode() },
					{ "time", el.CurrentDuration }
				};

				effects.Add(eff);
			}
            obj.Add("effects", effects);
            obj.Add("health", Health);
            obj.Add("mana", Mana);
            obj.Add("stamina", Stamina);

            return obj;
        }

		public void SetSaveData(JToken token)
		{
            var dic = token["parameters"].ToObject<Dictionary<StatsType, float>>();
            _statsDictionary.Clear();
            foreach (var pair in dic) _statsDictionary.Add(pair.Key, new Container(pair.Value));

            _effects.Clear();
            var effects = token["effects"] as JArray;
            foreach(JObject el in effects)
            {
                var effect = Assistants.ConstructEntityExtensions.CreateAnyCommandEffect(el["id"].Value<string>());
				var unitID = el["source"].Value<string>();
				(BotPrefabType, string, ulong) ids;
				if (unitID != null)//todo если игрок сохранялся в триггере, то при загрузке, триггер отработает повторно
				{
					effect.Source = ConstructEntityExtensions.FindUnit(unitID, false);
				}

				unitID = el["target"].Value<string>();
                if(unitID != null)
                {
                    effect.Target = ConstructEntityExtensions.FindUnit(unitID, false);
                }

                effect.CurrentDuration = el["time"].Value<float>();
                AddEffect(effect);
            }

            Health = token["health"].Value<float>();
            Mana = token["mana"].Value<float>();
            Stamina = token["stamina"].Value<float>();
        }
	}

    public interface IParamsAssistant : IStateRecorder
    {
        event SimpleHandle OnDyingEventHandler;

        Container this[StatsType type] { get; }

        float Health { get; set; }
        float Mana { get; set; }
        float Stamina { get; set; }
        float Armor { get; set; }

        bool TryGetParameter(StatsType type, out Container value);
    }

    public interface IStatusAssistant : IStateRecorder
    {
        void AddStatuses(IEnumerable<Status> statuses);
        void AddStatus(Status status);
        void RemoveStatuses(IEnumerable<Status> statuses);
        void RemoveStatus(Status status);
    }

    public interface IEffectAssistant : IStateRecorder
    {
        event SimpleHandle<BaseCommandEffect> OnEffectEventHandler;

        IReadOnlyCollection<BaseCommandEffect> GetAllEffects { get; }

        void AddEffect(BaseCommandEffect effect);
        void AddEffects(IEnumerable<BaseCommandEffect> effects);
        void RemoveEffect(BaseCommandEffect effect);
        void RemoveEffects(IEnumerable<BaseCommandEffect> effects);
    }
}
