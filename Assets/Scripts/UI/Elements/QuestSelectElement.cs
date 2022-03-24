using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class QuestSelectElement : MonoBehaviour
    {
        private string _id;

        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Toggle _toggle;

        public Toggle Toggle => _toggle;
        public string GetID => _id;

        public void SetContent(string id, Sprite icon = null)
        {
            _id = id;
            _text.text = _id;
            if (icon != null) _icon.sprite = icon;
		}
    }
}
