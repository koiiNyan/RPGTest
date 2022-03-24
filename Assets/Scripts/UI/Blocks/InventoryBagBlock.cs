using RPG.Assistants;
using RPG.UI.Elements;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace RPG.UI.Blocks
{
	public partial class InventoryBagBlock : BaseTabBlock
	{
		private GameObjectPool<ItemCell> _pool;
		private Dictionary<ItemType, List<BaseItem>> _bags;

		[SerializeField]
		private ItemCell _prefab;
		[SerializeField]
		private Transform _parentWidget;
		[SerializeField]
		private RectTransform _content;
		[SerializeField, Range(250f, 3000f), SQRFloat]
		private float _minSwitchDistance = 1250f;

		public SimpleHandle<BaseItem, bool> OnEnterItemEventHandler;
		public SuccessHandler<BaseItem, ItemElement, Vector3> OnItemMoveEventHandler;

		private void Awake()
		{
			_pool = new GameObjectPool<ItemCell>(_prefab);
		}

		private async void Start()
		{
			await System.Threading.Tasks.Task.Yield();
			CreateTabs(_bags.Select(t => StringHelper.GetInventoryLocalization(t.Key)).ToArray());
		}

		protected override void ShowTable(int selectIndex)
		{
			ClearElements();
			var bag = _bags[(ItemType)selectIndex];//todo СТРОГО ОБЯЗАТЕЛЬНО, ЧТОБЫ ЭЛЕМЕНТЫ В СЛОВАРЕ РАСПОЛАГАЛИСЬ В ПОРЯДКЕ ПЕРЕЧИСЛЕНИЯ (ПОФИКСИТЬ ЭТО)

			for(int i = 0; i < bag.Count; i++)
			{
				var element = _pool.GetOrCreateElement(out var isNew);
				if (isNew)
				{
					element.transform.SetParent(_content, false);
					element.Item.Construct(_parentWidget);
					element.Item.OnEndDragEventHandler += OnMoveItem;
					element.Item.OnEnterItemEventHandler += OnEnterItemEventHandler;//todo
					element.Item.Position = i;//можно и вне ифа оставить
				}
				element.Item.Content = bag[i];
			}
		}

		private void OnMoveItem(ItemElement item, Vector3 position)
		{
			//Проверка, что предмет вынесен за пределы сумки
			if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, position))
			{
				item.ResetPosition();

				var type = item.Content.Type;
				var result = OnItemMoveEventHandler?.Invoke(item, position);

				item.Content = result;
				_bags[type][item.Position] = result;
				return;
			}

			//Ищем ближайший слот для нового расположения
			ItemCell slot = null;
			foreach(var el in _pool)
			{
				var distance = Vector3.SqrMagnitude(el.transform.position - position);
				if (RectTransformUtility.RectangleContainsScreenPoint(el.transform as RectTransform, position) && distance < _minSwitchDistance)
				{
					slot = el;
					break;
				}
			}

			//Произошел обмен в ячейках
			if (slot != null)
			{
				var list = _bags[item.Content.Type];

				var swap = slot.Item.Content;
				var value = list[slot.Item.Position];

				slot.Item.Content = item.Content;
				list[slot.Item.Position] = list[item.Position];

				item.Content = swap;
				list[item.Position] = value;
			}

			item.ResetPosition();
		}

		public void Construct(Dictionary<ItemType, List<BaseItem>> bags) => _bags = bags;

		private void ClearElements()
		{
			foreach (var element in _pool)
			{
				element.Item.Content = null;
			}

			_pool.DisableAllElements();
		}

		protected override void OnDestroy()
		{
			foreach(var element in _pool)
			{
				element.Item.OnEndDragEventHandler -= OnMoveItem;
			}
			base.OnDestroy();
		}
	}
}