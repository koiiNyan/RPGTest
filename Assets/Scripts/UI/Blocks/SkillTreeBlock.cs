using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.UI.Elements;
using RPG.ScriptableObjects;
using System.Text;
using RPG.ScriptableObjects.Contexts;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace RPG.UI.Blocks
{
    [SaveData("skills")]
    public partial class SkillTreeBlock : BaseTabBlock, IStateRecorder
    {
        private GameObjectPool<SkillIconVisualElement> _passivePool;
        private GameObjectPool<SkillIconVisualElement> _activePool;

        private SkillTreeContext[] _contexts;
        private Vector2[,] _positions;

        private List<SkillData>[] _skillLists;

        [SerializeField]
        private SkillIconVisualElement _passivePrefab;
        [SerializeField]
        private SkillIconVisualElement _activePrefab;

        [Space, Header("---Ссылки на компоненты---"), SerializeField]
        private RectTransform _content;
        [SerializeField]
        private TextMeshProUGUI _freePointsText;
        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [SerializeField]
        private ImageAndTextElement[] _collapseBlocks;

        [Header("---Настройки таблицы талантов---")]
        [SerializeField]
        private Vector2 _border = new Vector2(10f, 10f);
        [SerializeField]
        private Vector2Int _countTable = new Vector2Int(5, 4);

        [Space, SerializeField]
        private Vector2 _cellSpace = new Vector2(20f, 20f);
        [SerializeField]
        private Vector2 _skillSize = new Vector2(150f, 150f);

        [Space, SerializeField]
        private Vector2[] _skillElementPositions;

        public SimpleHandle<InputSetType, SkillData> OnSkillUpEventHandler;
        public SimpleHandle<SkillData> OnSkillDownEventHandler;
        public SimpleHandle<SkillData, Vector3> OnEndDragSkillEventHandler;

        public Units.Player.ILevelProgress Progress { get; set; }//todo Интерфейс лежит у игрока, это плохо

		private void Awake()
		{
            _activePool = new GameObjectPool<SkillIconVisualElement>(_activePrefab);
            _passivePool = new GameObjectPool<SkillIconVisualElement>(_passivePrefab);

            _positions = new Vector2[_countTable.x, _countTable.y];

            for(int i = 0, k = 0; i < _countTable.x; i++)
            {
                for(int j = 0; j < _countTable.y; j++, k++)
                {
                    _positions[i, j] = _skillElementPositions[k];
				}
			}
        }

		private void Start()
		{
            CreateTabs(_contexts.Select(t => t.Description).ToArray());
        }

		protected async override void OnEnable()
        {
            base.OnEnable();
            _descriptionText.text = "";

            await System.Threading.Tasks.Task.Yield();
			_freePointsText.text = Progress.FreePoints.ToString();
        }

        [Zenject.Inject]
        public void SetTabs(SkillTreeContext[] contexts)
        {
            _skillLists = new List<SkillData>[contexts.Length];
            for (int i = 0; i < contexts.Length; i++)
            {
                //Клонируем информацию по скиллам
                _skillLists[i] = new List<SkillData>(contexts[i].Skills.Count);
                foreach (var skill in contexts[i].Skills)
                {
                    if (skill != null) _skillLists[i].Add(skill.CreateData());
                }
            }
            _contexts = contexts;
        }

        protected override void ShowTable(int selectIndex)
        {
            ClearElements();
            var skills = _skillLists[selectIndex];
            for (int i = 0; i < skills.Count; i++)
			{
				var element = skills[i].PassiveSkill ? _passivePool.GetOrCreateElement(out bool isNew) : _activePool.GetOrCreateElement(out isNew);
                var rect = element.transform as RectTransform;

				if (isNew)
				{
                    rect.SetParent(_content, false);
                    rect.sizeDelta = _skillSize;
                    rect.pivot = new Vector2(0f, 1f);
                    rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                    element.PassiveSkill = skills[i].PassiveSkill;
                }
                rect.anchoredPosition = _positions[skills[i].Position.x, skills[i].Position.y];
                element.SetContent(skills[i].Icon, skills[i].CurrentLevel != 0, skills[i].CurrentLevel);

                //Можно-ли прокачать скилл
                element.CanUp = Progress.FreePoints > 0 && skills[i].CurrentLevel < skills[i].MaxLevel;
                element.ID = skills[i].ID;
                element.OnUpgradeEventHandler += OnSkillUp;
                element.OnSelectEventHandler += OnShowDescription;
                element.OnEndDragEventHandler += OnEndDrag;
            }
		}

        private void OnSkillUp(SkillIconVisualElement element, int level)
        {
            Progress.FreePoints--;
            _freePointsText.text = Progress.FreePoints.ToString();

            //Лочим прокачку, если не хватает очков
            if (Progress.FreePoints <= 0)
            {
                foreach (var el in _activePool) el.CanUp = false;
                foreach (var el in _passivePool) el.CanUp = false;
			}

            var skill = FindSkillData(element.ID);
            skill.CurrentLevel = level;

            //Скилл максимально вкачен
            if (skill.CurrentLevel >= skill.MaxLevel) element.CanUp = false;

            OnShowDescription(element.ID);

            OnSkillUpEventHandler?.Invoke(InputSetType.None, skill);
        }

        private void OnShowDescription(string id)
        {
            var skill = FindSkillData(id);

            var str = new StringBuilder();

            //Заголовок описания
            str.Append($"\t\t{skill.ID} ");
            //Уровень раскачки скилла
            if(skill.CurrentLevel != skill.MaxLevel) str.Append($"({skill.CurrentLevel}/{skill.MaxLevel})");
            //Худ. описание
            str.Append($"\n\t{skill.DescriptionID}\n");
            //Время восстановления
            if (!skill.PassiveSkill) str.Append($"\tВремя восстановления: {skill.Delay} с\n");
            //Бонусы от прокачки
            if(skill.Statuses.Count != 0) str.Append($"<color=#FBA0A0><i>Бонусы от таланта:</i></color>");
            
            //Перечисление бонусов
            foreach(var status in skill.Statuses)
            {
                str.Append($"\n{StringHelper.GetStatsLocalization(status.Type)}: ");
                str.Append($"<b><color=#B9DB44><b><u>{skill.CurrentLevel * status.Data.Value}%");
                if(skill.CurrentLevel != skill.MaxLevel) str.Append($" + {status.Data.Value}%");
                str.Append("</size></color></u></b>");
            }

            _descriptionText.text = str.ToString();
		}

        private void OnEndDrag(string id, Vector3 position)
        {
            OnEndDragSkillEventHandler?.Invoke(FindSkillData(id), position);
		}

        private SkillData FindSkillData(string id)
        {
            //Находим активную вкладку
            int i = 0;
            for(; i < _tabs.Length; i++)
            {
                if (_tabs[i].Toggle.isOn) break;
			}
            //Находим выбранный скилл
            int j = 0;
            for(; j < _skillLists[i].Count; j++)
            {
                if (_skillLists[i][j].ID == id) break;
			}

            return _skillLists[i][j];
		}

        private void ClearElements()
        {
            foreach (var element in _passivePool)
            {
                element.OnUpgradeEventHandler -= OnSkillUp;
                element.OnSelectEventHandler -= OnShowDescription;
                element.OnEndDragEventHandler -= OnEndDrag;
            }
            foreach (var element in _activePool)
            {
                element.OnUpgradeEventHandler -= OnSkillUp;
                element.OnSelectEventHandler -= OnShowDescription;
                element.OnEndDragEventHandler -= OnEndDrag;
            }
            _passivePool.DisableAllElements();
            _activePool.DisableAllElements();
        }

#if UNITY_EDITOR

        [ContextMenu("ReCalc Sizes")]
        private void ReCalcSizes()
        {
            var rect = _content.rect.size;//1440 670

            //Получаем используемое пространство с вычетом отступов
            rect.x -= _border.x * 2f;
            rect.y -= _border.y * 2f;

            //Ширина клетки колонки
            var columnWidth = rect.x / _countTable.y;
            //Высота клетки строки
            var rowsHeight = rect.y / _countTable.x;

            //Свободное место в клетке без учета ширины скилла
            _cellSpace = new Vector2((columnWidth - _skillSize.x) / 2f, (rowsHeight - _skillSize.y) / 2f);

            _skillElementPositions = new Vector2[_countTable.x * _countTable.y];
            //(600 - (10 * 2)) = 580 //внутрненее расстояние для клеток
            //580 / 2 = 290//высота одной строки
            //290 - 150 = 140//свободная ширина в клетке
            //140 / 2 = 70 //оступ по высоте
            

            //10 + 70 = 80/ оступ для первой строки с учетом спейса в клетке
            //80 + 150 = 230/отступ до конца первой строки
            //230 + 70 = 300/отступ до начала второй строки
            //300 + 70 = 370 /отступ для вторйо строки с учетом спейса в клетке
            float y = -_border.y;
            //Идем по строкам
            for (int i = 0; i < _countTable.x; i++)
            {
                y -= _cellSpace.y;
                float x = _border.x;
                //Идем по столбцам в одной строке
                for (int j = 0; j < _countTable.y; j++)
                {
                    x += _cellSpace.x;
                    _skillElementPositions[i * _countTable.y + j] = new Vector2(x, y);
                    x += _skillSize.x + _cellSpace.x;
                }
                y -= _skillSize.y + _cellSpace.y;
            }
        }

        [ContextMenu("Create Blocks")]
        private void CreateBlocks()
        {
            CollapseBlocks();
            foreach (var position in _skillElementPositions)
            {
                var obj = new GameObject();
                obj.AddComponent<Image>();

                var rect = obj.GetComponent<RectTransform>();
                rect.SetParent(_content, false);
                rect.pivot = new Vector2(0f, 1f);
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.sizeDelta = _skillSize;
                rect.anchoredPosition = position;
                obj.name = string.Concat(Editor.EditorConstants.EditorGameObjectName, "_cell");
            }
		}

        [ContextMenu("Collapse Blocks")]
        private void CollapseBlocks()
        {
            var selectName = string.Concat(Editor.EditorConstants.EditorGameObjectName, "_cell");
            var objs = FindObjectsOfType<Transform>().Where(t=> t.name == selectName).ToArray();

            for(int i = 0; i < objs.Length; i++)
            {
                DestroyImmediate(objs[i].gameObject);
			}
		}
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SkillTreeBlock))]
    public class SkillTreePanelEditor : UnityEditor.Editor
    {
        private bool _foldout;

        private SkillTreeBlock _target;
        private MethodInfo _reCalcSizes;
        private MethodInfo _createBlocks;
        private MethodInfo _collapseBlocks;
        private MethodInfo _updateTogglesSize;

        private void OnEnable()
        {
            _target = target as SkillTreeBlock;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(20f);
            _foldout = EditorGUILayout.Foldout(_foldout, "-----*Context Menu*-----", new GUIStyle("foldout") { fontSize = 13, fontStyle = FontStyle.Bold });

            if (_foldout)
            {
                ReInit();
                PrintButtons("Пересчет размера сетки таблицы", _reCalcSizes, Color.green);
                PrintButtons("Визуализация сетки", _createBlocks, Color.yellow);
                PrintButtons("Очистить сетку", _collapseBlocks, Color.red);
                PrintButtons("Пересчет ширины переключателей", _updateTogglesSize, Color.magenta);
            }
        }

        private void PrintButtons(string label, MethodInfo method, Color color)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(label);
            GUI.color = color;
            if (GUILayout.Button(method.Name)) method.Invoke(target, null);
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }

        private void ReInit()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            if (_reCalcSizes == null)
            {
                _reCalcSizes = typeof(SkillTreeBlock).GetMethod("ReCalcSizes", flags);
			}
            if (_createBlocks == null)
            {
                _createBlocks = typeof(SkillTreeBlock).GetMethod("CreateBlocks", flags);
            }
            if (_collapseBlocks == null)
            {
                _collapseBlocks = typeof(SkillTreeBlock).GetMethod("CollapseBlocks", flags);
            }
            if (_updateTogglesSize == null)
            {
                _updateTogglesSize = typeof(SkillTreeBlock).GetMethod("UpdateTogglesSize", flags);
            }
        }
    }
#endif
}
