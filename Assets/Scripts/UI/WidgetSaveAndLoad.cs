using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using RPG.Assistants;
using RPG.Managers;
using RPG.UI.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Zenject;

namespace RPG.UI
{
    public class WidgetSaveAndLoad : MonoBehaviour, IClosableWidget
    {
        private GameObjectPool<SaveSlotVisualElement> _pool;
        private bool _isSaveMode;

        [Inject] private UnitManager _unitManager;
        [Inject] private GameObject _interface;

        [SerializeField]
        private string _defaultPath = "DefaultSaveSlot";
        [SerializeField]
        private string _saveModeText = "Сохранить игру";
        [SerializeField]
        private string _loadModeText = "Загрузить игру";

        [Space, SerializeField]
        private SaveSlotVisualElement _prefab;
        [SerializeField]
        private RectTransform _content;
        [SerializeField]
        private TextMeshProUGUI _buttonText;


        public event SimpleHandle OnCloseWidgetEventHandler;
        public event SimpleHandle OnLoadingGameEventhandler;
        public event SimpleHandle OnLoadedGameEventHandler;

        internal IConfirmWaiter Confirmer { get; set; }

        public void LoadDefaultSave() => LoadGame(_defaultPath, true);

        public void ShowWidgetInSaveMode(bool isSaveMode)
        {
            _isSaveMode = isSaveMode;

            _buttonText.text = isSaveMode ? _saveModeText : _loadModeText;//todo localization

            if (isSaveMode) ShowSaveGameMode();
            else ShowLoadGameMode();

            //foreach(var slot in _pool)
            //{

                //}

                //gameObject.SetActive(true);
        }

        private void ShowSaveGameMode()
        {
            SaveGame(GetFullPath(1));
        }
        private void ShowLoadGameMode()
        {
            LoadGame(GetFullPath(1), false);
        }

        private void LoadGame(string path, bool isResource)
        {
            OnLoadingGameEventhandler?.Invoke();

            JProperty property = null;

#if UNITY_EDITOR
            if (!isResource && !File.Exists(path)) Editor.EditorExtensions.ConsoleLog($"There is no save file on the path: <b>{path}</b>", Editor.PriorityMessageType.Critical);
#endif

            if (isResource)
            {
                var text = Resources.Load<TextAsset>(path);
                property = JToken.Parse(text.text).First as JProperty;
            }
            else
            {
                using (var file = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    using (var reader = new BsonReader(file))
                    {
                        property = ConstructEntityExtensions.GetSettings.Deserialize<JToken>(reader).First as JProperty;
                    }
                }
            }

            while (property != null)
            {
                ConstructEntityExtensions.LoadGameObjectState(property.Path, property.Value as JObject);
                property = property.Next as JProperty;
            }

            OnLoadedGameEventHandler?.Invoke();
        }

        private void SaveGame(string path)
        {
            if (!File.Exists(path))
            {
                var file = File.Create(path);
                file.Close();
            }

            var content = new JObject()
            {
                {_unitManager.GetPlayer.CreateUniqueUnitCode(), ConstructEntityExtensions.SaveGameObjectState(_unitManager.GetPlayer) },
                {_interface.name, ConstructEntityExtensions.SaveGameObjectState(_interface) },
            };

            foreach (var unit in _unitManager.GetNPCs)
            {
                content.Add(unit.CreateUniqueUnitCode(), ConstructEntityExtensions.SaveGameObjectState(unit));
            }
            
            using (var file = File.Open(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BsonWriter(file))
                {
                    ConstructEntityExtensions.GetSettings.Serialize(writer, content);
                }
                
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }

		private void Awake()
		{
            _pool = new GameObjectPool<SaveSlotVisualElement>(_prefab);
        }

		private string GetFullPath(int i) => string.Concat(Application.persistentDataPath, "/Slot", i, ".txt");

        public void OnClose_UnityEvent() => OnCloseWidgetEventHandler?.Invoke();
        public void OnSaveOrLoadGame_UnityEvent()
        {
            ShowLoadGameMode();
        }
    }
}
