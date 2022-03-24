using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class PhraseVisualElement : MonoBehaviour
    {
        private string[] _numberRichText = new string[4] { "<size=", /*25*/"><color=#", /*00D9FF*/">", ". </color></size>" };
        private int _index;

        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _textField;
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Color _numberColor = Color.cyan;
        [SerializeField]
        private Color _textColor = Color.white;
        [SerializeField]
        private int _numberSize = 25;

        [SerializeField, Tooltip("Отступ от левого края при наличии картинки")]
        private Vector2 _offsetWithIcon = new Vector2(50f, 0f);

        public event SimpleHandle<int> OnClickEventHandler;


        public void SetContent(int index, string text, Sprite sprite = null)
        {
            _index = index;

#if UNITY_EDITOR
            _textField.text = string.Concat(_numberRichText[0], _numberSize, _numberRichText[1], ColorUtility.ToHtmlStringRGB(_numberColor), _numberRichText[2], index + 1, _numberRichText[3], text);
            _textField.color = _textColor;
#else
            _textField.text = string.Concat(_numberRichText[0], index, _numberRichText[1], text);
#endif

            _icon.sprite = sprite;

            var rect = _textField.transform as RectTransform;

            _icon.enabled = sprite != null;
		}

		void Start()
        {
#if !UNITY_EDITOR
            //"<size=VALUE1><color=#VALUE2>"
            var str = string.Concat(_numberRichText[0], _numberSize, _numberRichText[1], ColorUtility.ToHtmlStringRGB(_numberColor), _numberRichText[2]);
            _numberRichText = new string[2] { str, _numberRichText[3] };
            _textField.color = _textColor;
#endif
        }

        private void OnClickEvent()
        {
            OnClickEventHandler?.Invoke(_index);
		}

		private void OnEnable()
		{
            _button.onClick.AddListener(OnClickEvent);
        }

		private void OnDisable()
		{
            _button.onClick.RemoveListener(OnClickEvent);
        }
	}
}
