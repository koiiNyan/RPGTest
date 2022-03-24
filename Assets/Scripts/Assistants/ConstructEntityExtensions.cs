using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RPG.Commands;
using RPG.Managers;
using RPG.ScriptableObjects.Configurations;
using RPG.ScriptableObjects.Contexts;
using RPG.ScriptableObjects.Effects;
using RPG.Units;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

namespace RPG.Assistants
{
    public static class ConstructEntityExtensions
    {
        private static Dictionary<Type, BaseEffectConfiguration> _effectConfigs;
        private static BaseItemContext[] _itemContexts;
        private static UnitManager _unitManager;
        private static GameObject _interfaceRoot;

        public static JsonSerializer GetSettings => new JsonSerializer
        {
            Formatting = Formatting.Indented
        };

        public static JObject SaveGameObjectState(GameObject GO)
        {
            var save = new JObject();

            var priorityList = new List<(IStateRecorder, SaveDataAttribute)>();

            //Сохраняем свойства и поля компонентов игрока
            foreach (var component in GO.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (!(component is IStateRecorder recorder)) continue;
                var attr = recorder.GetType().GetCustomAttribute<SaveDataAttribute>();

#if UNITY_EDITOR
                if (attr == null)
                {
                    Editor.EditorExtensions.ConsoleLog($"The class <b>{recorder.GetType().Name}</b>, which implements the <b>{nameof(IStateRecorder)}</b> interface, is not marked with the <b>{nameof(SaveDataAttribute)}</b> attribute", Editor.PriorityMessageType.Critical);
                    throw new System.ApplicationException();
                }
#endif

                priorityList.Add((recorder, attr));
            }

            //Сортировка массива, сначала сохраняются высокоприоритетные компоненты
            priorityList.Sort(PrioritySaveSort);

            //Запись данных в соответствии с приоритетностью компонентов
            foreach (var pair in priorityList) save.Add(pair.Item2.Key, pair.Item1.GetSaveData());

            return save;
        }

        public static JObject SaveGameObjectState(Component GO)
        {
            return SaveGameObjectState(GO.gameObject);
        }

        public static Unit FindUnit(string code, bool withCreate = true)
        {
            var strs = code.Split('%');

            //Код принадлежит боту
            if (strs.Length == 3)
            {
                foreach (var unit in _unitManager.GetNPCs) 
                    if (unit.GetID == strs[1]) return unit;

                if (withCreate) return _unitManager.CreateNPC((BotPrefabType)Enum.Parse(typeof(BotPrefabType), strs[0]), strs[1], ulong.Parse(strs[2]));
                else return null;
            }
            //Код принадлежит игроку
            else if (strs.Length == 2)
            {
#if UNITY_EDITOR
                var n = ulong.Parse(strs[1]);
                if (n != Constants.PlayerNumberID || strs[0] != Constants.PlayerName) Editor.EditorExtensions.ConsoleLog($"There is code in the save file that has been interpreted as <b>player code</b>, but is not. Number: <b>{n}</b>, Name: <b>{strs[0]}</b>", Editor.PriorityMessageType.Critical); 
#endif
                return _unitManager.GetPlayer;
            }

            return null;
        }

        public static GameObject FindWidget(string name)
        {
            foreach (var obj in _interfaceRoot.GetComponentsInChildren<Transform>(true))
                if (obj.name == name) return obj.gameObject;

#if UNITY_EDITOR
            Editor.EditorExtensions.ConsoleLog($"Widget named <b>{name}</b> was not found", Editor.PriorityMessageType.Critical);
#endif
            return null;
		}

        public static string CreateUniqueUnitCode(this Unit unit)
        {
            if (unit is Units.NPCs.NPCUnit npc)
                return string.Concat((unit as Units.NPCs.NPCUnit).GetPrefabType, "%", unit.GetID, "%", unit.NumberID);//todo terror code

            return string.Concat(unit.GetID, "%", unit.NumberID);
        }

        public static void LoadGameObjectState(string code, JObject value)
        {
            var obj = FindUnit(code, false)?.gameObject;
            if(obj == null)
            {
                obj = FindWidget(code);
			}
#if UNITY_EDITOR
            if (obj == null) Editor.EditorExtensions.ConsoleLog($"<b>{nameof(LoadGameObjectState)}: </b>Incorrect search code: <b>{code}</b>", Editor.PriorityMessageType.Critical);
#endif

            LoadGameObjectState(obj, value);
        }

        public static void LoadGameObjectState(GameObject any, JObject value)
        {
            var priorityList = new List<(IStateRecorder, SaveDataAttribute)>();

            //Устанавливаем свойства и поля компонентов игрока
            foreach (var component in any.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (!(component is IStateRecorder recorder)) continue;
                var attr = recorder.GetType().GetCustomAttribute<SaveDataAttribute>();

#if UNITY_EDITOR
                if (attr == null)
                {
                    Editor.EditorExtensions.ConsoleLog($"The class <b>{recorder.GetType().Name}</b>, which implements the <b>{nameof(IStateRecorder)}</b> interface, is not marked with the <b>{nameof(SaveDataAttribute)}</b> attribute", Editor.PriorityMessageType.Critical);
                    throw new System.ApplicationException();
                }
#endif

                priorityList.Add((recorder, attr));
            }
            //Сортировка массива, сначала загружаются высокоприоритетные компоненты
            priorityList.Sort(PrioritySaveSort);

            //Установка данных в соответствии с приоритетностью компонентов
            foreach (var pair in priorityList) pair.Item1.SetSaveData(value[pair.Item2.Key] as JObject);
        }

        public static T CreateCommandEffect<T>(string id) where T : BaseCommandEffect//todo тоже можно сделать систему проверки и рефлексии для редактора
        {
        
            return (_effectConfigs[typeof(T)] as EffectConfiguration<T>).CreateEffect(id);
		}

        /// <summary>
        /// Создает любой эффект по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор эффекта</param>
        /// <returns>Обобщенный эффект</returns>
        /// <remarks>ВНИМАНИЕ! Если известен тип эффекта, лучше воспользоваться методом CreateCommandEffect<T>(string), так как там происходит более оптимизированный поиск</remarks>
        public static BaseCommandEffect CreateAnyCommandEffect(string id)
        {
            foreach(var value in _effectConfigs.Values)
            {
                if (!value.ContainsEffectWithID(id)) continue;

                return value.CreateAnyEffect(id);
			}
#if UNITY_EDITOR
            Editor.EditorExtensions.ConsoleLog($"No effect was found for the specified identifier <b>{id}</b>", Editor.PriorityMessageType.Critical);
            throw new ApplicationException();
#else
            throw new ApplicationException("No effect was found for the specified identifier: " + id);
#endif
		}

        public static BaseItem FindItem(string id, ItemType? type = null, RarityItemType? rarity = null)
        {
#if UNITY_EDITOR
            IEnumerable<BaseItemContext> contexts = null;
            if (_itemContexts == null)
            {
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    Editor.EditorExtensions.ConsoleLog($"An attempt was detected to add an item with id:<b>{id}<b> not in GameMode. Reflection of the GameManager field is involved", Editor.PriorityMessageType.Low);
                }
                var name = "_itemContexts";
                var field = typeof(Managers.GameManager).GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    Editor.EditorExtensions.ConsoleLog($"<b>{nameof(Managers.GameManager)}</b> does not contain a field named <b>{name}</b>", Editor.PriorityMessageType.Critical);
                    throw new ApplicationException();
                }

                contexts = field.GetValue(Object.FindObjectOfType<Managers.GameManager>()) as IEnumerable<BaseItemContext>;
            }
            else
            {
                contexts = _itemContexts;
            }
#else

            IEnumerable<BaseItemContext> contexts = _itemContexts;
#endif
            if (type != null) contexts = contexts.Where(t => t.Type == type.Value);
            if (rarity != null) contexts = contexts.Where(t => t.Rarity == rarity.Value);

            BaseItem item = null;
            foreach (var context in contexts)
            {
                item = context.TryGetItemByID(id);
                if (item != null)
                {
                    item.Count = 1;
                    return item;
                }
            }

#if UNITY_EDITOR
            if (item == null) Editor.EditorExtensions.ConsoleLog($"Item with identifier <b>{id}</b> is missing in contexts", Editor.PriorityMessageType.Critical);
#endif
            return item;
        }

        internal static void Construct(UnitManager unitManager, GameObject interfaceRoot, BaseEffectConfiguration[] effectConfig, BaseItemContext[] itemContexts)
        {
            _unitManager = unitManager;
            _interfaceRoot = interfaceRoot;
            _effectConfigs = new Dictionary<Type, BaseEffectConfiguration>(effectConfig.Length);
            foreach(var config in effectConfig)
            {

#if UNITY_EDITOR
                var types = config.GetType().BaseType.GenericTypeArguments;
                if(types == null || types.Length != 1)
                {
                    Editor.EditorExtensions.ConsoleLog($"Among the EffectConfigurations there is an ScriptableObject that is not generated-type, inherited from <b>{nameof(BaseEffectConfiguration)}</b>", Editor.PriorityMessageType.Critical);
                    throw new ApplicationException();
				}
                var type = types[0];
#else
                var type = config.GetType().BaseType.GenericTypeArguments[0];
#endif
                _effectConfigs.Add(type, config);
			}
            _itemContexts = itemContexts;
        }

        private static int PrioritySaveSort((IStateRecorder, SaveDataAttribute) a, (IStateRecorder, SaveDataAttribute) b)
        {
            if (a.Item2.Priority > b.Item2.Priority) return -1;
            if (a.Item2.Priority < b.Item2.Priority) return 1;
            return 0;
        }
    }
}
