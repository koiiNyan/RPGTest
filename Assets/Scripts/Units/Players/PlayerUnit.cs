
using RPG.Commands;
using RPG.Managers;
using RPG.UI.Blocks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RPG.Units.Player
{
	[RequireComponent(typeof(PlayerInputComponent), typeof(PlayerMoveComponent), typeof(PlayerStateComponent))]
	public partial class PlayerUnit : Unit
	{
        private CameraComponent _camera;

#pragma warning restore 0649
#pragma warning restore IDE0044

        [Inject]
        private CursorBlock _focus;
        [Inject]
        private UIManager _ui;

#pragma warning restore IDE0044
#pragma warning restore 0649

        public override bool CanSpeak => Set.CurrentStyle == WeaponStyleType.None;

        public Dictionary<InputSetType, BaseCommandSkill> SkillSet { get; private set; } = new Dictionary<InputSetType, BaseCommandSkill>();
        public Dictionary<ItemType, List<BaseItem>> Inventory { get; private set; } = new Dictionary<ItemType, List<BaseItem>>();

        /// <summary>
        /// Оповещение о получении предмета игроком. МОжет пригодится для выполнения условий заданий
        /// </summary>
        public event SimpleHandle<string> UpdateInventoryEventHandler;

        public bool AddItem(BaseItem item, int index = -1)
        {
#if UNITY_EDITOR
            if (item == null) Editor.EditorExtensions.ConsoleLog("Cannot add <b>null</b> item to player bag", Editor.PriorityMessageType.Critical);
#endif
            UpdateInventoryEventHandler?.Invoke(item.ID);
            //Ищем дубликат предмета в сумке
            BaseItem it = null;
            for (int i = 0; i < Inventory[item.Type].Count; i++)
            {
                if (Inventory[item.Type][i] != item) continue;
                it = Inventory[item.Type][i];
            }

            //Если в сумке есть такой же предмет, он стакается и стак не полный
            if(it != null && !it.Unstackable && it.Count < it.MaxStack)
            {
                it.Count++;
                return true;
			}

            if(index != -1)
            {
#if UNITY_EDITOR
                if (Inventory[item.Type][index] != null) Editor.EditorExtensions.ConsoleLog($"There was already an item in bag <b>{item.Type}</b> in slot <b>{index}</b>, it was replaced, make sure this is the normal behavior of the application", Editor.PriorityMessageType.Notification);
#endif
                if (Inventory[item.Type][index] == null)
                {
                    Inventory[item.Type][index] = item;
                    return true;
                }
            }

            //Добавляем новый предмет в сумку
            for (int i = 0; i < Inventory[item.Type].Count; i++)
            {
                if (Inventory[item.Type][i] == null)
                {
                    Inventory[item.Type][i] = item;
                    return true;
                }
            }

            return false;
        }

        public void RemoveItemByID(string id, int count = 1, ItemType? type = null)
        {
            List<BaseItem> bag = null;
            BaseItem item = null;

            var finding = new Action<List<BaseItem>>((list) =>
            {
                foreach (var i in list)
                {
                    if (i.ID != id) continue;
                    bag = list;
                    item = i;
                    break;
                }
            });

            if (type != null) bag = Inventory[type.Value];
            else
            {
                foreach(var b in Inventory)
                {
                    finding(b.Value);
                    if (bag != null) break;
                }
			}

            item.Count -= count;
            if (item.Count <= 0) bag.Remove(item);
        }

        private void OnInteract()
        {
            if (Set.InArmed || !_focus.CanInteract) return;

            _camera.CameraPosition = CameraPositionType.Dialogue;
            Target = _focus.LastFocusUnit;
            _ui.CallInteract(_focus.LastFocusUnit);
		}

        public void ResetFocus()
        {
            Target = null;
            _camera.CameraPosition = CameraPositionType.Default;
		}

        private void OnCast(InputSetType input)
        {
            if (Move.LockMovement || input == InputSetType.None || !SkillSet.ContainsKey(input)) return;

            State.Effects.AddEffect(SkillSet[input].Clone());
		}

        protected override void FindNewTarget()
		{
            var units = _npcManager.GetNPCs;

            var distance = _sqrFindTargetDistance;
            Target = null;
            foreach (var unit in units)
            {
                if (unit.State.Side == State.Side) continue;

                var currentDistance = (unit.transform.position - transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    Target = unit;
                }
            }
        }

		protected override void Start()
		{
            _camera = FindObjectOfType<CameraComponent>();
            var callback = gameObject.AddComponent<PlayerAnimationCallbackComponent>();

            NumberID = Constants.PlayerNumberID;

            //Конфигурация сумки
            var size = Constants.PlayerInventorySize.x * Constants.PlayerInventorySize.y;
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                var list = new List<BaseItem>(size);
                for (int i = 0; i < size; i++)
                {
                    list.Add(null);
                }
                Inventory.Add(type, list);
            }
            base.Start();

            AddItem(Assistants.ConstructEntityExtensions.FindItem("Старый меч"));//todo
        }

        protected override void BindingEvents(bool unbind = false)
        {
            base.BindingEvents(unbind);

            var inputs = (PlayerInputComponent)Input;
            var move = (PlayerMoveComponent)Move;
            if (unbind)
            {
                inputs.ArmedEventHandler -= () => Set.InArmed = !Set.InArmed;
                inputs.Skill1EventHandler -= () => OnCast(InputSetType.InputE); 
                move.Binding(unbind: true);
                inputs.MainEventHandler -= OnInteract;
                Set.OnAddStatusWeaponEventHandler -= State.Statuses.AddStatuses;
                Set.OnRemoveStatusWeaponEventHandler -= State.Statuses.RemoveStatuses;
                return;
            }

            inputs.ArmedEventHandler += () => Set.InArmed = !Set.InArmed;
            inputs.Skill1EventHandler += () => OnCast(InputSetType.InputE);
            move.Binding(unbind: false);
            inputs.MainEventHandler += OnInteract;
            Set.OnAddStatusWeaponEventHandler += State.Statuses.AddStatuses;
            Set.OnRemoveStatusWeaponEventHandler += State.Statuses.RemoveStatuses;

        }
    }
}
