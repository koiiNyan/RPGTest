using RPG.ScriptableObjects.Contexts;
using RPG.UI;
using RPG.Units.NPCs;
using RPG.Units.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace RPG.Managers
{
	public class UIManager : MonoBehaviour, IManager
    {
        private Dictionary<WidgetType, GameObject> _widgets;

#pragma warning restore IDE0044
#pragma warning disable IDE0090

        [Inject]
        private UnitManager _unitManager;
        [Inject]
        private PlayerControls _controls;
        [Inject]
        private PlayerUnit _player;

#pragma warning restore IDE0090
#pragma warning restore IDE0044

        [SerializeField]
        private GameObject _pools;

        [Space, SerializeField]
        private WidgetMenu _menu;
        [SerializeField]
        private WidgetBattle _battle;
        [SerializeField]
        private WidgetDialogues _dialogue;
        [SerializeField]
        private WidgetQuests _quest;
        [SerializeField]
        private WidgetSkills _level;
        [SerializeField]
        private WidgetInventories _inventory;

        [Space, SerializeField]
        private ConversationContext[] _conversations;
        [Space, SerializeField]
        private PhraseSpriteDictionary _phraseSprites;

        public void CallInteract(NPCUnit interactUnit)
        {
            if (interactUnit.SpeakingQueue.Count == 0) return;

            SwitchWidget(WidgetType.Dialogues);

            var id = interactUnit.SpeakingQueue.Peek();
            var conversation = _conversations.First(t => t.ID == id);
            //Удаляем беседу, если она больше не воспроизводится
            if (!conversation.IsLoop) interactUnit.SpeakingQueue.Dequeue();
            _dialogue.SetContext(interactUnit, conversation.Context);
        }

        private void CancelInteract()
        {
            SwitchWidget(WidgetType.Battle);
            _player.ResetFocus();
        }

        private void OpenTrade() 
        {
            Debug.Log("Open Trade!");
        }
        private void UpdateQuest()
        {
            var npc = _player.Target as NPCUnit;
            CancelInteract();
            //Нет ни квестов, ни пунктов квестов у бота

            if (npc.ParagraphList.Count == 0 && npc.QuestQueue.Count == 0) return;

            if (_quest.TryToUpdateParagraph(npc.ParagraphList))
            {
                return;
            }
            //У игрока не оказалось активных параграфов бота - берем его задание
            else if (!_quest.HasActiveQuest(npc.ParagraphList) && npc.QuestQueue.Count != 0)
            {
                _quest.AddQuest(npc.QuestQueue.Dequeue());
            }
        }

        private void OnUpdateUI(PhraseType type)
        {
			switch (type)
			{
				case PhraseType.None:
                    CancelInteract();
                    break;
				case PhraseType.Phrase:
                    throw new ApplicationException($"Пришло событие, которое не должно приходить: <b>{type}</b>");
				case PhraseType.Answers:
                    throw new ApplicationException($"Пришло событие, которое не должно приходить: <b>{type}</b>");
				case PhraseType.Trade:
                    CancelInteract();
                    OpenTrade();
					break;
				case PhraseType.Quest:
                    UpdateQuest();
					break;
			}
		}

		void Start()
        {
            _widgets = new Dictionary<WidgetType, GameObject>
            {
                { WidgetType.Battle, _battle.gameObject },
                { WidgetType.Dialogues, _dialogue.gameObject },
                { WidgetType.Inventories, _inventory.gameObject },
                { WidgetType.Quests, _quest.gameObject },
                { WidgetType.Skills, _level.gameObject },
                { WidgetType.Menu, _menu.gameObject }
            };

            _unitManager.DieBotEventHandler += _quest.TryToUpdateCondition;

            SwitchWidget(WidgetType.Battle);

            _dialogue.OnUpdateDialogueStateEventHandler += OnUpdateUI;
			_controls.Unit.Quests.performed += OnShowQuests;
            _controls.Unit.Levels.performed += OnShowLevels;
            _controls.Unit.Inventory.performed += OnShowInventory;
            _controls.Unit.Pause.performed += OnShowPause;

            _quest.OnCloseWidgetEventHandler += SwitchWidget;
            _quest.OnQuestCompletedEventHandler += AddRewards;
            _level.OnCloseWidgetEventHandler += SwitchWidget;
            _inventory.OnCloseWidgetEventHandler += SwitchWidget;
            _menu.OnCloseWidgetEventHandler += SwitchWidget;
            _menu.OnLoadingGameEventhandler += PrepareForLoadGame;
            _menu.OnLoadedGameEventHandler += UpdateGameAfterLoad;
        }

        private void AddRewards(string id, List<string> rewards)
        {
            foreach(var reward in rewards)
            {
                var strs = reward.Split('%');
                var count = int.Parse(strs[1]);

                if (strs[0] == "Exp") (_player.State as PlayerStateComponent).AddExp(count);
                else if (strs[0] == "Gold") return;//todo
                else _player.AddItem(Assistants.ConstructEntityExtensions.FindItem(strs[0]));
            }

            //todo доделать систему итемов и выдавать награду предметами и золотом
            var exps = rewards.FirstOrDefault(t => t.Contains("Exp"))?.Split('%');

            if(exps != null && exps.Length == 2)
            {
                (_player.State as PlayerStateComponent).AddExp(int.Parse(exps[1]));//todo
            }
		}

        private void OnShowQuests(UnityEngine.InputSystem.InputAction.CallbackContext obj) => SwitchWidget(WidgetType.Quests);
        private void OnShowLevels(UnityEngine.InputSystem.InputAction.CallbackContext obj) => SwitchWidget(WidgetType.Skills);
        private void OnShowInventory(UnityEngine.InputSystem.InputAction.CallbackContext obj) => SwitchWidget(WidgetType.Inventories);
        private void OnShowPause(UnityEngine.InputSystem.InputAction.CallbackContext obj) => SwitchWidget(WidgetType.Menu);

        private void SwitchWidget() => SwitchWidget(WidgetType.Battle);
        private void SwitchWidget(WidgetType type)
        {
            foreach(var pair in _widgets)
            {
                pair.Value.SetActive(type == pair.Key);
			}

            if(type == WidgetType.Battle)
            {
                _controls.Unit.Enable();
                _controls.Camera.Enable();
#if !UNITY_EDITOR
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
#endif
                _controls.UI.Disable();
            }
            else
            {
                _controls.Unit.Disable();
                _controls.Camera.Disable();
#if !UNITY_EDITOR
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
#endif
                _controls.UI.Enable();
            }
        }

        private void OnDestroy()
		{
            _unitManager.DieBotEventHandler -= _quest.TryToUpdateCondition;

            _dialogue.OnUpdateDialogueStateEventHandler -= OnUpdateUI;
            _controls.Unit.Quests.performed -= OnShowQuests;
            _controls.Unit.Levels.performed -= OnShowLevels;
            _controls.Unit.Inventory.performed -= OnShowInventory;
            _quest.OnCloseWidgetEventHandler -= SwitchWidget;
            _quest.OnQuestCompletedEventHandler -= AddRewards;
            _level.OnCloseWidgetEventHandler -= SwitchWidget;
            _inventory.OnCloseWidgetEventHandler -= SwitchWidget;
            _menu.OnCloseWidgetEventHandler -= SwitchWidget;
            _menu.OnLoadingGameEventhandler -= PrepareForLoadGame;
            _menu.OnLoadedGameEventHandler -= UpdateGameAfterLoad;
        }

        private void PrepareForLoadGame()
        {
            _battle.ClearVisual();
            _pools.SetActive(false);
        }

        private void UpdateGameAfterLoad()
        {
            _pools.SetActive(true);
		}

        [ContextMenu("Configuration")]
        public void Configuration()
        {
            if (!Application.isEditor) return;

#if UNITY_EDITOR
            var contexts = RPGExtensions.FindAllAssetsByType<ConversationContext>();

            _conversations = contexts.ToArray();
            foreach (var context in contexts)
            {
                var pair = context.GetAllAnswerContexts();

                foreach (var answer in pair.Item1)
                {
                    _phraseSprites.TryGetValue(answer.Type, out var sprite);
                    answer.Sprite = sprite;
                    sprite = default(Sprite);
                }
                UnityEditor.EditorUtility.SetDirty(context);
            }

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        [Inject]
        private void ConstructSignalBus(SignalBus signal)
        {
            signal.SubscribeId<LinkedList<EquipmentItem>>(Constants.LoadIdentifier, (list) =>
            {
                _inventory.ForceRemoveEquipments();
                foreach (var element in list)
                {
                    _inventory.SetEquipment(element);
                }
            });
		}
    }
}
