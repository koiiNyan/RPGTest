using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace RPG.UI.Elements
{
    public class SaveSlotVisualElement : MonoBehaviour
    {
        [SerializeField]
        private InverseToggle _toggle;
        [SerializeField]
        private TextMeshProUGUI _dateText;

        public InverseToggle InverseToggle => _toggle;
        public TextMeshProUGUI DateText => _dateText;
    }
}
