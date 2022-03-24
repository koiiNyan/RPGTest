using RPG.Commands;
using RPG.ScriptableObjects.Effects;
using RPG.Triggers;
using RPG.UI;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Zenject;

namespace RPG.Managers
{
	public class TriggerManager : MonoBehaviour, IManager
	{
		private LinkedList<TriggerComponent> _effectTriggers = new LinkedList<TriggerComponent>();
		private LinkedList<QuestTriggerComponent> _questTriggers = new LinkedList<QuestTriggerComponent>();

		[Inject]
		private WidgetQuests _quests;

		[SerializeField]
		private Transform _triggerPool;

		public void UpdateQuestState(IdentificatorParagraphData data)
		{
			var list = new LinkedList<IdentificatorParagraphData>();
			list.AddLast(data);
			_quests.TryToUpdateParagraph(list);// UpdateQuest(data);

			var quests = _questTriggers.Where(t => t.GetContextID == data.QuestID && t.GetParagraphID == data.ParagraphID);

			foreach (var trigger in quests) trigger.UpdateTriggerState();
		}

		public void ActivateQuest(string contextID, string paragraphID)
		{
			var quests = _questTriggers.Where(t => t.GetContextID == contextID && t.GetParagraphID == paragraphID);
			foreach (var quest in quests) quest.Enable = true;
		}

		private void Start()
		{
			#region
#if UNITY_EDITOR
			var triggers = _triggerPool.GetComponentsInChildren<Transform>().Where(t=> t != _triggerPool);

			foreach (var trigger in triggers)
			{
				var trig = trigger.GetComponent<TriggerComponent>();
				if (trig == null)
				{
					Editor.EditorExtensions.ConsoleLog($"the object {trig.name} is not a trigger");
				}
			}

			foreach(var trigger in _triggerPool.GetComponentsInChildren<TriggerComponent>())
			{
				var tr = trigger as QuestTriggerComponent;
				if (tr == null) continue;

				tr.gameObject.layer = LayerMask.NameToLayer(Editor.EditorConstants.PlayerTriggerLayer);
			}

#endif
			#endregion

			foreach (var trigger in _triggerPool.GetComponentsInChildren<TriggerComponent>())
			{
				var tr = trigger as QuestTriggerComponent;

				if (tr != null) _questTriggers.AddLast(tr);
				else _effectTriggers.AddLast(trigger);
			}

			_quests.OnAddQuestEventHandler += ActivateQuest;
		}

		private void OnDestroy()
		{
			_quests.OnAddQuestEventHandler -= ActivateQuest;
		}

		public void Configuration()
		{
			if (!Application.isEditor) return;
		}

		[Inject]
		private void ConstructSignalBus(SignalBus signal)
		{
			signal.SubscribeId<(IdentificatorParagraphData, ParagraphStateType)>(Constants.LoadIdentifier, (data) =>
			{
				switch (data.Item2)
				{
					case ParagraphStateType.Inactive:
						break;
					case ParagraphStateType.Active:
						ActivateQuest(data.Item1.QuestID, data.Item1.ParagraphID);
						break;
					case ParagraphStateType.Completed:
						UpdateQuestState(data.Item1);
						break;
					case ParagraphStateType.Failed:
						throw new System.NotImplementedException();
				}
			});
		}
	}
}