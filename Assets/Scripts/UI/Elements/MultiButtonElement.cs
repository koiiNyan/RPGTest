using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static UnityEngine.UI.Button;

namespace RPG.UI.Elements
{
	[RequireComponent(typeof(Animator))]
	public class MultiButtonElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		private bool _isInit;
		private Vector2 _defaultGlarePosition;
		private float _defaultTextSize;
		private Color _defaultTextColor;
		private bool _interactable = true;

		[Space, SerializeField]
		private Animator _animator;
		[SerializeField]
		private Image _background;
		[SerializeField]
		private TextMeshProUGUI _text;

		[Space, SerializeField]
		private RectTransform _glareImage;
		[SerializeField]
		private Image _enterImage;

		[Space, SerializeField]
		private Color _backgroundSwitchColor = Color.gray;
		[SerializeField]
		private Color _textSwitchColor = Color.white;

		[Space, SerializeField]
		private ButtonClickedEvent _onClick;

		public ButtonClickedEvent OnClick => _onClick;

		public bool Interactable
		{
			get => _interactable;
			set
			{
				if (!_isInit) InitInternalData();

				if (_interactable == value) return;

				_interactable = value;
				_animator.enabled = value;

				//Обнуляем состояние кнопки
				if(!_animator.enabled)
				{
					_glareImage.anchoredPosition = _defaultGlarePosition;
					_enterImage.enabled = false;
					_text.fontSize = _defaultTextSize;
					_text.color = _defaultTextColor;
					_text.transform.localRotation = new Quaternion();
				}

				var tempColor = _background.color;
				_background.color = _backgroundSwitchColor;
				_backgroundSwitchColor = tempColor;

				tempColor = _text.color;
				_text.color = _textSwitchColor;
				_textSwitchColor = tempColor;

				if (!_interactable)
				{
					OnPointerExit(null);
					OnPointerUp(null);
				}
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_animator.SetBool("InEnter", true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_animator.SetBool("InEnter", false);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_animator.SetBool("InDown", true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_animator.SetBool("InDown", false);
			//Если метод вызван вручную - пропускаем првоерку
			if (eventData == null) return;
			if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, eventData.position)) _onClick.Invoke();
		}

		private void InitInternalData()
		{
			_defaultGlarePosition = _glareImage.anchoredPosition;
			_defaultTextColor = _text.color;
			_defaultTextSize = _text.fontSize;
			_isInit = true;
		}

		private void Start()
		{
			if (!_isInit) InitInternalData();
		}
	}
}
