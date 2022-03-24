using RPG.ScriptableObjects;
using RPG.UI.Elements;

using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace RPG.UI.Blocks
{
    public class SelectSkillBlock : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _newPanelText;

        [Space, SerializeField]
        private SelectSkillPanel[] _selectElements;
        //[SerializeField, SQRFloat]
        //private float _minDistance = 1.1f; 


        public SimpleHandle<InputSetType, SkillData> OnSkillSelectedEventHandler;


        public int CurrentLevel { get; set; }

        public void OnSelectSkill(SkillData skill, Vector3 position)
        {
            SelectSkillPanel element = null;
            foreach(var el in _selectElements)
            {
                if (!el.gameObject.activeSelf) continue;

                if(RectTransformUtility.RectangleContainsScreenPoint(el.transform as RectTransform, position))
                {
                    element = el;
                    break;
				}
			}

            if (element == null) return;

            element.Element.SetContent(skill.Icon, true, skill.CurrentLevel);
            element.SetStrings(skill.ID, "Время восстановления: ", skill.Delay + " c");

            OnSkillSelectedEventHandler?.Invoke(element.GetBind, skill);
		}
	}
}
