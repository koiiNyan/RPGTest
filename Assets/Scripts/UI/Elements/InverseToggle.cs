using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class InverseToggle : MonoBehaviour
    {
        private Toggle _toggle;

        [SerializeField]
        private Image _checkmark;
        [SerializeField]
        private TextMeshProUGUI _selectText;
        
        [Space, SerializeField]
        private Color _unselectColor;
        [SerializeField]
        private Color _selectColor;

        public Toggle Toggle
        {
            get
            {
                if(_toggle == null)
                {
                    _toggle = GetComponent<Toggle>();
				}

                return _toggle;
			}
		}

        public string Content
        {
            get => _selectText.text;
            set => _selectText.text = value;
		}

        public void OnToggleSwitch_UnityEvent(bool select)
        {
            _checkmark.enabled = !select;

            if (_selectText == null) return;

            _selectText.color = select ? _selectColor : _unselectColor;
		}
	}
}
