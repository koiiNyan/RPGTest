using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.ScriptableObjects;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.UI.Elements
{
    [RequireComponent(typeof(Image), typeof(Animation))]
    public class SkillIconVisualElement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Coroutine _coroutine;
        private RectTransform _dragIcon;
        private int _level;

        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Image _shadow;
        [SerializeField]
        private Animation _animation;

        [Space, SerializeField, Range(0.1f, 10f)]
        private float _fillingSpeed = 2f;

        public SimpleHandle<SkillIconVisualElement, int> OnUpgradeEventHandler;
        public SimpleHandle<string> OnSelectEventHandler;
        public SimpleHandle<string, Vector3>  OnEndDragEventHandler;

        public bool CanUp { get; set; }
        public bool PassiveSkill { get; set; }
        public string ID { get; set; } 

        public void SetContent(Sprite icon, bool isUp, int level)
        {
            _icon.sprite = icon;
            _shadow.fillAmount = isUp ? 0f : 1f;
            _level = level; 
            if (_text != null) _text.text = _level.ToString();
		}

        private IEnumerator Filling()
        {
            var time = 1f;
            while(time > 0f)
            {
                time -= TimeAssistant.UIDeltaTime / _fillingSpeed;

                _shadow.fillAmount = time;
                yield return null;
			}

            _coroutine = null;

            _level++;
            OnUpgradeEventHandler?.Invoke(this, _level);

            _shadow.fillAmount = 0f;
            _animation.Play();
        }

        private void UpNumber_UnityEvent()
        {
            if (_text == null && !int.TryParse(_text.text, out var value)) return;
            
            _text.text = (int.Parse(_text.text) + 1).ToString();
		}

        public void OnPointerDown(PointerEventData eventData)
        {
            OnSelectEventHandler?.Invoke(ID);
            if (_animation.isPlaying || !CanUp) return;
            _coroutine = StartCoroutine(Filling());
        }

        public void OnPointerUp(PointerEventData eventData)
		{
            if (_coroutine == null || _animation.isPlaying) return;
            _shadow.fillAmount = _level == 0 ? 1f : 0f;
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (PassiveSkill || _level < 1) return;
            var image  = Instantiate(this).GetComponent<Image>();
            image.GetComponent<SkillIconVisualElement>().enabled = false;

            _dragIcon = image.transform as RectTransform;
            _dragIcon.SetParent(transform.parent, false);
            _dragIcon.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || eventData.pointerEnter == null) return;

            RectTransform rect = eventData.pointerEnter.transform as RectTransform;
            if (rect == null) return;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out var pos);
            _dragIcon.position = pos;
        }

		public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragIcon == null) return;

            RectTransform rect = eventData.pointerEnter.transform as RectTransform;
            if (rect != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out var pos);
                OnEndDragEventHandler?.Invoke(ID, pos);
            }

            Destroy(_dragIcon.gameObject);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SkillIconVisualElement))]
    public class SkillIconVisualElementEditor : UnityEditor.Editor
    {
        private bool _foldout;
        private Image _iconProp;
        private TextMeshProUGUI _textProp;

		private void OnEnable()
		{
            _iconProp = serializedObject.FindProperty("_icon").objectReferenceValue as Image;
            var field = serializedObject.FindProperty("_text").objectReferenceValue;
            if(field != null) _textProp = field as TextMeshProUGUI;
        }

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

            EditorGUILayout.Space(10f);
            _foldout = EditorGUILayout.Foldout(_foldout, "Internal Params", new GUIStyle("foldout") {fontSize = 13, fontStyle = FontStyle.Bold});

            if (_foldout)
            {
                _iconProp.sprite = EditorGUILayout.ObjectField("Sprite", _iconProp.sprite, typeof(Sprite), false) as Sprite;
                if(_textProp != null)
                    _textProp.text = EditorGUILayout.TextField("Text", _textProp.text);
            }
        }
	}

#endif
}