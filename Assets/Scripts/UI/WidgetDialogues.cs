using RPG.ScriptableObjects;
using RPG.UI.Elements;
using RPG.Units.NPCs;
using RPG.Units.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RPG.UI
{
    public class WidgetDialogues : MonoBehaviour
    {
        private GameObjectPool<PhraseVisualElement> _pool;
        private string[] _nameRichText = new string[] { "<size="/*70*/, "><i><color=#"/*FFFC02*/, ">"/*Староста*/, ": </color></i></size>" };
        private int _personNumber;
        private string _playerName;

        private NPCUnit _currentTarget;
        private DialogueContext _currentContext;

        [Inject]
        private PlayerControls _controls;

        [SerializeField]
        private PhraseVisualElement _prefab;

        [Space, SerializeField]
        private TextMeshProUGUI _mainText;
        [SerializeField]
        private Scrollbar _scrollbar;
        [SerializeField]
        private RectTransform _content;

        [Space, SerializeField]
        private Color _unitColor = Color.yellow;
        [SerializeField]
        private int _unitNameSize = 50;

        public SimpleHandle<PhraseType> OnUpdateDialogueStateEventHandler;

        public void SetContext(NPCUnit target, DialogueContext context)
        {
            _currentTarget = target;
            _currentContext = context;
            _personNumber = 0;

            SetDialogue();
        }

        private void SetDialogue()
        {
            DisableAllElements();
            if (_currentContext.Type == PhraseType.Answers)
            {
                _scrollbar.value = 1f;

                int i = 0;
                for (; i < _currentContext.Answers.Count; i++)
                {
                    var element = _pool.GetOrCreateElement(out var isNew);

                    if (isNew) element.transform.SetParent(_content, false);

                    element.OnClickEventHandler += ChoiceAnswer;
                    element.SetContent(i, _currentContext.Answers[i].ContextID, _currentContext.Answers[i].Sprite);
                }

                var height = (_prefab.transform as RectTransform).sizeDelta.y * i;
                _content.sizeDelta = new Vector2(_content.sizeDelta.x, height);
            }
            _personNumber++;
            _mainText.text = string.Concat(_nameRichText[0], _unitNameSize, _nameRichText[1], ColorUtility.ToHtmlStringRGB(_unitColor), _nameRichText[2], _personNumber % 2 == 0 ? _currentTarget.State.DisplayName : _playerName, _nameRichText[3], _currentContext.ContextID);
        }

        private void ChoiceAnswer(int index)
        {
            if (_currentContext.Answers[index].Type == PhraseType.Phrase)
            {
                _currentContext = _currentContext.Answers[index].Dialogue;
                _personNumber = 1;
                SetDialogue();
                return;
            }
            OnUpdateDialogueStateEventHandler?.Invoke(_currentContext.Answers[index].Type);

            /*//Если у перехода имеется продолжение - идем по нему
            if(_currentContext.Dialogue != null)
            {
                _currentContext = _currentContext.Dialogue;
                SetDialogue();
            }
            //Переход пустой - выходим
            else 
            {
                OnUpdateDialogueStateEventHandler?.Invoke(PhraseType.None);
            }*/
        }

        private void OnSkip(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (_currentContext.Type == PhraseType.None)
            {
                OnUpdateDialogueStateEventHandler?.Invoke(_currentContext.Type);
                return;
            }

            if (_currentContext.Type != PhraseType.Phrase) return;

            _currentContext = _currentContext.Dialogue;
            SetDialogue();
        }

		private void Awake()
		{
            _pool = new GameObjectPool<PhraseVisualElement>(_prefab);
		}

		private void Start()
        {
            _controls.UI.Skip.performed += OnSkip;

            var grid = _content.GetComponent<GridLayoutGroup>();
            grid.cellSize = (_prefab.transform as RectTransform).sizeDelta;
        }

        [Inject]
        private void Construct(PlayerStateComponent state)
        {
            _playerName = state.DisplayName;
        }

        private void DisableAllElements()
        {
            foreach (var element in _pool)
            {
                element.OnClickEventHandler -= ChoiceAnswer;
            }
            _pool.DisableAllElements();
        }

        private void OnDestroy()
        {
            _pool.OnDestroy();
            _controls.UI.Skip.performed -= OnSkip;
        }
    }
}
