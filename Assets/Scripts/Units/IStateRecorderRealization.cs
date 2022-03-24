using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RPG.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace RPG.Units
{
	public partial class Unit
	{
        protected internal SignalBus Loader;

        public virtual JObject GetSaveData()
        {
            var obj = new JObject();
            obj.Add("transform", JsonConvert.SerializeObject(new TransformData(transform), Formatting.Indented));

            return obj;
        }

        public virtual void SetSaveData(JToken token)
        {
            JsonConvert.DeserializeObject<TransformData>(token["transform"].Value<string>()).SetLocalTransformData(transform);
        }
    }

    public partial class UnitStateComponent
    {
        public virtual JObject GetSaveData()
        {
            var obj = new JObject();

            var stats = _stats as IStateRecorder;

#if UNITY_EDITOR
            if (stats == null) Editor.EditorExtensions.ConsoleLog($"class <b>{_stats.GetType().Name}</b> does not implement interface <b>{nameof(IStateRecorder)}</b>", Editor.PriorityMessageType.Critical);
#endif

            obj.Add("stats", stats.GetSaveData());
            obj.Add("display", _displayName);

            return obj;
        }

        public virtual void SetSaveData(JToken token)
        {
            var stats = _stats as IStateRecorder;

#if UNITY_EDITOR
            if (stats == null) Editor.EditorExtensions.ConsoleLog($"class <b>{_stats.GetType().Name}</b> does not implement interface <b>{nameof(IStateRecorder)}</b>", Editor.PriorityMessageType.Critical);
#endif
            stats.SetSaveData(token["stats"]);
            _displayName = token["display"].Value<string>();
        }
    }

    public partial class UnitMoveComponent
    {
        public JObject GetSaveData()
        {
            var obj = new JObject
            {
                { "sprint", InSprint },
                { "crouch", InCrouch }
            };

            return obj;
        }

        public void SetSaveData(JToken token)
        {
            InSprint = token["sprint"].Value<bool>();
            InCrouch = token["crouch"].Value<bool>();
        }
    }

    public partial class WeaponSetComponent
    {

        public JObject GetSaveData()
        {
            var token = new JObject();
            foreach (var pair in _sets)
            {
                token.Add(pair.Key.ToString(), pair.Value.Item.ID);
            }

            token.Add("armed", InArmed);

            return token;
        }

        public void SetSaveData(JToken token)
        {
            var equipments = new LinkedList<EquipmentItem>();

            //Чистка снаряжения перед новой установкой
            foreach (var set in _sets.Keys) ChangeEquipment(null, set);

            foreach (var pair in token as JObject)
            {
                if (pair.Key == "armed") continue;//todo Костыль, чтобы не подцепать к словаре отсутствующий ключ перечисления
                var key = (EquipmentType)Enum.Parse(typeof(EquipmentType), pair.Key);

                var id = pair.Value.Value<string>();
                if (string.IsNullOrEmpty(id)) continue;

                equipments.AddLast(Assistants.ConstructEntityExtensions.FindItem(id) as EquipmentItem);
                //ChangeEquipment(Assistants.ConstructEntityExtensions.FindItem(id) as EquipmentItem);
            }

            Owner.Loader?.FireId(Constants.LoadIdentifier, equipments);

            InArmed = token["armed"].Value<bool>();
        }
    }
}

namespace RPG.Units.Player
{
    public partial class PlayerUnit
    {
        [Inject]
        private void ConstructSignal(SignalBus signal) => Loader = signal;

        public override JObject GetSaveData()
        {
            var obj = base.GetSaveData();

            //Общий массив сумок
            var inventory = new JObject();
            foreach (var bag in Inventory)
            {
                var jBag = new JObject();
                //Сохраняем массив элементов из сумки по ключу
                inventory.Add(bag.Key.ToString(), jBag);
                for (int i = 0; i < bag.Value.Count; i++)
                {
                    if (bag.Value[i] == null) continue;
                    //Конфигурация элемента: Имя - индекс в сумке, значение - объединение айди и кол-ва через соединитель
                    jBag.Add(i.ToString(), string.Concat(bag.Value[i].ID, "%", bag.Value[i].Count));
                }
            }

            obj.Add("inventory", inventory);


            var dic = new Dictionary<InputSetType, string>(SkillSet.Count);
            foreach (var pair in SkillSet)
            {
                dic.Add(pair.Key, pair.Value.ID);
            }
            obj.Add("skillSet", JObject.FromObject(dic));
            return obj;
        }

        public override void SetSaveData(JToken token)
        {
            base.SetSaveData(token);
            //Проходим по массиву сумок
            foreach (JProperty item in token["inventory"])
            {
                //Находим сумку, соответствующую токен-сумке
                var bag = Inventory[(ItemType)Enum.Parse(typeof(ItemType), item.Name)];
                bag.Nulling();
                //Двигаемся по элементам токен-сумки и парсим их
                foreach (var element in item.Value as JObject)
                {
                    var values = element.Value.Value<string>().Split('%');

                    var baseItem = Assistants.ConstructEntityExtensions.FindItem(values[0]);
                    baseItem.Count = int.Parse(values[1]);
                    AddItem(baseItem, int.Parse(element.Key));
                }
            }

            var dic = token["skillSet"].ToObject<Dictionary<InputSetType, string>>();
            SkillSet.Clear();
            foreach (var pair in dic)
            {
                var effect = Assistants.ConstructEntityExtensions.CreateCommandEffect<NonTargetEffect>(pair.Value);
                effect.Source = this;//todo пока у игрока есть только нон-таргет скилл эффекты
                SkillSet.Add(pair.Key, effect);
            }
        }
    }
}


namespace RPG.Units.NPCs
{
    public partial class NPCUnit
    {
        public override JObject GetSaveData()
        {
            var obj = base.GetSaveData();

            if (SpeakingQueue != null) obj.Add("speak", JArray.FromObject(SpeakingQueue));
            if (QuestQueue != null) obj.Add("quest", JArray.FromObject(QuestQueue));
            if (ParagraphList != null) obj.Add("paragraph", JArray.FromObject(ParagraphList));
            obj.Add("style", (byte)_weaponType);
            obj.Add("prefab", (byte)_botPrefabType);

            return obj;
        }

        public override void SetSaveData(JToken token)
        {
            base.SetSaveData(token);

            var el = token["speak"];
            if (el != null) SpeakingQueue = el.ToObject<Queue<string>>();
            el = token["quest"];
            if (el != null) QuestQueue = el.ToObject<Queue<string>>();
            el = token["paragraph"];
            if (el != null) ParagraphList = el.ToObject<LinkedList<IdentificatorParagraphData>>();
            _weaponType = (WeaponStyleType)token["style"].Value<byte>();
            _botPrefabType = (BotPrefabType)token["prefab"].Value<byte>();
        }
    }

    public partial class NPCInputComponent
    {

        public JObject GetSaveData()
        {

            var obj = new JObject
            {
                { "state", (byte)StateType },
                { "delay", Delay },
                { "start", new JValue(_startPoint.ConvertToString()) },
                { "patroll", _currentPatrollingPointIndex },
            };

            var array = new JArray();
            foreach (var point in _patrollingPoints)
            {
                array.Add(new JValue(point.ConvertToString()));
            }

            obj.Add("points", array);

            return obj;
        }

        public void SetSaveData(JToken token)
        {
            StateType = (AIStateType)token["state"].Value<byte>();
            Delay = token["delay"].Value<float>();
            _startPoint = SimpleExtensions.ConvertToVector3(token["start"].Value<string>());
            _currentPatrollingPointIndex = token["patroll"].Value<int>();

            var array = token["points"] as JArray;
            _patrollingPoints = new Vector3[array.Count];

            var index = 0;
            foreach (JValue point in array)
            {
                _patrollingPoints[index] = SimpleExtensions.ConvertToVector3(point.Value<string>());
                index = index + 1;
            }
        }
    }
}
