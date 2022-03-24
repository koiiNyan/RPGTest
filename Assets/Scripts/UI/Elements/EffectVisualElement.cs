using RPG.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Elements
{
    public class EffectVisualElement : MonoBehaviour
    {
        private BaseCommandEffect _effect;

        [SerializeField]
        private Image _iconShadow;
        [SerializeField]
        private Image _iconFill;

        public float Filling
        {
            get => _iconFill.fillAmount;
            set => _iconFill.fillAmount = value;
        }

        public Color ShadowColor
        {
            get => _iconShadow.color;
            set => _iconShadow.color = value;
        }

        public BaseCommandEffect Effect 
        {
            get => _effect;
            set
            {
                _effect = value;
                _iconShadow.sprite = _iconFill.sprite = _effect.Sprite;
            }
        }
    }
}