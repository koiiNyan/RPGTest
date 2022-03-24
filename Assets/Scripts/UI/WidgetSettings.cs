using RPG.Managers;
using RPG.ScriptableObjects;
using RPG.UI.Elements;
using RPG.Units.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace RPG.UI
{
    public class WidgetSettings : MonoBehaviour, IClosableWidget
    {
        private GameSettingsData _instance;

        [Inject]
        private PlayerControls _controls;
        [Inject]
        private GameSettings _settings;

        [SerializeField]
        private MultiButtonElement _cancelButton;
        [SerializeField]
        private MultiButtonElement _confirmButton;
        [SerializeField]
        private InverseToggle _generalToggle;

        [Space, SerializeField]
        private GameObject _generalSettings;
        [SerializeField]
        private GameObject _bindingSettings;

        [Space, SerializeField]
        private SelectorVisualElement _complication;
        [SerializeField]
        private SliderVisualElement _mouseSensitivity;
        [SerializeField]
        private SelectorVisualElement _fullScreen;
        [SerializeField]
        private SelectorVisualElement _resolution;
        [SerializeField]
        private Toggle _mute;
        [SerializeField]
        private SliderVisualElement _generalVolume;
        [SerializeField]
        private SliderVisualElement _soundtrackVolume;
        [SerializeField]
        private SliderVisualElement _effectVolume;
        [SerializeField]
        private SliderVisualElement _dialogueVolume;

        [Space, SerializeField]
        private BindingVisualElement[] _bindings;

        [Space, SerializeField]
        private string _cancelChangeTooltip = "Сохранить внесенные изменения?";//todo add localization

        public event SimpleHandle OnCloseWidgetEventHandler;

        internal IConfirmWaiter Confirmer { get; set; }

		public void ShowSettings()
        {
            ShowConfirmButton(false);
            gameObject.SetActive(true);

            _instance = _settings.GetSettingsData();

            //Инициализация не проведена для перечислений
            if(_complication.Variant == null)
            {
                var strs = Enum.GetNames(typeof(ComplicationMode));//todo set localization
                _complication.SetContentAndValue(strs);

                var res = Screen.resolutions;
                strs = new string[res.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    strs[i] = string.Concat(res[i].width, "x", res[i].height);
                }
                _resolution.SetContentAndValue(strs);

                strs = Enum.GetNames(typeof(FullScreenMode));//todo set localization
                _fullScreen.SetContentAndValue(strs);
            }

            UpdateState();
		}

        private void UpdateState()
        {
            _complication.VariantIndex = (int)_instance.Complication;
            _mouseSensitivity.Value = _instance.MouseSensitivity;
            _fullScreen.VariantIndex = (int)_instance.FullScreen;
            _resolution.Variant = string.Concat(_instance.Resolution.Width, "x", _instance.Resolution.Height);
            _mute.isOn = _instance.IsMute;
            _generalVolume.Value = _instance.GeneralVolume;
            _soundtrackVolume.Value = _instance.SoundtrackVolume;
            _effectVolume.Value = _instance.EffectVolume;
            _dialogueVolume.Value = _instance.DialogueVolume;
        }

        private void ShowConfirmButton(bool value = true)
        {
            _confirmButton.Interactable = value;
            _cancelButton.Interactable = value;
        }

		//todo Прокинуть изменения в игру
		#region Подписки на события полей настроек

		private void ComplicationChange(string value)
        {
            ShowConfirmButton();
            _instance.Complication = (ComplicationMode)Enum.Parse(typeof(ComplicationMode), value);
        }
        private void MouseSensitivityChange(float value)
        {
            ShowConfirmButton();
            _instance.MouseSensitivity = value;
        }
        private void FullScreenChange(int value)
        {
            ShowConfirmButton();
            _instance.FullScreen = (FullScreenMode)value;
            Screen.fullScreenMode = _instance.FullScreen;
        }
        private void ResolutionChange(string value)
        {
            ShowConfirmButton();
            var strs = value.Split('x');
            _instance.Resolution = new Resolution() { width = int.Parse(strs[0]), height = int.Parse(strs[1]), refreshRate = _instance.Resolution.RefreshRate };
            Screen.SetResolution(_instance.Resolution.Width, _instance.Resolution.Height, _instance.FullScreen);
        }
        private void MuteChange(bool value)
        {
            ShowConfirmButton();
            _instance.IsMute = value;//todo Подменять у АудиоМенеджера ссылку на временный ГеймСеттинг
        }
        private void GeneralVolumeChange(float value)
        {
            ShowConfirmButton();
            _instance.GeneralVolume = value;
        }
        private void SoundtrackVolumeChange(float value)
        {
            ShowConfirmButton();
            _instance.SoundtrackVolume = value;
        }
        private void EffectVolumeChange(float value)
        {
            ShowConfirmButton();
            _instance.EffectVolume = value;
        }
        private void DialogueVolumeChange(float value)
        {
            ShowConfirmButton();
            _instance.DialogueVolume = value;
        }

        #endregion

        #region Подписка и отписка. Start() & OnDestroy()

        private void Start()
        {
            _complication.OnStringChanged.AddListener(ComplicationChange);
            _mouseSensitivity.OnValueChanged.AddListener(MouseSensitivityChange);
            _fullScreen.OnIntChanged.AddListener(FullScreenChange);
            _resolution.OnStringChanged.AddListener(ResolutionChange);
            _mute.onValueChanged.AddListener(MuteChange);
            _generalVolume.OnValueChanged.AddListener(GeneralVolumeChange);
            _soundtrackVolume.OnValueChanged.AddListener(SoundtrackVolumeChange);
            _effectVolume.OnValueChanged.AddListener(EffectVolumeChange);
            _dialogueVolume.OnValueChanged.AddListener(DialogueVolumeChange);

            foreach (var bind in _bindings)
            {
                bind.SetControls(_controls);
                bind.OnUpdateBindingsEventHandler += ShowConfirmButton;
            }
        }

		private void OnValidate()
		{
            _bindings = GetComponentsInChildren<BindingVisualElement>(true);
		}

		private void OnDestroy()
		{
            _complication.OnStringChanged.RemoveListener(ComplicationChange);
            _mouseSensitivity.OnValueChanged.RemoveListener(MouseSensitivityChange);
            _fullScreen.OnIntChanged.RemoveListener(FullScreenChange);
            _resolution.OnStringChanged.RemoveListener(ResolutionChange);
            _mute.onValueChanged.RemoveListener(MuteChange);
            _generalVolume.OnValueChanged.RemoveListener(GeneralVolumeChange);
            _soundtrackVolume.OnValueChanged.RemoveListener(SoundtrackVolumeChange);
            _effectVolume.OnValueChanged.RemoveListener(EffectVolumeChange);
            _dialogueVolume.OnValueChanged.RemoveListener(DialogueVolumeChange);
        }

        #endregion

        public void OnSwitchSettingType_UnityEvent(bool value)
        {
            if (!value) return;

            if(_generalToggle.Toggle.isOn)
            {
                _generalSettings.SetActive(true);
                _bindingSettings.SetActive(false);
			}
            else
            {
                _generalSettings.SetActive(false);
                _bindingSettings.SetActive(true);
            }
        }

		public void OnConfirm_UnityEvent()
        {
            ShowConfirmButton(false);

            if(_generalToggle.Toggle.isOn)
            {
                _settings.SetSettings(_instance);
            }

            _settings.SaveBindingsAndSettings(_controls);
        }

        public void OnCancel_UnityEvent()
        {
            ShowConfirmButton(false);
            _instance = _settings.GetSettingsData();

            UpdateState();
        }

        public void OnClose_UnityEvent()
        {
            //Если изменения не были внесены/сохранены, не нужно больше предупреждений
            if(!_confirmButton.Interactable)
            {
                OnCloseWidgetEventHandler?.Invoke();
                return;
            }

            var confirmHanlder = Confirmer.CreateAsyncWaiting(_cancelChangeTooltip, OptionType.Confirm, OptionType.Cancel, 10);
            StartCoroutine(Confirmer.ConfirmWaiting(confirmHanlder,
                () => 
                {
                    OnConfirm_UnityEvent();
                    OnCloseWidgetEventHandler?.Invoke();
                },
                () =>
                {
                    OnCancel_UnityEvent();
                    OnCloseWidgetEventHandler?.Invoke();
                }));
        }//todo если были изменения, нужно оповестить о их потере
    }
}
