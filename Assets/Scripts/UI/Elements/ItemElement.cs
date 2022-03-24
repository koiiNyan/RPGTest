using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
	public class ItemElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private Coroutine _coroutine;
		private Transform _parent;
		private Transform _dragParent;
		private BaseItem _item;

		[SerializeField]
		private Image _icon;
		[SerializeField]
		private TextMeshProUGUI _count;

		public SimpleHandle<BaseItem, bool> OnEnterItemEventHandler;
		public SimpleHandle<ItemElement> OnStartDragEventHandler;
		public SimpleHandle<ItemElement, Vector3> OnEndDragEventHandler;

		public BaseItem Content
		{
			get => _item;
			set
			{
				_item = value;

				if (_item == null)
				{
					if(gameObject.activeSelf) gameObject.SetActive(false);
					return;
				}
				if (!gameObject.activeSelf) gameObject.SetActive(true);
				_icon.sprite = value.Icon;

				_count.text = value.Count < 2 ? string.Empty : value.Count.ToString();
			}
		}

		public int Position { get; set; }

		public void ResetPosition()
		{
			transform.SetParent(_parent, true);
			transform.localPosition = Vector3.zero;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
			else OnEnterItemEventHandler?.Invoke(null, false);

			if(OnStartDragEventHandler != null)
			{
				OnStartDragEventHandler.Invoke(this);
				return;
			}
			transform.SetParent(_dragParent, true);
			transform.SetAsLastSibling();
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.pointerEnter == null) return;

			RectTransform rect = eventData.pointerEnter.transform as RectTransform;
			if (rect == null) return;

			RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out var pos);
			transform.position = pos;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			RectTransform rect = eventData.pointerEnter.transform as RectTransform;
			if (rect != null)
			{
				RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out var pos);
				OnEndDragEventHandler?.Invoke(this, pos);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_coroutine = StartCoroutine(PopupDelay());
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
			else OnEnterItemEventHandler?.Invoke(Content, false);
		}

		private IEnumerator PopupDelay()
		{
			yield return new WaitForSeconds(Constants.DelayShowPopupInItem);
			OnEnterItemEventHandler?.Invoke(Content, true);
			_coroutine = null;
		}

		public void Construct(Transform dragParent)
		{
			_parent = transform.parent; _dragParent = dragParent;
		}
	}
}

