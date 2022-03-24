using RPG.Units.Player;
using UnityEditor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using System.Linq;

namespace RPG.UI.Elements
{
    [RequireComponent(typeof(Button))]
    public class BindingVisualElement : MonoBehaviour
    {
        private PlayerControls _controls;
        private InputAction _action; 
        private RebindingOperation _rebind;


        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private int _bindIndex = 0;

        public InputActionAsset _test;

        [Space, SerializeField, ReadOnly]
        private string _actionName;

        public SimpleHandle<bool> OnUpdateBindingsEventHandler;

        public void SetControls(PlayerControls controls)
        {
            _controls = controls;
            _action = _controls.First(t => t.name == _actionName);
            UpdateText();
        }

        public void OnBind_UnityEvent()
        {
            _text.text = StringHelper.BindWaitingText;
            _action.Disable();

            _rebind?.Dispose();
            _rebind = _action.PerformInteractiveRebinding()
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(Rebinding);

            _rebind.Start();
        }

        void UpdateText()
        {
            _text.text = InputControlPath.ToHumanReadableString(_action.bindings[_bindIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        private void Rebinding(RebindingOperation operation)
        {
            _rebind.Dispose();
            _rebind = null;
            UpdateText();

            _action.Enable();
            OnUpdateBindingsEventHandler?.Invoke(true);//todo добавить отмену привязки
        }
 
		private void OnDestroy()
		{
            _rebind?.Dispose();
        }
	}

#if UNITY_EDITOR
    [CustomEditor(typeof(BindingVisualElement))]
    public class BindingVisualElementEditor : UnityEditor.Editor
    {
        private int _selectPopup;
        private SerializedProperty _nameProp;

		private void OnEnable()
		{
            _nameProp = serializedObject.FindProperty("_actionName");
		}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUILayout.Space(15f);
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(35f);
            EditorGUILayout.LabelField("Rebing input name:", GUILayout.Width(150f));
            EditorGUILayout.Space(10f);
            var str = _nameProp.stringValue;

            if (string.IsNullOrEmpty(_nameProp.stringValue))
            {
                _selectPopup = 0;
            }
            else
            {

                for (int i = 0; i < Constants.GetInputActionMapName.Length; i++)
                {
                    if (Constants.GetInputActionMapName[i] == str) _selectPopup = i;
                }
            }

            _selectPopup = EditorGUILayout.Popup(_selectPopup, Constants.GetInputActionMapName, GUILayout.ExpandWidth(true));
            _nameProp.stringValue = Constants.GetInputActionMapName[_selectPopup];

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
	}
#endif
}