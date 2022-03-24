using Newtonsoft.Json.Linq;

using RPG.Units.Items;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.Units
{
    [SaveData("sets", 2)]
    public partial class WeaponSetComponent : UnitComponent, IStateRecorder
    {
        private bool _inArmed;
        private WeaponStyleType _style;

        [SerializeField]
        private EquipmentSetDictionary _sets;

        public event SimpleHandle<List<Status>> OnAddStatusWeaponEventHandler;
        public event SimpleHandle<List<Status>> OnRemoveStatusWeaponEventHandler;

        public WeaponStyleType CurrentStyle => _style;

        public bool InArmed 
        {
            get => _inArmed;
            set
            {
                if (_inArmed == value) return;
                _inArmed = value;

                var instance = _sets[EquipmentType.MainHand].Instance as WeaponComponent;
                if (instance == null) return;

                //Смена слоев и типа стиля персонажа
                Owner.Animator.SetLayerWeight(Constants.GetLayersInts[_style], 0f);

                _style = value
                    ? instance.Style
                    : WeaponStyleType.None;

                Owner.Animator.SetLayerWeight(Constants.GetLayersInts[_style], 1f);

                instance.Activity = value;
                if (_sets[EquipmentType.AdditionalHand].Instance != null) _sets[EquipmentType.AdditionalHand].Instance.Activity = value;
            }
        }

        public void ChangeEquipment(EquipmentItem item, EquipmentType type = default)
        {
            if (item != null) type = item.Equipment;

            var set = _sets[type];

            //Удаление старого эквипа и его статусов
            if (set.Instance != null)//Unity автоинициализирует простые классы, поэтому проверка на set.Item != null в сериализованных полях всегда ложна
            {
                if (set.Instance is WeaponComponent w) w.OnCollisionEnterEventHandler -= OnWeaponCollision;

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) DestroyImmediate(set.Instance.gameObject);
                else Destroy(set.Instance.gameObject);
#else
                Destroy(set.Instance.gameObject);//Уничтожение объекта в сцене
#endif
                set.Instance = null;//Удаление визуала
                if (set.Item.Statuses.Count != 0) OnRemoveStatusWeaponEventHandler?.Invoke(set.Item.Statuses);//удаление статусов
                set.Item = new EquipmentItem();//Удаление информации
            }

            //Если эквип просто снят, без переодевания - выходим
            if (item == null) return;

            //Добавление нового эквипа и его статусов
            set.Instance = Instantiate(item.Prefab, set.ParentBone);//Создание объекта в сцене & Добавление визуала
            set.Instance.Equipment = item.Equipment;
            set.Item = item;//Добавление информации
            if (set.Item.Statuses.Count != 0) OnAddStatusWeaponEventHandler?.Invoke(set.Item.Statuses);//добавление статусов

            //Доп обработка оружия
            if (set.Instance is WeaponComponent weapon)
            {
                //Выключение оружия, если персонаж не вооружен
                if (!InArmed) set.Instance.Activity = false;

                weapon.OnCollisionEnterEventHandler += OnWeaponCollision;
            }
        }

        public void ColliderStateChange(string equipmentType, bool isActive)
        {
#if UNITY_EDITOR
            if (!Enum.TryParse<EquipmentType>(equipmentType, out var type)) Editor.EditorExtensions.ConsoleLog($"The animation asks for a non-existent value from the <b>{equipmentType}<b> enumeration <b>{nameof(EquipmentType)}</b>", Editor.PriorityMessageType.Critical);
#else
            var type = (EquipmentType)Enum.Parse(typeof(EquipmentType), equipmentType);
#endif

            (_sets[type].Instance as WeaponComponent).Collider = isActive;
        }

        private void OnWeaponCollision(GameObject obj, EquipmentType type)
        {
            var unit = obj.GetComponentInParent<Unit>();

            if (unit == null) return;

            float damage = Owner.State.Parameters[StatsType.Damage];

            if(unit.State.Parameters.TryGetParameter(StatsType.ArmorMult, out var armor))
            {
                damage = (1 - armor / 100f) * damage;
			}

            unit.State.Parameters.Health -= damage;
        }

		private async void Start()
		{
            await System.Threading.Tasks.Task.Yield();//todo temp fix event binding for NPCUnit

			if(GetComponent<NPCs.NPCUnit>() != null)
            {
                var set = _sets[EquipmentType.MainHand];
                if (set.Instance == null) return;
                
                InArmed = true;
                (set.Instance as WeaponComponent).OnCollisionEnterEventHandler += OnWeaponCollision;

                set = _sets[EquipmentType.AdditionalHand];
                if (set.Instance == null) return;
                (set.Instance as WeaponComponent).OnCollisionEnterEventHandler += OnWeaponCollision;
            }
		}

#if UNITY_EDITOR
        [ContextMenu("Set Equip Params by ID")]
        private void SetEquipParamsByID()
        {
            var contexts = RPGExtensions.FindAllAssetsByType<ScriptableObjects.Contexts.EquipmentItemContext>();

            var fieldInfo = typeof(NPCs.NPCUnit).GetField("_equipments", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var equipment = fieldInfo.GetValue(GetComponent<NPCs.NPCUnit>()) as string[];

            EquipmentItem item = null;
            foreach(var equip in equipment)
            {
                foreach (var context in contexts)
                {
                    item = context.Items.FirstOrDefault(t => t.ID == equip);

                    if(item != null)
                    {
                        try
                        {
                            ChangeEquipment(item);
                        }
                        catch { }

                        _sets[item.Equipment].Instance.Activity = true;
                    }
                }
			}
        }

        [ContextMenu("Clear NPC Equipments")]
        private void ClearNPCEquipments()
        {
            if (GetComponent<NPCs.NPCUnit>() == null) return;

            foreach(var set in _sets.Values)
            {
                if(set.Instance != null)
                {
                    DestroyImmediate(set.Instance.gameObject);
                    set.Item = new EquipmentItem();
				}
			}
		}
#endif
	}
}

