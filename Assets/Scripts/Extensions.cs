using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RotaryHeart.Lib.SerializableDictionary;

using RPG.Commands;
using RPG.UI.Elements;
using RPG.Units;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Unity.Mathematics;

using UnityEditor;

using UnityEngine;

namespace RPG
{
    public interface IClosableWidget
    {
        event SimpleHandle OnCloseWidgetEventHandler;
        void OnClose_UnityEvent();
    }

    interface IConfirmWaiter
    {
        ResultHandler CreateAsyncWaiting(string text, OptionType trueOption, OptionType falseOption, int time = -1);
        IEnumerator ConfirmWaiting(ResultHandler handler, Action trueResult, Action falseResult = null);
    }

    public interface IStateRecorder
    {
        JObject GetSaveData();
        void SetSaveData(JToken token);
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class SaveDataAttribute : Attribute
    {
        public string Key { get; }
        public byte Priority { get; }

        public SaveDataAttribute(string key, byte priority = 0)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(key)) throw new ApplicationException($"The <b>{nameof(SaveDataAttribute)}</b> attribute must contain a non-null binding key");
#endif
            Key = key;
            Priority = priority;
        }
    }

    public delegate void SimpleHandle();
    public delegate void SimpleHandle<T>(T arg);
    public delegate void SimpleHandle<T1, T2>(T1 arg1, T2 arg2);
    public delegate T1out SuccessHandler<T1out, T1, T2>(T1 arg1, T2 arg2);

    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }
    public class SQRFloatAttribute : PropertyAttribute { }

    public class SerializeFieldDataStateAttribute : PropertyAttribute { }

    public class ConfigurationSkeletonAttribute : Attribute { }


	[Serializable]
	public class StatsDictionary : SerializableDictionaryBase<StatsType, Container>	{ }
	[Serializable]
    public class PhraseSpriteDictionary : SerializableDictionaryBase<PhraseType, Sprite> { }
    [Serializable]
    public class EquipmentDictionary : SerializableDictionaryBase<EquipmentType, ItemCell> { }
    [Serializable]
    public class EquipmentSetDictionary : SerializableDictionaryBase<EquipmentType, EquipmentSet> { }

    public static class StringHelper
    {
        private static Dictionary<StatsType, string> _rusTemp;
        private static Dictionary<ItemType, string> _rusTemp2;
        private static Dictionary<OptionType, string> _rusTemp3;

        public static string ConfirmDelayText = "Отмена через: ";
        public static string BindWaitingText = "Ожидание привязки...";

        public static LocalizationType Language { get; set; }
        public static string GetStatsLocalization(StatsType stats)//todo
        {
            return _rusTemp[stats];
		}
        public static string GetInventoryLocalization(ItemType type)//todo
        {
            return _rusTemp2[type];
		}
        public static string GetOptionTypeLocalization(OptionType type)
        {
            return _rusTemp3[type];
		}

        static StringHelper()
        {
            _rusTemp = new Dictionary<StatsType, string>
            {
                { StatsType.MoveAcceleration, "Скорость хотьбы" },
                { StatsType.SprintAcceleration, "Скорость бега" },
                { StatsType.MoveVelocity, "Предел скорости хотьбы" },
                { StatsType.SprintVelocity, "Предел скорости бега" },
                { StatsType.ForceJump, "Сила прыжка" },
                { StatsType.Health, "Здоровье" },
                { StatsType.Mana, "Мана" },
                { StatsType.Stamina, "Выносливость" },
                { StatsType.HPRegInSec, "Регенерация здоровья" },
                { StatsType.MPRegInSec, "Регенерация маны" },
                { StatsType.SPRegInSec, "Регенерация выносливости" },
                { StatsType.Damage, "Урон" },
                { StatsType.CriticalChance, "Вероятность критической атаки" },
                { StatsType.ArmorMult, "Броня" },
                { StatsType.RotateSpeed, "Скорость поворота" },
            };

            _rusTemp2 = new Dictionary<ItemType, string>
            {
                { ItemType.Other, "Прочее" },
                { ItemType.Material, "Материалы" },
                { ItemType.Ingredient, "Ингредиенты" },
                { ItemType.Ammunition, "Амуниция" },
                { ItemType.Equipment, "Снаряжение" },
            };

            _rusTemp3 = new Dictionary<OptionType, string>
            {
                { OptionType.Ok, "Ок" },
                { OptionType.Back, "Назад" },
                { OptionType.Cancel, "Отмена" },
                { OptionType.Confirm, "Принять" }
            };
		}
	}

    public static partial class Constants
    {
        public static ulong PlayerNumberID = 1;
        public static string PlayerName = "Player";
        public static string FloorTag = "Floor";
        public static string AttackTag = "Attack";
        public static string BlockTag = "Block";
        public static string BattleLayerName = "Battle";
        public static string ObstacleLayerName = "Obstacles";
        public static string LoadIdentifier = "load";

        public static int ObstacleLayerInt;
        public static float DelayShowPopupInItem = 2f;
        public static Vector2Int PlayerInventorySize = new Vector2Int(7, 8);

        private static Dictionary<WeaponStyleType, string> _layers = new Dictionary<WeaponStyleType, string>
        {
            { WeaponStyleType.None, "Unarmed" },
            { WeaponStyleType.ShieldAndSword, "SwordAndShield" },
            { WeaponStyleType.Bow, "Archer" }
        };

        private static Dictionary<WeaponStyleType, int> _layersInt;

        public static IDictionary<WeaponStyleType, int> GetLayersInts => _layersInt;
        public static int GetAnimatorIndexLayer(WeaponStyleType type) => _layersInt[type];
        public static string GetAnimatorNameLayer(WeaponStyleType type) => _layers[type];

        public static void ConfigurationConstance(Animator animator)
        {
            ObstacleLayerInt = LayerMask.NameToLayer(ObstacleLayerName);

            _layersInt = new Dictionary<WeaponStyleType, int>(_layers.Count);

            foreach(var pair in _layers)
            {
#if UNITY_EDITOR
                if (animator.GetLayerIndex(pair.Value) == -1) 
                    Editor.EditorExtensions.ConsoleLog($"Animation Layer with name <b>{pair.Value}</b> in Controller: <b>{animator.name}</b> not found", Editor.PriorityMessageType.Critical);
#endif
                _layersInt.Add(pair.Key, animator.GetLayerIndex(pair.Value));
			}
        }
    }

    public static class SimpleExtensions
    {
        public static float4 SimpleConvert(this Color v) => new float4(v.r, v.g, v.b, v.a);
        public static Color SimpleConvert(this float4 v) => new Color(v.x, v.y, v.z, v.w);
        public static string ConvertToString(this Vector3 v) => $"{v.x}|{v.y}|{v.z}";
        public static Vector3 ConvertToVector3(string str)
        {
            var strs = str.Split('|');
#if UNITY_EDITOR
            if (strs.Length != 3) Editor.EditorExtensions.ConsoleLog($"Trying to convert string <b>{str}</b> to vector", Editor.PriorityMessageType.Critical);
#endif
            return new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]));
		}
        public static void Nulling<T>(this IList<T> list)
        {
            for(int i = 0; i < list.Count; i++) list[i] = default;
        }
        public static void Nulling(this Array list)
        {
            for (int i = 0; i < list.Length; i++) list.SetValue(default, i);
        }
    }

    public static class MonoBehaviourExtensions
    {
        public static T FindComponent<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponent<T>();
            if(component == null) PrintLog(typeof(T).Name, source.name);
            return component;
        }

        public static T[] FindComponents<T>(this MonoBehaviour source) where T : Component
        {
            var components = source.GetComponents<T>();
            if (components == null || components.Length == 0) PrintLog(typeof(T).Name, source.name);
            return components;
        }

        public static T FindComponentInChildren<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponentInChildren<T>();
            if (component == null) PrintLog(typeof(T).Name, source.name);
            return component;
        }

        public static T[] FindComponentsInChildren<T>(this MonoBehaviour source) where T : Component
        {
            var components = source.GetComponentsInChildren<T>();
            if (components == null || components.Length == 0) PrintLog(typeof(T).Name, source.name);
            return components;
        }

        public static T FindComponentInParent<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponentInParent<T>();
            if (component == null) PrintLog(typeof(T).Name, source.name);
            return component;
        }
        public static T[] FindComponentsInParent<T>(this MonoBehaviour source) where T : Component
        {
            var components = source.GetComponentsInParent<T>();
            if (components == null || components.Length == 0) PrintLog(typeof(T).Name, source.name);
            return components;
        }

        private static void PrintLog(string componentName, string name)
        {
#if UNITY_EDITOR
            Editor.EditorExtensions.ConsoleLog($"Component : <b>{componentName}</b> not found on GameObject : <i>{name}</i>", Editor.PriorityMessageType.Critical);
#endif
        }
    }

#if UNITY_EDITOR
    public static class RPGExtensions
    {
        public static LinkedList<T> FindAllAssetsByType<T>(SearchOption option = SearchOption.AllDirectories, string additionalPath = "") where T : UnityEngine.Object
        {
            var pathes = Directory.GetFiles(Application.dataPath + additionalPath, "*.asset", option);
            var assets = new LinkedList<T>();
            foreach (string path in pathes)
            {
                var assetPath = "Assets" + path.Replace(Application.dataPath, "").Replace('\\', '/');
                var file = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as T;

                if (file != null) assets.AddLast(file);
            }

            return assets;
        }
    }
#endif
}
