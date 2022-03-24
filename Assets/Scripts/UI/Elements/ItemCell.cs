using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class ItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
		[SerializeField]
		private Image _background;
		[SerializeField]
		private ItemElement _item;

		public bool EnterCursor { get; set; }
		public float EnterFill { get; set; }
		public Color BackgroundColor { get => _background.color; set => _background.color = value; }


		public bool IsEmpty => !_item.gameObject.activeSelf;

		public ItemElement Item 
		{
			get => _item;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			EnterCursor = true;
			//if (_recolorCoroutine == null) _recolorCoroutine = StartCoroutine(Recoloring());
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			EnterCursor = false;
			//if (_recolorCoroutine == null) _recolorCoroutine = StartCoroutine(Recoloring());
		}

		//private Coroutine _recolorCoroutine;
		//[SerializeField]
		//private Color _exitColor;
		//[SerializeField]
		//private Color _enterColor;
		//[SerializeField, Range(0.01f, 3f)]
		//private float _speedFilling = 0.25f;

		/*private IEnumerator Recoloring()
		{
			while(true)
			{
				//При наведенном курсоре заполнение стремится к 1
				//При убранном курсоре заполнение стремится к 0
				if ((EnterCursor && EnterFill >= 1f) || (!EnterCursor && EnterFill <= 0f)) break;

				EnterFill += TimeAssistant.UIDeltaTime * (EnterCursor ? _speedFilling : -_speedFilling);
				_background.color = Color.Lerp(_exitColor, _enterColor, EnterFill);
				yield return null;
			}

			_recolorCoroutine = null;
		}*/
	}
}
