using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class QuestVisualElement : MonoBehaviour
    {
        private readonly string[] _richText = new string[] { "<size=", /*60*/ ">", /*CONTENT*/ "</size>" };

        private string _textContent;
        private int? _counter;

        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Image _filling;

        [Space, SerializeField]
        private Color _completeColor;
        [SerializeField]
        private Color _failedColor;
        [Space, SerializeField]
        private Sprite _currentIcon;
        [SerializeField]
        private Sprite _completeIcon;
        [SerializeField]
        private Sprite _failedIcon;

        [Space, SerializeField]
        private string _separatorText = ": ";
        [SerializeField, Range(0, 10), Tooltip("На сколько размер шрифта больше у числителя квеста")]
        private int _sizeCounterMult = 5;
        [SerializeField, Range(0f, 10f)]
        private float _timeFillingText = 2f;

        public void SetContent(string text, int counter)
        {
            _textContent = text; _counter = counter;
            _filling.fillAmount = 0f;
            _text.text = counter > 0 ? _text.text = CreateRichString() : _textContent;

            _icon.enabled = true;
            _icon.sprite = _currentIcon;
        }

        public void ClearVisual()
        {
            _icon.enabled = false;
            _text.text = "";
		}

        public void UpdateState(int count)
        {
            _counter -= count;
            _text.text = _counter.Value <= 0 ? _textContent : CreateRichString();
		}

        public void CompleteQuest()
        {
            _filling.color = _completeColor;

            if (_counter != null)
            {
                _counter = null;
                _text.text = _textContent;
            }
            StartCoroutine(Filling(true));
		}

        public void FailQuest()
        {
            _filling.color = _failedColor;
            StartCoroutine(Filling(false));
        }

        private IEnumerator Filling(bool isComplete)
        {
            var fill = 0f;
            var rect = _filling.transform as RectTransform;
            rect.sizeDelta = new Vector2(_text.GetRenderedValues(true).x + 15f, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(rect.sizeDelta.x / 2f, 0f);
            while (fill < 1f)
            {
                yield return new WaitForEndOfFrame();
                fill += Time.deltaTime / _timeFillingText;
                _filling.fillAmount = fill;
			}

            _icon.sprite = isComplete ? _completeIcon : _failedIcon;

            yield return new WaitForSeconds(1.5f);
            ClearVisual();
        }

        private string CreateRichString()
        {
            return string.Concat(_textContent, _separatorText, _richText[0], _text.fontSize + _sizeCounterMult, _richText[1], _counter, _richText[2]);
        }
    }
}
