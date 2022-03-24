using RPG.UI.Blocks;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class WidgetBattle : MonoBehaviour
    {
        [SerializeField]
        private CursorBlock _focus;
        [SerializeField]
        private ConditionBlock _conditions;
        [SerializeField]
        private EffectBlock _effects;

        public void ClearVisual()
        {
            _effects.ClearVisual();
		}
    }
}
