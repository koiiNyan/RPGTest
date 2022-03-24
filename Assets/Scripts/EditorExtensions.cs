#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Units;
using System.Reflection;
using System;
using System.Xml.Linq;

namespace RPG.Editor
{
    public static class EditorExtensions
    {
        private static Dictionary<PriorityMessageType, string> _dic;

        public static void ConsoleLog(object message, PriorityMessageType type = PriorityMessageType.None)
        {
            switch (type)
            {
                case PriorityMessageType.None:
                    Debug.Log(message);
                    break;
                case PriorityMessageType.Notification:
                    Debug.Log(string.Concat(_dic[type], message));
                    break;
                case PriorityMessageType.Low:
                    Debug.LogError(string.Concat(_dic[type], message));
                    break;
                case PriorityMessageType.Critical:
                    Debug.LogError(string.Concat(_dic[type], message));
                    if(EditorApplication.isPlaying) EditorApplication.isPaused = true;
                    break;
            }
        }

        public static void LogError(IEnumerable<object> messages, PriorityMessageType type = PriorityMessageType.None)
        {
            var builder = new System.Text.StringBuilder();

            foreach (var message in messages) builder.Append(message);
            ConsoleLog(builder.ToString(), type);
		}

        public static void PrintRichSettings()
        {
            
		}

        static EditorExtensions()
        {
            _dic = new Dictionary<PriorityMessageType, string>()
            {
                { PriorityMessageType.None, string.Empty },
                { PriorityMessageType.Notification, "<color=#007AFF>[Notification]</color> " },
                { PriorityMessageType.Low, "<color=#005500>[Low]</color> " },
                { PriorityMessageType.Critical, "<color=#880000>[Critical]</color> " }
            };
        }
    }

    public static class EditorConstants
    {
        /// <summary>
        /// Название игрового объекта для фокусирования юнитов
        /// </summary>
        public static readonly string FocusTargetPointName = "Neck";
        /// <summary>
        /// Название игрового объекта для проверки нахождения в воздухе
        /// </summary>
        public static readonly string AirColliderName = "AirCollider";
        /// <summary>
        /// Название слоя, в котором существуют триггеры
        /// </summary>
        public static readonly string TriggerLayer = "Triggers";
        /// <summary>
        /// Название слоя игрока
        /// </summary>
        public static readonly string PlayerLayer = "Player";
        /// <summary>
        /// Название слоя, в котором существуют объекты взаимодействующие ТОЛЬКО с игроков
        /// </summary>
        public static readonly string PlayerTriggerLayer = "PlayerTriggers";
        /// <summary>
        /// Массив имен костей, дочерними объектами к которым должны быть коллайдеры проверки прыжков
        /// </summary>
        public static readonly string[] BonesNameForAirColliders = new string[] { "LeftFoot", "RightFoot" };
        /// <summary>
        /// Параметры настройки коллайдеров проверки прыжков
        /// </summary>
        public static Bounds AirColliderBound = new Bounds(new Vector3(0f, 0.01f, 0f), new Vector3(0.5f, 0.16f, 0.5f));
        /// <summary>
        /// Префикс игровых объектов, создаваемых для редактора
        /// </summary>
        public static readonly string EditorGameObjectName = "EDITOR_OBJECT";
        /// <summary>
        /// Настройки капсулы агентов. x - baseOffset | y - radius | z - height
        /// </summary>
        public static readonly Vector3 MeshAgentObstacleAvoidance = new Vector3(-0.05f, 0.3f, 1.5f);
        /// <summary>
        /// Настройка стиля отрисовки в гизмос текста
        /// </summary>
        public static readonly GUIStyle GizmosLabelStyle = new GUIStyle("label")
        {
            fontSize = 56,
            fontStyle = FontStyle.Bold,
        };
        /// <summary>
        /// Настройка стиля отрисовки в гизмос маленького текста
        /// </summary>
        public static readonly GUIStyle GizmosSmallLabelStyle = new GUIStyle("label")
        {
            fontSize = 36,
        };
    }

	public enum PriorityMessageType : byte
    {
        None,
        Notification,
        Low,
		Critical,
	}

    [CustomPropertyDrawer(typeof(Container))]
    public class ContainerPropertyDrawer : PropertyDrawer
    {
        private const string c_propertyName = "_defaultValue";
        private const float c_coef = 0.4f;
        private const float c_space = 5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative(c_propertyName);
            var rect = new Rect(position.x, position.y, position.width * c_coef, position.height);
            EditorGUI.LabelField(rect, property.displayName);
            rect = new Rect(position.x + position.width * c_coef + c_space, position.y, position.width * (1 - c_coef) - c_space, position.height);
            EditorGUI.PropertyField(rect, prop, GUIContent.none);
        }
    }

    //[CustomPropertyDrawer(typeof(Pair))]
    //public class AudioPropertyDrawer : BasePairPropertyDrawer { }

    public abstract class BasePairPropertyDrawer : BasePropertyDrawer
    {
        public BasePairPropertyDrawer()
        {
            _properties = new string[] { "Key", "Value" };
        }
    }

    public abstract class BasePropertyDrawer : PropertyDrawer
    {
        protected string[] _properties;

        private const float space = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var firstLineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            DrawMainProperties(firstLineRect, property);

            EditorGUI.indentLevel = indent;
        }

        private void DrawMainProperties(Rect rect, SerializedProperty property)
        {
            rect.width = (rect.width - 2 * space) / _properties.Length;
            foreach (var prop in _properties)
            {
                DrawProperty(rect, property.FindPropertyRelative(prop));
                rect.x += rect.width + space;
            }
        }

        private void DrawProperty(Rect rect, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property, GUIContent.none);
        }
    }


    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }


    [CustomPropertyDrawer(typeof(SQRFloatAttribute))]
    public class SQRFloatDrawer : PropertyDrawer
    {
        private const float _labelWidthPercent = 0.4f;
        private const float _space = 35f;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(rect.x, rect.y, rect.width * _labelWidthPercent, EditorGUIUtility.singleLineHeight);
            //Название поля
            EditorGUI.LabelField(labelRect, property.displayName);

            var rectangle = new Rect(rect.x + rect.width * _labelWidthPercent, rect.y, rect.width * (1f - _labelWidthPercent) / 2f, EditorGUIUtility.singleLineHeight);

            var printRect = new Rect(rectangle.x + _space, rectangle.y, rectangle.width - _space, rectangle.height);
            //Значение поля
            GUI.enabled = false;
            var value = EditorGUI.FloatField(printRect, GUIContent.none, Mathf.Sqrt(property.floatValue));
            property.floatValue = value * value;
            GUI.enabled = true;
            rectangle.x += rectangle.width;
            
            printRect = new Rect(rectangle.x, rectangle.y, rectangle.width - _space, rectangle.height);
            //Приписка к полю с корнем из текущего значения
            EditorGUI.LabelField(printRect, "Sqrt");

            printRect = new Rect(rectangle.x + _space, rectangle.y, rectangle.width - _space, rectangle.height);

            //Поле с корнем текущего значения
            EditorGUI.PropertyField(printRect, property, GUIContent.none);
        }
    }
}
#endif