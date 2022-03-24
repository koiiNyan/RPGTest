using RPG.Units.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

namespace RPG.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewGameSettings", menuName = "Scriptable Objects/Game Settings", order = 101)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        private GameSettingsData _data;


		/// <summary>
		/// Сложность игры
		/// </summary>
		public ComplicationMode Complication { get => _data.Complication; set => _data.Complication = value; }
        /// <summary>
        /// Чувствительность мыши
        /// </summary>
        /// <remarks>В диапазоне от 0 до 1</remarks>
        public float MouseSensitivity 
        {
            get => _data.MouseSensitivity;
            set => _data.MouseSensitivity = Mathf.Clamp(value, 0f, 2f);
        }
        /// <summary>
        /// Развернута-ли игра в фулл
        /// </summary>
        public FullScreenMode FullScreen { get => _data.FullScreen; set => _data.FullScreen = value; }
        /// <summary>
        /// Разрешение экрана
        /// </summary>
        public ResolutionData Resolution { get => _data.Resolution; set => _data.Resolution = value; }
        /// <summary>
        /// Общая громкость игры
        /// </summary>
        /// <remarks>В диапазоне от 0 до 1</remarks>
        public float GeneralVolume 
        {
            get => _data.GeneralVolume; 
            set => _data.GeneralVolume = Mathf.Clamp01(value);
        }
        /// <summary>
        /// Громкость сандтрека
        /// </summary>
        /// <remarks>В диапазоне от 0 до 1</remarks>
        public float SoundtrackVolume 
        {
            get => _data.SoundtrackVolume; 
            set => _data.SoundtrackVolume = Mathf.Clamp01(value);
        }
        /// <summary>
        /// Громкость эффектов
        /// </summary>
        /// <remarks>В диапазоне от 0 до 1</remarks>
        public float EffectVolume 
        { 
            get => _data.EffectVolume; 
            set => _data.EffectVolume = Mathf.Clamp01(value);
        }
        /// <summary>
        /// Громкость диалогов
        /// </summary>
        /// <remarks>В диапазоне от 0 до 1</remarks>
        public float DialogueVolume 
        { 
            get => _data.DialogueVolume; 
            set => _data.DialogueVolume = Mathf.Clamp01(value);
        }
        /// <summary>
        /// Выключена-ли музыка
        /// </summary>
        public bool IsMute { get => _data.IsMute; set => _data.IsMute = value; }

        public GameSettingsData GetSettingsData()
        {
            return new GameSettingsData
            {
                Complication = Complication,
                MouseSensitivity = MouseSensitivity,
                FullScreen = FullScreen,
                Resolution = Resolution,
                GeneralVolume = GeneralVolume,
                SoundtrackVolume = SoundtrackVolume,
                EffectVolume = EffectVolume,
                DialogueVolume = DialogueVolume,
                IsMute = IsMute
            };
        }

        /// <summary>
        /// Вызывается ТОЛЬКО у оригинального объекта, прокинутого в редакторе!
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettings(GameSettingsData settings)
        {
            Complication = settings.Complication;
            MouseSensitivity = settings.MouseSensitivity;
            FullScreen = settings.FullScreen;
            Resolution = settings.Resolution;
            GeneralVolume = settings.GeneralVolume;
            SoundtrackVolume = settings.SoundtrackVolume;
            EffectVolume = settings.EffectVolume;
            DialogueVolume = settings.DialogueVolume;
            IsMute = settings.IsMute;
        }

        public void LoadBindingsAndSettings(PlayerControls controls)
        {
            CreateConfigFiles();
            //Парсим общие сеттинги
            using (var file = File.OpenRead(GetFullPath(isTXTtype: true)))
            {
                if(file.Length > 0)
                {
                    var binary = new BinaryFormatter();
                    _data = binary.Deserialize(file) as GameSettingsData;
                }
            }
            //Парсим привязки клавиш
            XDocument doc = XDocument.Load(GetFullPath(isTXTtype: false));
            XElement root = doc.Root;
            var overrides = new Dictionary<Guid, string>();
            //Считываем все сохраненные привязки
            foreach (var bind in root.Elements("Bind"))
            {
                overrides.Add(
                    Guid.Parse(bind.Attribute("Guid").Value),
                    bind.Attribute("Path").Value);
            }
            //Обновление привязок клавиш
            foreach (var action in controls)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (overrides.TryGetValue(action.bindings[i].id, out var value))
                    {
                        action.ApplyBindingOverride(0, new InputBinding { overridePath = value });
                    }
                }
            }
        }

        public void SaveBindingsAndSettings(PlayerControls controls)
        {
            CreateConfigFiles();

            //Обновляем общие сеттинги
            using (var file = File.OpenWrite(GetFullPath(isTXTtype: true)))
            {
                var binary = new BinaryFormatter();
                binary.Serialize(file, _data);
            }

            //Обновляем сеттинги привязок
            var path = GetFullPath(isTXTtype: false);
            var doc = XDocument.Load(path);
            doc.RemoveNodes();
            var root = new XElement("Root");

            //Сохранение привязок клавиш
            foreach (var action in controls)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (!string.IsNullOrEmpty(action.bindings[i].overridePath))
                    {
                        var bind = new XElement("Bind");
                        bind.SetAttributeValue("Guid", action.bindings[i].id);
                        bind.SetAttributeValue("Path", action.bindings[i].overridePath);
                        root.Add(bind);
                    }
                }
            }

            var stream = File.OpenWrite(path);
            doc.Add(root);
            doc.Save(stream);
            stream.Close();
        }

        public float GetVolume(AudioSourceType type)
        {
            if (IsMute) return 0f;

            switch (type)
            {
                case AudioSourceType.General:
                    return GeneralVolume;
                case AudioSourceType.Soundtrack:
                    return GeneralVolume * SoundtrackVolume;
                case AudioSourceType.Effect:
                    return GeneralVolume * EffectVolume;
                case AudioSourceType.Dialogue:
                    return GeneralVolume * DialogueVolume;
            }

            throw new System.ApplicationException($"Unimplemented logic of type enumeration: <b>{nameof(AudioSourceType)}</b>");
        }

        private string GetFullPath(bool isTXTtype) => string.Concat(Application.persistentDataPath, "/", name, isTXTtype ? ".txt" : ".xml");

        private void CreateConfigFiles()
        {
            var path = GetFullPath(isTXTtype: true);
            if (!File.Exists(path))
            {
                var file = File.Create(path);
                file.Close();
            }

            path = GetFullPath(isTXTtype: false);
            if (!File.Exists(path))
            {
                var newDoc = new XDocument();
                newDoc.Add(new XElement("Root"));

                var stream = File.OpenWrite(path);
                newDoc.Save(stream);
                stream.Close();
            }
        }
    }

    [System.Serializable]
    public class GameSettingsData
    {
        [Header("---Game settings---"), Tooltip("Сложность игры")]
        public ComplicationMode Complication;
        [Range(0f, 2f), Tooltip("Чувствительность мыши")]
        public float MouseSensitivity = 1f;

        [Header("---Screen settings---"), Tooltip("Развернута-ли игра в фулл")]
        public FullScreenMode FullScreen = FullScreenMode.FullScreenWindow;
        [Tooltip("Разрешение экрана")]
        public ResolutionData Resolution;

        [Header("---Sound settings---"), Range(0f, 1f), Tooltip("Общая громкость игры")]
        public float GeneralVolume = 1f;
        [Range(0f, 1f), Tooltip("Громкость сандтрека")]
        public float SoundtrackVolume = 1f;
        [Range(0f, 1f), Tooltip("Громкость эффектов")]
        public float EffectVolume = 1f;
        [Range(0f, 1f), Tooltip("Громкость диалогов")]
        public float DialogueVolume = 1f;
        [Tooltip("Выключена-ли музыка")]
        public bool IsMute;

        internal GameSettingsData() { }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(GameSettings))]
    public class GameSettingsEditor : UnityEditor.Editor
    {
		public override void OnInspectorGUI()
		{
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space(30f);
            GUI.color = Color.green;
            UnityEditor.EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Reset Params"))
            {
                typeof(GameSettings).GetProperty(nameof(GameSettings.Complication)).SetValue(target, ComplicationMode.Easy);
                typeof(GameSettings).GetProperty(nameof(GameSettings.MouseSensitivity)).SetValue(target, 1f);
                typeof(GameSettings).GetProperty(nameof(GameSettings.FullScreen)).SetValue(target, FullScreenMode.FullScreenWindow);
                typeof(GameSettings).GetProperty(nameof(GameSettings.Resolution)).SetValue(target, new ResolutionData { Width = 2560, Height = 1440, RefreshRate = 59 });
                typeof(GameSettings).GetProperty(nameof(GameSettings.GeneralVolume)).SetValue(target, 1f);
                typeof(GameSettings).GetProperty(nameof(GameSettings.SoundtrackVolume)).SetValue(target, 1f);
                typeof(GameSettings).GetProperty(nameof(GameSettings.EffectVolume)).SetValue(target, 1f);
                typeof(GameSettings).GetProperty(nameof(GameSettings.DialogueVolume)).SetValue(target, 1f);
                typeof(GameSettings).GetProperty(nameof(GameSettings.IsMute)).SetValue(target, false);

                UnityEditor.EditorUtility.SetDirty(target);
            }
            GUILayout.FlexibleSpace();
            UnityEditor.EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
	}

#endif
}