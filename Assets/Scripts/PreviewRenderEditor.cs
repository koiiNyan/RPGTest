using RPG.UI.Elements;

using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace RPG.Editor
{
    public class PreviewRenderEditor : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private ItemElement _previewItem;

        public void SetItemCellValue(BaseItem item)
        {
            if (_previewItem == null) return;

            _previewItem.Content = item;

            _camera.Render();
        }
    }
}
#endif
