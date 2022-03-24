using RPG.Assistants;
using RPG.ScriptableObjects.Configurations;
using RPG.Units;
using RPG.Units.NPCs;
using RPG.Units.Player;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace RPG.Managers
{
	public class UnitManager : MonoBehaviour
	{
#pragma warning restore IDE0044
#pragma warning disable IDE0090

		private ulong _uniqueNumber = 2;

		private readonly LinkedList<NPCUnit> _bots = new LinkedList<NPCUnit>();
		private readonly DispatcherAssistant _dispatcher_A = new DispatcherAssistant();
		private DataStateAssistant _dataState_A;
		private ActivityAssistant _activity_A;
		private BotAssistant _bot_A;

		[Inject]
		private ConversationConfiguration _conversations;
		[Inject]
		private QuestConfiguration _quests;

#pragma warning restore IDE0090
#pragma warning restore IDE0044


		[SerializeField]
		private BotInfoData[] _botPrefabs;

		[Space, SerializeField, Range(1f, 100f), Tooltip("Расстояние до игрока для активации логики бота")]
		private float _distanceActivation = 40f;
		[SerializeField, Range(1f, 100f), Tooltip("Интервал проверки активности ботов")]
		private float _activityCheckDelay = 2f;
		[SerializeField, Range(1f, 100f), Tooltip("Расстояние триггера бота на игрока")]
		private float _targetFocusDistance = 15f;
		[SerializeField, Range(1f, 100f), Tooltip("Расстояние, пройдя которое, бот возвращается к своим задачам")]
		private float _resetDistance = 15f;

		[Space, SerializeField]
		private Transform _npcPool;

		public IReadOnlyCollection<NPCUnit> GetNPCs => _bots;

		public bool InFight => _activity_A.InFight;

		[Inject]
		public PlayerUnit GetPlayer { get; private set; }

		public event SimpleHandle<string> DieBotEventHandler;

		private void Update()
		{
			_dispatcher_A.OnUpdate();
			_dataState_A.OnUpdate(TimeAssistant.GameDeltaTime);
			_bot_A.OnUpdate(TimeAssistant.GameDeltaTime);
		}

		private async void Start()
		{
			#region
#if UNITY_EDITOR
			var units = _npcPool.GetComponentsInChildren<Unit>();



			foreach (var npc in units)
			{
				var bot = npc as NPCUnit;
				if (bot == null)
				{
					Editor.EditorExtensions.ConsoleLog($"the unit {npc.name} is not a NPC");
				}
			}
#endif
			foreach (var bot in _npcPool.GetComponentsInChildren<NPCUnit>()) _bots.AddLast(bot);
#if UNITY_EDITOR
			if (FindObjectsOfType<NPCUnit>().Length != _bots.Count)
				Editor.EditorExtensions.ConsoleLog($"NPC must be in a <b>{_npcPool.name}</b> pool", Editor.PriorityMessageType.Critical);
#endif
			#endregion

			_dataState_A = new DataStateAssistant(_bots, GetPlayer);
			_bot_A = new BotAssistant(_bots);
			await Task.Yield();

			_activity_A = new ActivityAssistant(_bots, GetPlayer);
			StartCoroutine(_activity_A.CheckActivity(_distanceActivation, _activityCheckDelay, _targetFocusDistance, _resetDistance));

			foreach (NPCUnit unit in _bots)
			{
				RegistrationUnit(unit);
				ConfigurationUnit(unit);
			}

			TimeAssistant.OnGameScaleChangeEventHandler += ForceNPCStopped;
		}

		private void ForceNPCStopped(float value)
		{
			var boolean = value == 0f;
			foreach(var bot in _bots)
			{
				var input = bot.Input as NPCInputComponent;
				if(input.Activity) input.ForceDisable = boolean;
			}
		}

		public Unit CreateNPC(BotPrefabType type, string id, ulong number)
		{
			var prefab = 
#if UNITY_EDITOR
				_botPrefabs.FirstOrDefault(t => t.BotPrefabType == type);
			if (prefab.Prefab == null) Editor.EditorExtensions.ConsoleLog($"No unit prefab with type <b>{type}</b> was found among the Resources", Editor.PriorityMessageType.Critical);
#else
				_botPrefabs.First(t => t.BotPrefabType == type);
#endif

			var instance = Instantiate(prefab.Prefab, _npcPool);
			instance.SetID = id;

			if (number != 0) instance.NumberID = number;

			_dispatcher_A.AddAction(() =>
			{
				_bots.AddLast(instance);
				RegistrationUnit(instance);
			});		

			return instance;
		}

		public void RemoveNPC(Unit unit)
		{
			unit.Enable = false;
			_bots.Remove(unit as NPCUnit);
			unit.Animator.SetTrigger("Die");
			unit.State.OnDyingEventHandler -= RemoveNPC;


			Destroy(unit.gameObject, 10f);//todo
			DieBotEventHandler?.Invoke(unit.GetID);
		}

		public void ConfigurationUnit(NPCUnit unit)
		{
			unit.SpeakingQueue = _conversations.GetQueueByID(unit.GetID);
			unit.QuestQueue = _quests.GetQuestByID(unit.GetID);
			unit.ParagraphList = _quests.GetParagraphByID(unit.GetID);
		}

		private void RegistrationUnit(NPCUnit unit)
		{
			if (unit.NumberID == 0)
			{
				unit.NumberID = _uniqueNumber;
				_uniqueNumber++;
			}
			else if (unit.NumberID > _uniqueNumber) _uniqueNumber = unit.NumberID + 1;

			unit.Enable = true;
			unit.ConfigurationAIController();
			unit.State.OnDyingEventHandler += RemoveNPC;
		}

		private void OnDestroy()
		{
			foreach(var unit in new LinkedList<NPCUnit>(_bots))
			{
				RemoveNPC(unit);
			}
			TimeAssistant.OnGameScaleChangeEventHandler -= ForceNPCStopped;
		}

#if UNITY_EDITOR

		private bool _isShow;

		[ContextMenu("Show Ranges")]
		private void ShowRanges()
		{
			_isShow = !_isShow;
		}

		private void OnDrawGizmos()
		{
			if (!_isShow) return;

			var obj = UnityEditor.Selection.activeGameObject;
			if (obj == null) return;
			var npc = obj.GetComponent<NPCInputComponent>();
			if (npc == null) return;

			Gizmos.color = new Color(0.5f, 1f, 0.7f, 0.3f);
			Gizmos.DrawSphere(npc.transform.position, _distanceActivation);
			var newPos = npc.transform.position;
			newPos.y += _distanceActivation / 2f;
			newPos.x += _distanceActivation / 2f;
			GUI.color = new Color(0.5f, 1f, 0.7f);
			UnityEditor.Handles.Label(newPos, "DistanceActivation: " + _distanceActivation, Editor.EditorConstants.GizmosSmallLabelStyle);

			Gizmos.color = new Color(1f, 0.99f, 0.59f, 0.3f);
			Gizmos.DrawSphere(npc.transform.position, _targetFocusDistance);
			newPos = npc.transform.position;
			newPos.y += _targetFocusDistance / 2f;
			newPos.x += _targetFocusDistance / 2f;
			GUI.color = new Color(1f, 0.99f, 0.59f);
			UnityEditor.Handles.Label(newPos, "Target Focus Distance: " + _targetFocusDistance, Editor.EditorConstants.GizmosSmallLabelStyle);

			Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.3f);
			Gizmos.DrawSphere(npc.transform.position, _resetDistance);
			newPos = npc.transform.position;
			newPos.y += _resetDistance / 2f;
			newPos.x += _resetDistance / 2f;
			GUI.color = new Color(1f, 0.5f, 0.5f);
			UnityEditor.Handles.Label(newPos, "Reset Distance: " + _resetDistance, Editor.EditorConstants.GizmosSmallLabelStyle);
			GUI.color = Color.white;
		}
#endif
	}
}