using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static TMPro.TMP_Dropdown;
using static TMPro.TMP_InputField;

namespace RPG.UI.Elements
{
    public class SelectorVisualElement : MonoBehaviour
    {
        private string[] _variants;
        private int _currentIndex;

        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Button _prefButton;
        [SerializeField]
        private Button _nextButton;
        
        public OnChangeEvent OnStringChanged { get; set; } = new OnChangeEvent();
        public DropdownEvent OnIntChanged { get; set; } = new DropdownEvent();

        public string Variant 
        {
            get => _variants != null ? _variants[_currentIndex] : null;
            set
            {
                for(int i = 0; i < _variants.Length; i++)
                {
                    if (_variants[i] != value) continue;

                    _currentIndex = i;
                    _text.text = value;
                    CheckInteractable();
                    return;
				}

                throw new System.ApplicationException("Variants collection does not contain a set value");
			}
        }

        public int VariantIndex
        {
            get => _currentIndex;
            set
            {
                if (value < 0 || value >= _variants.Length) throw new System.ApplicationException("Variant collection does not allow the given index: <b>" + value);
                _currentIndex = value;
                _text.text = _variants[_currentIndex];
                CheckInteractable();
            }
		}

        private void CheckInteractable()
        {
            _prefButton.interactable = _currentIndex != 0;
            _nextButton.interactable = _currentIndex < _variants.Length - 1;
        }

        public void SetContentAndValue(string[] variants, int index = 0)
        {
            _variants = variants;
            _currentIndex = index;

            _text.text = _variants[_currentIndex];
		}

        public void OnPrefVariant_UnityEvent()
        {
            _currentIndex = Mathf.Clamp(_currentIndex - 1, 0, _variants.Length - 1);
            _text.text = _variants[_currentIndex];

            OnStringChanged.Invoke(_text.text);
            OnIntChanged.Invoke(_currentIndex);
            CheckInteractable();
        }

        public void OnNextVariant_UnityEvent()
        {
            _currentIndex = Mathf.Clamp(_currentIndex + 1, 0, _variants.Length - 1);
            _text.text = _variants[_currentIndex];

            OnStringChanged.Invoke(_text.text);
            OnIntChanged.Invoke(_currentIndex);
            CheckInteractable();
        }
    }
}
