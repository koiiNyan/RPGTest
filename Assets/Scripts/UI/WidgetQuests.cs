using Newtonsoft.Json.Linq;
using RPG.UI.Elements;
using RPG.Units.Player;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace RPG.UI
{
    [SaveData("quests")]
    public partial class WidgetQuests : MonoBehaviour, IClosableWidget, IStateRecorder
    {
        private ToggleGroup _toggleGroup;

        private GameObjectPool<QuestSelectElement> _pool;
        private QuestBaseContext _selectQuest;
        private LinkedList<QuestBaseContext> _activeQuests = new LinkedList<QuestBaseContext>();
        private LinkedList<QuestBaseContext> _completedQuests = new LinkedList<QuestBaseContext>();

        private IEnumerable<QuestBaseContext> _quests;

        [SerializeField]
        private QuestSelectElement _prefab;

        [Space, SerializeField]
        private Transform _content;
        [SerializeField]
        private QuestVisualElement _currentQuest;
        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [Space, SerializeField]
        private InverseToggle _activeListToggle;
        [SerializeField]
        private InverseToggle _completedListToggle;

        [Space, SerializeField]
        private Sprite _activeQuestIcon;
        [SerializeField]
        private Sprite _inactiveQuestIcon;
        [SerializeField]
        private Sprite _completeQuestIcon;
        
        [Space, SerializeField]
        private Color _inactiveParapraphColor = new Color(0.1529412f, 0.04705882f, 0.6509804f, 0.3960784f);
        [SerializeField]
        private Color _failedParagraphColor = new Color(1f, 0f, 0f, 1f);

        //todo ПО этой штуке проиходит выдача лута и оповещение игрока о выполнении задания
        public event SimpleHandle<string, List<string>> OnQuestCompletedEventHandler;
        public event SimpleHandle<string, string> OnAddQuestEventHandler;
        public event SimpleHandle OnCloseWidgetEventHandler;

        public bool HasActiveQuest(LinkedList<IdentificatorParagraphData> paragraphDatas)
        {
            foreach(var data in paragraphDatas)
            {
                var quest = _activeQuests.FirstOrDefault(t => t.ContextID == data.QuestID); 
                if (quest != null) return true;
			}

            return false;
		}

        public void TryToUpdateCondition(string condition)
        {
            foreach(var quest in _activeQuests)
            {
                var par = quest.ParagraphIDs.First(t => t.Type == ParagraphStateType.Active);

                if(par.ObjectCondition == condition)
                {
                    par.Condition--;
                    var list = new LinkedList<IdentificatorParagraphData>();
                    list.AddLast(new IdentificatorParagraphData(quest.ContextID, par.ParagraphID));
                    TryToUpdateParagraph(list);
                    UpdateVisualQuest();
                    return;
                }
			}
		}

        public bool TryToUpdateParagraph(LinkedList<IdentificatorParagraphData> paragraphDatas)
        {
            QuestBaseContext quest = null;

            foreach (var paragraph in paragraphDatas)
            {
                quest = _activeQuests.FirstOrDefault(t => t.ContextID == paragraph.QuestID);
                if (quest == null) continue;

                //Ищем индекс активного параграфа, который можем обновить
                var index = 0;
                while (quest.ParagraphIDs[index].ParagraphID != paragraph.ParagraphID && index < quest.ParagraphIDs.Count) index++;

                //Если найденный параграф не активен - мы не можем его обновить
                if (index >= quest.ParagraphIDs.Count || quest.ParagraphIDs[index].Type != ParagraphStateType.Active) continue;

                //Проверка на завершенность параграфа квеста
                if (TryToCompleteParagraph(quest, index))
                {
                    paragraphDatas.Remove(paragraph);
                }
                return true;
            }
            return false;
        }

        /// <returns>Выполнился-ли параграф квеста</returns>
        private bool TryToCompleteParagraph(QuestBaseContext quest, int index)
        {
            //Если пункт выполнен - помечаем
            if (quest.ParagraphIDs[index].Condition <= 0) quest.ParagraphIDs[index].Type = ParagraphStateType.Completed;

            //Если обновленный пункт выполнился - активируем следующий пункт
            if (quest.ParagraphIDs[index].Type == ParagraphStateType.Completed)
            {
                //Если выполнился последний пункт квеста
                if (index + 1 == quest.ParagraphIDs.Count)
                {
                    OnQuestCompletedEventHandler?.Invoke(quest.ContextID, quest.RewardIDs);
                    _activeQuests.Remove(quest);
                    _completedQuests.AddLast(quest);

                    if (_currentQuest.gameObject.activeSelf) _currentQuest.CompleteQuest();
                }
                //Активируем следующий пункт квеста
                else
                {
                    index++;
                    OnAddQuestEventHandler?.Invoke(quest.ContextID, quest.ParagraphIDs[index].ParagraphID);
                    quest.ParagraphIDs[index].Type = ParagraphStateType.Active;
                    UpdateVisualQuest();
                }

                return true;
            }

            return false;
        }

        public void AddQuest(string contextID)
        {
            var context = _quests.First(t => t.ContextID == contextID);
            _activeQuests.AddLast(context);
            context.ParagraphIDs[0].Type = ParagraphStateType.Active;
            OnAddQuestEventHandler?.Invoke(context.ContextID, context.ParagraphIDs[0].ParagraphID);
            _selectQuest = context;
            UpdateVisualQuest();
		}

        private void UpdateVisualQuest()
        {
            if (_selectQuest == null)
            {
                _currentQuest.ClearVisual();
                return;
            }
            var selectParagraph = _selectQuest.ParagraphIDs.FirstOrDefault(t => t.Type == ParagraphStateType.Active);

            if (selectParagraph == null) _currentQuest.ClearVisual();
            else
            {
                _currentQuest.gameObject.SetActive(true);
                _currentQuest.SetContent(selectParagraph.ParagraphID, selectParagraph.Condition);
            }
        }

        private void OnSelectQuest_UnityEvent(bool isSelect)
        {
            //ОТключение игнорируем
            if (!isSelect) return;

            var element = _pool.FirstOrDefault(t => t.Toggle.isOn);
            if (element == null) return;

            //Находим квеста, соответствующий выбранному переключателю с квестом
            if (_activeListToggle.Toggle.isOn)
            {
                _selectQuest = _activeQuests.FirstOrDefault(t => t.ContextID == element.GetID);
            }

            if (_selectQuest == null) _descriptionText.text = "";
            else PrintDescription(_selectQuest);
        }

        private void PrintDescription(QuestBaseContext context)
        {
            var text1 = $"\n{context.DescriptionID}\n\n\n";
            var text2 = new StringBuilder();
            foreach (var par in context.ParagraphIDs)
            {
                switch (par.Type)
                {
                    case ParagraphStateType.Completed:
                        text2.Append($"\t<s>{par.ParagraphID}</s>\n");
                        break;
                    case ParagraphStateType.Active:
                        text2.Append($"\t{par.ParagraphID}"); text2.Append("\n");
                        break;
                    case ParagraphStateType.Inactive:
                        text2.Append($"\t<color=#{ColorUtility.ToHtmlStringRGBA(_inactiveParapraphColor)}>{par.ParagraphID}</color>\n");
                        break;
                    case ParagraphStateType.Failed:
                        text2.Append($"\t<s><color=#{ColorUtility.ToHtmlStringRGBA(_failedParagraphColor)}>{par.ParagraphID}</color></s>\n");
                        break;
                }
            }

            _descriptionText.text = text1 + text2;
        }

        private void EnableInfo()
        {
            _pool.DisableAllElements();

            if (_activeListToggle.Toggle.isOn ? _activeQuests.Count == 0 : _completedQuests.Count == 0)
            {
                _descriptionText.text = "";
                return;
            }

            var icon = _activeListToggle.Toggle.isOn ? _activeQuestIcon : _completeQuestIcon;

            foreach (var data in _activeListToggle.Toggle.isOn ? _activeQuests : _completedQuests)
            {
                var element = _pool.GetOrCreateElement(out var isNew);

                if (isNew)
                {
                    element.transform.SetParent(_content, false);
                    element.Toggle.onValueChanged.AddListener(OnSelectQuest_UnityEvent);
                }

                element.SetContent(data.ContextID, icon);
            }
        }



		private void OnEnable()
		{
            //Включаем список активных квестов
            if (!_activeListToggle.Toggle.isOn)
            {
                //Тоггл групп не работает при выключенном интерфейсе
                //как только интерфейс включается, восстанавливается старое состояние (которое хранится в тоггл групп)
                //поэтому нужно руками переключить все тогглы (выключить не нужные и включить нужный), чтобы все корректно отработало
                _completedListToggle.Toggle.isOn = false;
                _activeListToggle.Toggle.isOn = true;
            }
            EnableInfo();

            foreach (var element in _pool)
            {
                if (_toggleGroup == null)
                {
                    _toggleGroup = element.gameObject.AddComponent<ToggleGroup>();
                    element.Toggle.isOn = true;//todo нужно еще пометку икнонки поставить
                    OnSelectQuest_UnityEvent(true);
                }
                if (element.Toggle.group == null) element.Toggle.group = _toggleGroup;
            }
        }

		private void OnDisable()
		{
            UpdateVisualQuest();
        }

		private void Awake()
		{
            _pool = new GameObjectPool<QuestSelectElement>(_prefab);
		}

		private void Start()
        {
            var grid = _content.GetComponent<GridLayoutGroup>();
            grid.cellSize = (_prefab.transform as RectTransform).sizeDelta;
        }

        private void OnDestroy()
		{
            foreach (var element in _pool) element.Toggle.onValueChanged.RemoveListener(OnSelectQuest_UnityEvent);
            _pool.OnDestroy();
		}

        [Inject]
        private void ConstructQuests(ScriptableObjects.Contexts.QuestContext[] quests) => _quests = quests.Select(t => t.Context);

        public void ChangeList_UnityEvent(bool value)
        {
            if (!value) return;
            Debug.Log(_activeListToggle.Toggle.isOn);
            EnableInfo();
            OnSelectQuest_UnityEvent(true);
        }

        public void OnClose_UnityEvent() => OnCloseWidgetEventHandler?.Invoke();
    }
}
