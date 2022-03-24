using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class ImageAndTextElement : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _text;

        public Image Image => _image;
        public TextMeshProUGUI Text => _text;
    }
}
