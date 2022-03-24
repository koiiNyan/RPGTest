using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.UI.Slider;

namespace RPG.UI.Elements
{
    public class SliderVisualElement : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private TextMeshProUGUI _value;
        [SerializeField]
        private string _additionalValue = "%";
        [SerializeField]
        private float _multiply = 100f;
        [SerializeField]
        private Color _minColor;
        [SerializeField]
        private Color _maxColor;
        [SerializeField]
        private float _maxValue = 1f;

        public float Value
        {
            get => _slider.value;
            set
            {
                _slider.value = value;
                OnSliderUpdate_UnityEvent(value);
            }
        }

        public SliderEvent OnValueChanged => _slider.onValueChanged;


        public void OnSliderUpdate_UnityEvent(float value)
        {
            _value.text = string.Concat(Mathf.RoundToInt(value * _multiply), _additionalValue);
            _value.color = Color.Lerp(_minColor, _maxColor, value / _maxValue);
		}
    }
}
