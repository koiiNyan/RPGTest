using RPG.UI.Elements;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Blocks
{
    public abstract class BaseTabBlock : MonoBehaviour
    {
        protected InverseToggle[] _tabs;
        private ToggleGroup _tabsGroup;

        [SerializeField]
        private InverseToggle _togglePrefab;
        [SerializeField]
        private GridLayoutGroup _togglesGroup;

        public void ForceUpdateBlock()
        {
            SwitchTab(true);
		}

        protected void CreateTabs(string[] tabNames)
        {
            _tabs = new InverseToggle[tabNames.Length];
            for (int i = 0; i < tabNames.Length; i++)
            {
                //Создаем вкладку таблицы под скиллы
                _tabs[i] = Instantiate(_togglePrefab);
                _tabs[i].transform.SetParent(_togglesGroup.transform);
                _tabs[i].transform.localScale = Vector3.one;

                if (_tabsGroup == null)
                {
                    _tabsGroup = _tabs[i].gameObject.AddComponent<ToggleGroup>();
                }

                _tabs[i].Toggle.group = _tabsGroup;
                _tabs[i].Content = tabNames[i];

                _tabs[i].Toggle.onValueChanged.AddListener(SwitchTab);
            }

            //Обновление размеров табов, исходя из их количества
            var length = _togglesGroup.GetComponentsInChildren<Toggle>().Length;
            var width = (_togglesGroup.transform as RectTransform).rect.width - _togglesGroup.padding.left - _togglesGroup.padding.right;
            _togglesGroup.cellSize = new Vector2(width / length, _togglesGroup.cellSize.y);

            Canvas.ForceUpdateCanvases();

            _tabs[0].Toggle.isOn = true;
            SwitchTab(true);
        }

        private void SwitchTab(bool value)
        {
            if (!value) return;

            int i = 0;
            for (; i < _tabs.Length; i++)
            {
                if (_tabs[i].Toggle.isOn) break;
            }

            ShowTable(i);
        }

        protected abstract void ShowTable(int selectIndex);

        protected virtual void OnEnable()
        {
            if (_tabs == null) return;
            foreach (var tab in _tabs) tab.Toggle.isOn = false;
            _tabs[0].Toggle.isOn = true;
        }

		protected virtual void OnDestroy()
		{
            foreach (var tab in _tabs)
            {
                tab.Toggle.onValueChanged.RemoveListener(SwitchTab);
            }
        }
	}
}
