using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
#endif


namespace RPG.UI.Elements
{
    public class SelectSkillPanel : MonoBehaviour
    {
        [SerializeField]
        private string _richTextStart;
        [SerializeField]
        private string _richTextEnd;

        [SerializeField]
        private SkillIconVisualElement _element;
        [SerializeField]
        private TextMeshProUGUI _skillName;
        [SerializeField]
        private TextMeshProUGUI _cooldownText;

        [Space, SerializeField]
        private InputSetType _inputBind;

        public InputSetType GetBind => _inputBind;
        public SkillIconVisualElement Element { get => _element; set => _element = value; }

        public void SetStrings(string name, string descCd, string cd)
        {
            _skillName.text = name;
            _cooldownText.text = string.Concat(descCd, _richTextStart, cd, _richTextEnd);
		}
    }
    /*
#if UNITY_EDITOR
    [CustomEditor(typeof(SelectSkillPanel))]
    public class SelectSkillPanelEditor : UnityEditor.Editor
    {
		#region Patterns

		private readonly string _boldPatternStart = "<b>";
        private readonly string _boldPatternEnd = "</b>";

        private readonly string _italicPatternStart = "<i>";
        private readonly string _italicPatternEnd = "</i>";

        private readonly string _underlinePatternStart = "<u>";
        private readonly string _underlinePatternEnd = "</u>";

        private readonly string _strikethroughPatternStart = "<s>";
        private readonly string _strikethroughPatternEnd = "</s>";

        private readonly string _colorPatternStart = @"<color=#\w*>";
        private readonly string _colorPatternEnd = @"</color>";

        private readonly string _sizePatternStart = @"<size=\d*>";
        private readonly string _sizePatternEnd = @"</size>";

        #endregion

        private bool _foldout;
        private SerializedProperty _richTextStartProp;
        private SerializedProperty _richTextEndProp;

		#region Rich Settings
		
        private bool _bold;
        private bool _italic;
        private bool _underline;
        private bool _strikethrough;
        private bool _color;
        private bool _size;
        private Color _colorValue;
        private float _sizeValue;
        
        #endregion

        private void OnEnable()
        {
            _richTextStartProp = serializedObject.FindProperty("_richTextStart");
            _richTextEndProp = serializedObject.FindProperty("_richTextEnd");

            _bold = _richTextStartProp.stringValue.Contains("<b>");
            _italic = _richTextStartProp.stringValue.Contains("<i>");
            _underline = _richTextStartProp.stringValue.Contains("<u>");
            _strikethrough = _richTextStartProp.stringValue.Contains("<s>");

            var reg = new Regex(_colorPatternStart);
            _color = reg.IsMatch(_richTextStartProp.stringValue);
            if(_color)
            {
                var str = reg.Match(_richTextStartProp.stringValue).Value;
                ColorUtility.TryParseHtmlString(str.Replace("<color=", "").Replace(">", ""), out _colorValue);
			}

            reg = new Regex(_sizePatternStart);
            _size = reg.IsMatch(_richTextStartProp.stringValue);
            if(_size)
            {
                var str = reg.Match(_richTextStartProp.stringValue).Value;
                float.TryParse(str.Replace("<size=", "").Replace(">", ""), out _sizeValue);
			}
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10f);
            //_foldout = EditorGUILayout.Foldout(_foldout, "Rich Settings", new GUIStyle("foldout") { fontSize = 13, fontStyle = FontStyle.Bold });

            //if (_foldout)
            //{
            #region 

            var value = GUILayout.Toggle(_bold, "Use Bold");
            if (value != _bold)
            {
                _bold = value;
                if (value) AddRichSetting(_boldPatternStart, _boldPatternEnd);
                else RemoveRichSetting(_boldPatternStart, _boldPatternEnd);
            }
            value = GUILayout.Toggle(_italic, "Use Italic");
            if (value != _italic)
            {
                _italic = value;
                if (value) AddRichSetting(_italicPatternStart, _italicPatternEnd);
                else RemoveRichSetting(_italicPatternStart, _italicPatternEnd);
            }
            value = GUILayout.Toggle(_underline, "Use Underline");
            if (value != _underline)
            {
                _underline = value;
                if (value) AddRichSetting(_underlinePatternStart, _underlinePatternEnd);
                else RemoveRichSetting(_underlinePatternStart, _underlinePatternEnd);
            }
            value = GUILayout.Toggle(_strikethrough, "Use Strikethrough");
            if (value != _strikethrough)
            {
                _strikethrough = value;
                if (value) AddRichSetting(_strikethroughPatternStart, _strikethroughPatternEnd);
                else RemoveRichSetting(_strikethroughPatternStart, _strikethroughPatternEnd);
            }

            #endregion

            EditorGUILayout.BeginHorizontal();
            _color = GUILayout.Toggle(_color, "Use Color");
            var colorPatternStartOld = _colorPatternStart.Replace(@"\w*", ColorUtility.ToHtmlStringRGBA(_colorValue));
            _colorValue = EditorGUILayout.ColorField(_colorValue);
            var colorPatternStartNew = _colorPatternStart.Replace(@"\w*", ColorUtility.ToHtmlStringRGBA(_colorValue));

            if (colorPatternStartOld != colorPatternStartNew || !_color)
                RemoveAndAddRichSetting(colorPatternStartOld, colorPatternStartNew, _colorPatternEnd);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _size = GUILayout.Toggle(_size, "Use Size");
            var sizePatternStartOld = _sizePatternStart.Replace(@"\d*", _sizeValue.ToString());
            _sizeValue = EditorGUILayout.FloatField(_sizeValue);
            var sizePatternStartNew = _sizePatternStart.Replace(@"\d*", _sizeValue.ToString());

            if (sizePatternStartOld != sizePatternStartNew || !_size)
                RemoveAndAddRichSetting(sizePatternStartOld, sizePatternStartNew, _sizePatternEnd);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            //}
        }

        private void RemoveRichSetting(string pattern, string end)
        {
            _richTextStartProp.stringValue = _richTextStartProp.stringValue.Replace(pattern, "");
            _richTextEndProp.stringValue = _richTextEndProp.stringValue.Replace(end, "");
        }

        private void AddRichSetting(string pattern, string end)
        {
            _richTextStartProp.stringValue = pattern + _richTextStartProp.stringValue;
            _richTextEndProp.stringValue = _richTextEndProp.stringValue + end;
        }

        private void RemoveAndAddRichSetting(string startOld, string startNew, string end)
        {
            RemoveRichSetting(startOld, end);
            AddRichSetting(startNew, end);
        }
    }

#endif
*/
}
