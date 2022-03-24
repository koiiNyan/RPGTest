using RPG.Managers;
using RPG.UI.Blocks;
using RPG.UI.Elements;
using RPG.Units.Player;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

namespace RPG.UI
{
    public class WidgetMenu : MonoBehaviour, IClosableWidget, IConfirmWaiter
    {
        private Coroutine _popupCoroutine;

        [Inject]
        private UnitManager _unitManager;

        [SerializeField]
        private ConfirmElement _confirmer;
        [SerializeField]
        private WidgetSaveAndLoad _games;
        [SerializeField]
        private WidgetSettings _settings;

        [SerializeField]
        private TextMeshProUGUI _saveGameButton;

        [Header("---Настройка попапа---"), Space, SerializeField]
        private RectTransform _popup;
        [SerializeField]
        private TextMeshProUGUI _popupText;
        [SerializeField, Range(1f, 10f)]
        private float _popupDelay = 3f;
        [SerializeField]
        private Vector3 _endPosition;
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField, Range(0.1f, 3f)]
        private float _moveTime = 1f;

        [Space, SerializeField]
        private string _exitGameTooltip = "Вы действительно хотите покинуть игру? Несохраненные данные будут потеряны";//todo localization


        public event SimpleHandle OnCloseWidgetEventHandler;
        public event SimpleHandle OnLoadingGameEventhandler;
        public event SimpleHandle OnLoadedGameEventHandler;

		private void OnEnable()
		{
            TimeAssistant.SetGameDeltaTime(0f);

            //Во время боя нельзя сохранять игру
            var color = _saveGameButton.color;
            color.a = _unitManager.InFight ? 0.3f : 1f;
            _saveGameButton.color = color;
        }

		private void OnDisable()
		{
            TimeAssistant.SetGameDeltaTime(1f);
            _popup.anchoredPosition = _startPosition;
        }

		private IEnumerator ShowPopup()
        {
            _popup.gameObject.SetActive(true);

            //Выдвижение попапа
            yield return PopupMoving(_startPosition, _endPosition);
            //Демонстрация попапа
            yield return new WaitForSeconds(_popupDelay);
            //Убирание попапа
            yield return PopupMoving(_endPosition, _startPosition);

            _popup.gameObject.SetActive(false);
            _popupCoroutine = null;
        }

        private IEnumerator PopupMoving(Vector3 start, Vector3 end)
        {
            var time = 0f;

            //Перемещение попапа
            while (time <= 1f)
            {
                _popup.anchoredPosition = Vector3.Lerp(start, end, time);
                time += TimeAssistant.UIDeltaTime / _moveTime;
                yield return null;
            }
        }

        public ResultHandler CreateAsyncWaiting(string text, OptionType trueOption, OptionType falseOption, int time = -1) => _confirmer.CreateAsyncWaiting(text, trueOption, falseOption, time);

        public IEnumerator ConfirmWaiting(ResultHandler handler, Action trueResult, Action falseResult = null)
        {
            while(!handler.IsCompleted)
            {
                yield return null;
			}

            if (handler.Result)
            {
                trueResult.Invoke();
            }
            else
            {
                falseResult?.Invoke();
            }

            _confirmer.gameObject.SetActive(false);
		}

        public void SaveGame_UnityEvent()
        {
            if(_unitManager.InFight)
            {
                if (_popupCoroutine != null) StopCoroutine(_popupCoroutine);
                _popupCoroutine = StartCoroutine(ShowPopup());
                _popupText.text = "Во время боя нельзя сохраняться";
                return;
			}

            _games.ShowWidgetInSaveMode(true);
        }

        public void LoadGame_UnityEvent()
        {
            _games.ShowWidgetInSaveMode(false);
        }

        public void Settings_UnityEvent()
        {
            _settings.ShowSettings();
		}

        public void ExitGame_UnityEvent()
        {
            var confirmHanlder = _confirmer.CreateAsyncWaiting(_exitGameTooltip, OptionType.Confirm, OptionType.Cancel);
            StartCoroutine(ConfirmWaiting(confirmHanlder, () => { SceneManager.LoadScene(0); }, null));
		}

		public void OnClose_UnityEvent() => OnCloseWidgetEventHandler?.Invoke();

        private void Start()
        {
            _games.OnCloseWidgetEventHandler += CloseSubMenu;
            _games.OnLoadingGameEventhandler += OnLoadingGameEventhandler;
            _games.OnLoadedGameEventHandler += OnLoadedGameEventHandler;
            _settings.OnCloseWidgetEventHandler += CloseSubMenu;

            _games.Confirmer = this;
            _settings.Confirmer = this;

            if (_games.gameObject.activeSelf) _games.gameObject.SetActive(false);
            if (_settings.gameObject.activeSelf) _settings.gameObject.SetActive(false);
            if (_confirmer.gameObject.activeSelf) _confirmer.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _games.OnCloseWidgetEventHandler -= CloseSubMenu;
            _settings.OnCloseWidgetEventHandler -= CloseSubMenu;
            _games.OnLoadingGameEventhandler -= OnLoadingGameEventhandler;
            _games.OnLoadedGameEventHandler -= OnLoadedGameEventHandler;
        }

        private void CloseSubMenu()
        {
            if (_games.gameObject.activeSelf) _games.gameObject.SetActive(false);
            if (_settings.gameObject.activeSelf) _settings.gameObject.SetActive(false);
        }
    }
}
