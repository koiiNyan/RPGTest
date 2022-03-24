using RPG.Units.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;
using RPG.ScriptableObjects;
using RPG.UI;
using RPG.ScriptableObjects.Contexts;
using RPG.UI.Blocks;
using RPG.Units;
using RPG.ScriptableObjects.Configurations;
using UnityEngine.InputSystem;
using TMPro;

#if UNITY_EDITOR
using System.IO;
#endif

namespace RPG.Managers
{
    public class GameManager : MonoInstaller, IManager
	{
		[SerializeField]
		private GameObject _interfaceRoot;

		[SerializeField]
		private GameSettings _settings;
		[SerializeField]
		private InterfaceInteractSettings _interactSettings;

		[SerializeField]
		private ScriptableObjectConfiguration[] _generalConfigurations;
		[SerializeField]
		private QuestContext[] _questContexts;
		[SerializeField]
		private SkillTreeContext[] _skillConfigurations;
		[SerializeField]
		private BaseEffectConfiguration[] _effectConfigurations;
		[SerializeField]
		private BaseItemContext[] _itemContexts;

		public override void InstallBindings()
		{
			Assistants.ConstructEntityExtensions.Construct(FindObjectOfType<UnitManager>(), _interfaceRoot, _effectConfigurations, _itemContexts);
			//Прокидываем привязки в контроллер
			var controls = new PlayerControls();
			_settings.LoadBindingsAndSettings(controls);
			Container.BindInstance(_settings).AsSingle();
			Container.BindInstance(_interactSettings).AsSingle();
			var player = FindObjectOfType<PlayerUnit>();
			Container.BindInstance(player).AsSingle();
			Container.BindInstance(FindObjectOfType<PlayerStateComponent>()).AsSingle();
			Container.BindInstance(player.Inventory).AsSingle();
			Container.BindInstance(controls).AsSingle();
			Container.BindInstance(FindObjectOfType<CameraComponent>()).AsSingle();
			Container.BindInstance(Camera.main).AsSingle();
			Container.BindInstance(_interfaceRoot).AsSingle();
			Container.BindInstances(_generalConfigurations);
			Container.BindInstance(_questContexts);
			Container.BindInstance(_skillConfigurations);
			//Container.BindInstance<BaseItemContext[]>(_itemContexts);
			ConfigurationLoaderWithSignalBus();
			Constants.ConfigurationConstance(player.GetComponent<Animator>());

			StartCoroutine(DelayLoadGame());
		}

		private void ConfigurationLoaderWithSignalBus()
		{
			SignalBusInstaller.Install(Container);
			Container.DeclareSignal<LinkedList<EquipmentItem>>().WithId(Constants.LoadIdentifier);
			Container.DeclareSignal<(IdentificatorParagraphData, ParagraphStateType)>().WithId(Constants.LoadIdentifier);
		}

		private IEnumerator DelayLoadGame()
		{
			yield return new WaitForSeconds(0.5F);
			//todo Сдесь лучше сделать статическое поле, в которое будет проставляться адрес сейва, который нужно прогружать,
			//чтобы не тянуть сначала дефолтный сейв, а потом еще и подгружаеn подгружает выбранный сейв
			_interfaceRoot.GetComponentInChildren<WidgetSaveAndLoad>(true).LoadDefaultSave();
		}

#if UNITY_EDITOR
		public override void Start()
		{
			base.Start();
			var level = FindObjectsOfType<Transform>().First(t => t.name == "Floor");

			if (level == null) Editor.EditorExtensions.ConsoleLog($"The scene does not contain the \"Floor\" game object", Editor.PriorityMessageType.Critical);
			level.gameObject.AddComponent<Editor.LevelCheckComponent>();
		}

		[ContextMenu("Find Level And Fix Tag & Layer")]
		private void FindLevelAndFixTagAndLayer()
		{
			var level = FindObjectsOfType<Transform>().First(t => t.name == "Level");

			if (level == null) Editor.EditorExtensions.ConsoleLog($"The scene does not contain the \"Level\" game object", Editor.PriorityMessageType.Critical);
			foreach(var obstacle in level.GetComponentsInChildren<Transform>())
			{
				obstacle.gameObject.tag = Constants.FloorTag;
				obstacle.gameObject.layer = LayerMask.NameToLayer(Constants.ObstacleLayerName);
			}

			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
#endif

		[ContextMenu("Configuration")]
		public void Configuration()
        {
			if (!Application.isEditor) return;
#if UNITY_EDITOR

			#region Поиск и добавление в массив всех конфигураторов

			var configs = RPGExtensions.FindAllAssetsByType<ScriptableObjectConfiguration>(SearchOption.AllDirectories, "/Configurations");

			var items = new LinkedList<BaseItemContext>();
			var skills = new LinkedList<SkillTreeContext>();
			var quests = new LinkedList<QuestContext>();
			var effects = new LinkedList<BaseEffectConfiguration>();
			var general = new LinkedList<GeneralConfiguration>();

			foreach(var el in configs)
			{
				if (el is BaseItemContext c) items.AddLast(c);
				else if (el is SkillTreeContext s) skills.AddLast(s);
				else if (el is QuestContext q) quests.AddLast(q);
				else if (el is BaseEffectConfiguration e) effects.AddLast(e);
				else if (el is GeneralConfiguration g) general.AddLast(g);
			}

			_itemContexts = items.ToArray();
			_skillConfigurations = skills.ToArray();
			_questContexts = quests.ToArray();
			_effectConfigurations = effects.ToArray();
			_generalConfigurations = general.ToArray();

			#endregion

			#region Вызов методов конфигурации у всех менеджеров

			var managers = FindObjectsOfType<MonoBehaviour>().Where(t => t is IManager && t != this);

			foreach (IManager manager in managers) manager.Configuration();

			var str = new System.Text.StringBuilder();
			foreach (var manager in managers)
			{
				str.Append("<b>");
				str.Append(manager.GetType().Name);
				str.Append("</b>, ");
			}
			Debug.Log("Successful configuration call for the following managers: " + str.ToString());

			#endregion

			#region Удаление со сцены всех игровых объектов редактора

			var GOs = FindObjectsOfType<Transform>().Where(t => t.name.Contains(Editor.EditorConstants.EditorGameObjectName)).ToArray();

			for(int i = 0; i < GOs.Length; i++)
			{
				DestroyImmediate(GOs[i].gameObject);
			}

			Debug.Log("Destroyed all GOs of the editor with the prefix:" + Editor.EditorConstants.EditorGameObjectName);

			var parsers = FindObjectsOfType<Editor.UnitParses>();

			for (int i = 0; i < parsers.Length; i++) DestroyImmediate(parsers[i]);

			#endregion

			#region Добавление недостающих компонентов через атрибут RequireComponent

			var monos = FindObjectsOfType<MonoBehaviour>();

			var action = new System.Action<GameObject, System.Type>((target, type) =>
			{
				if (type == null) return;

				var component = target.GetComponent(type);
				if (component != null) return;

				target.AddComponent(type);
				Debug.Log($"The GameManager has added a <b>{type.Name}</b> component to the <b>{target.name}</b> Unit that is required by the <i>{nameof(RequireComponent)}</i> attribute");
			});

			foreach(var mono in monos)
			{
				var atrs = mono.GetType().GetCustomAttributes(typeof(RequireComponent), false);

				foreach(RequireComponent atr in atrs)
				{
					action(mono.gameObject, atr.m_Type0);
					action(mono.gameObject, atr.m_Type1);
					action(mono.gameObject, atr.m_Type2);
				}
			}

			#endregion

			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
		}
	}

	internal interface IManager
	{
		void Configuration();
	}
}
