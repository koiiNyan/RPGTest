using RPG.ScriptableObjects;
using RPG.ScriptableObjects.Contexts;
using RPG.UI.Blocks;
using RPG.UI.Elements;
using RPG.Units.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;

using UnityEngine;

using Zenject;

namespace RPG.UI
{
    public class WidgetInventories : MonoBehaviour, IClosableWidget
    {
        [Inject]
        private PlayerUnit _player;
        [Inject]
        private InterfaceInteractSettings _interactSettings;

        [SerializeField]
        private InventoryBagBlock _inventory;
        [SerializeField]
        private EquipmentDictionary _equipment;
        [SerializeField]
        private RectTransform _dropIcon;

        [Space, Header("---Всплывающее окно информации---"), SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private RectTransform _popup;
        [SerializeField]
        private TextMeshProUGUI _popupName;
        [SerializeField]
        private TextMeshProUGUI _popupDescription;

        public event SimpleHandle<ItemElement> OnChangeEquipEventHandler;
        public event SimpleHandle OnCloseWidgetEventHandler;

        private void Start()
		{
            _inventory.SetIterfaceParams(_interactSettings.ExitColor, _interactSettings.EnterColor, _interactSettings.SpeedFilling);
            _inventory.Construct(_player.Inventory);
            _inventory.OnItemMoveEventHandler += OnEquip;
            _inventory.OnEnterItemEventHandler += ShowDescriptionPopup;
            
            foreach(var element in _equipment.Values)
            {
                element.Item.Construct(transform);
                element.Item.OnStartDragEventHandler += UnequipItem;
                element.Item.OnEnterItemEventHandler += ShowDescriptionPopup;//todo
			}
		}

        public BaseItem SetEquipment(EquipmentItem item)
        {
            var oldEquip = _equipment[item.Equipment].Item.Content;
            _equipment[item.Equipment].Item.Content = item;
            //Изменение сета снаряжения игрока
            _player.Set.ChangeEquipment(item);
            return oldEquip;
        }

        public void ForceRemoveEquipments()
        {
            foreach(var equipment in _equipment.Values)
            {
                equipment.Item.Content = null;
			}
		}

        private BaseItem OnEquip(ItemElement element, Vector3 position)
        {
            //Предмет перенесен в корзину
            if(RectTransformUtility.RectangleContainsScreenPoint(_dropIcon, position)) return null;

            var equip = element.Content as EquipmentItem;
            //Предмет никуда не был перенесен
            if (equip == null) return element.Content;
            
            //Снаряжение оружием персонажа
            if(RectTransformUtility.RectangleContainsScreenPoint(_equipment[equip.Equipment].transform as RectTransform, position))
            {
                return SetEquipment(equip);
            }

            return element.Content;
        }
        
        private void UnequipItem(ItemElement element)
        {
            _player.AddItem(element.Content);
            var equip = element.Content as EquipmentItem;
            //Изменение сета снаряжения игрока
            _player.Set.ChangeEquipment(null, equip.Equipment);
            element.Content = null;
            _inventory.ForceUpdateBlock();
            ShowDescriptionPopup(null, false);
        }

        public void ShowDescriptionPopup(BaseItem item, bool isShow)
        {
            if (isShow)
            {
                _popup.gameObject.SetActive(true);
            }
            else
            {
                _popup.gameObject.SetActive(false);
                return;
            }

            var point = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            _popup.position = RectTransformUtility.PixelAdjustPoint(point, transform as RectTransform, _canvas);
            _popupName.text = item.ID;

            var str = new StringBuilder();

            //Худ. описание
            str.Append($"{item.DescriptionID}\n");
            //Перечисление бонусов
            foreach (var status in item.Statuses)
            {
                str.Append($"\n<color=#FFF500>{StringHelper.GetStatsLocalization(status.Type)}:</color> {status.Data.Value}%");
            }

            _popupDescription.text = str.ToString();
        }

        public void OnSort_UnityEvent()
        {
            foreach(var bag in _player.Inventory.Values)
            {
                bag.Sort(Sorter);
			}

            _inventory.ForceUpdateBlock();
		}

        private int Sorter(BaseItem a, BaseItem b)
        {
            if(a == null)
            {
				return b == null ? 0 : 1;
			}
            else
            {
                if (b == null) return -1;

                var count = a.ID.Length > b.ID.Length ? b.ID.Length : a.ID.Length;
                for(int i = 0; i < count; i++)
                {
                    if (i >= a.ID.Length) return -1;
                    if (i >= b.ID.Length) return 1;

                    if (a.ID[i] == b.ID[i]) continue;

                    if (a.ID[i] < b.ID[i]) return -1;
                    if (a.ID[i] > b.ID[i]) return 1;
				}
			}

            throw new ApplicationException("Sort error");
		}

        public void OnClose_UnityEvent() => OnCloseWidgetEventHandler?.Invoke();

		private void OnDestroy()
		{
            _inventory.OnItemMoveEventHandler -= OnEquip;
            foreach (var element in _equipment.Values)
            {
                element.Item.OnStartDragEventHandler -= UnequipItem;
            }
        }
	}
}