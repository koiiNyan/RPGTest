using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace RPG.Editor
{
    public class AnimationFixWindow : BaseCustomEditorWindow
    {
        private AnimationClip _clip;
        //private string _curveName = string.Empty;
        private float _deltaKeyframe;
        
        //Все кривые привязки анимации
        private EditorCurveBinding[] _curveBindings;
        private AnimationCurve _selectCurve;
        //Список путей в анимации
        private string[] _allCurvesPathes;
        //Список названий свойств по выбранному пути
        private string[] _sortCurvesPropertyNames;

        private int _selectCurvePathIndex;
        private int _selectCurvePropertyNameIndex;
        private int _selectPopupCalculate;
        private string[] _operations = new string[] { "+", "-", "*" };
        private float _firstElementCalculate;
        private float _secondElementCalculate;
        private float _resultElementCalculate;

        #region Settings

        private float _calcLen = 100f;
        private bool _changeCurve;

        #endregion

        [MenuItem("Extensions/Windows/Animation Curve Repair Window #c", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<AnimationFixWindow>(false, "Animation Curve Repair Window", true);
            window.minSize = window.maxSize = new Vector2(300f, 450f);
        }

        public override void OnEnable()
        {
            if (EditorPrefs.HasKey("AnimFix:clip"))
            {
                _clip = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("AnimFix:clip"), typeof(AnimationClip)) as AnimationClip;
                _curveBindings = AnimationUtility.GetCurveBindings(_clip);
                _allCurvesPathes = _curveBindings.Select(t => t.path).ToArray();
            }
            //if(EditorPrefs.HasKey("AnimFix:curve"))
            //    _curveName = EditorPrefs.GetString("AnimFix:curve");
            if (EditorPrefs.HasKey("AnimFix:example"))
                _deltaKeyframe = EditorPrefs.GetFloat("AnimFix:example");


            
        }

        protected override void OnDisable()
		{
            EditorPrefs.SetString("AnimFix:clip", AssetDatabase.GetAssetPath(_clip));
            //EditorPrefs.SetString("AnimFix:curve", _curveName);
            EditorPrefs.SetFloat("AnimFix:example", _deltaKeyframe);
        }

        private bool PrintHeader()
        {
            //Строка с полем для анимации
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);
            EditorGUILayout.LabelField("Animation clip:", GUILayout.MaxWidth(90f));
            GUILayout.Space(GetDefaultSpace / 2f);
            _clip = (AnimationClip)EditorGUILayout.ObjectField(_clip, typeof(AnimationClip), false);

            GUILayout.Space(GetDefaultSpace);
            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Cyan];
            if (GUILayout.Button("Get curves", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                _changeCurve = false;
                _curveBindings = AnimationUtility.GetCurveBindings(_clip);
                _allCurvesPathes = _curveBindings.Select(t => t.path).ToArray();
                var binding = _curveBindings.First(t => t.path == _allCurvesPathes[_selectCurvePathIndex] && t.propertyName == _sortCurvesPropertyNames[_selectCurvePropertyNameIndex]);
                _selectCurve = AnimationUtility.GetEditorCurve(_clip, binding);
            }
            GUILayout.Space(20f);

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Green];
            if (GUILayout.Button("Fix Keyframe", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                _changeCurve = true;
                var frames = _selectCurve.keys;
                //Изменяли саму кривую вручную
                for (int i = 0; i < frames.Length; i++)
                    frames[i].value += _deltaKeyframe;
                _selectCurve.keys = frames;
            }
            GUILayout.Space(20f);

            GUI.color = Color.cyan;
            if (_changeCurve && GUILayout.Button("Save", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                AnimationUtility.SetEditorCurve(_clip, AnimationUtility.GetCurveBindings(_clip).First(t => t.propertyName == _sortCurvesPropertyNames[_selectCurvePropertyNameIndex] && t.path == _allCurvesPathes[_selectCurvePathIndex]), _selectCurve);
                //Сохраняем файл
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Default];

            GUILayout.FlexibleSpace();
            //Конец лейбла и поля под анимацию
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20f);

            return true;
		}

        private bool PrintSelectorCurves()
        {
            if (_curveBindings == null || _allCurvesPathes == null || _allCurvesPathes.Length == 0) return false;
            EditorGUILayout.LabelField("Curves", GUIEditorExtensions.GetHeaderLabelStyle(position));

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);

            EditorGUILayout.LabelField("Curve path:", GUILayout.MaxWidth(85f));
            var shift = _selectCurvePathIndex;
            _selectCurvePathIndex = EditorGUILayout.Popup(_selectCurvePathIndex, _allCurvesPathes, GUILayout.MaxWidth(400f));
            //Обновление внутренних данных, при изменении селекторов
            if (shift != _selectCurvePathIndex || _sortCurvesPropertyNames == null)
                _sortCurvesPropertyNames = _curveBindings.Where(t => t.path == _allCurvesPathes[_selectCurvePathIndex]).Select(t => t.propertyName).ToArray();

            GUILayout.Space(GetDefaultSpace / 2f);

            EditorGUILayout.LabelField("Curve name:", GUILayout.MaxWidth(85f));
            shift = _selectCurvePropertyNameIndex;
            _selectCurvePropertyNameIndex = EditorGUILayout.Popup(_selectCurvePropertyNameIndex, _sortCurvesPropertyNames, GUILayout.MaxWidth(160f));
            //Обновление внутренних данных, при изменении селекторов
            if (shift != _selectCurvePropertyNameIndex || _selectCurve == null)
            {
                var binding = _curveBindings.First(t => t.path == _allCurvesPathes[_selectCurvePathIndex] && t.propertyName == _sortCurvesPropertyNames[_selectCurvePropertyNameIndex]);
                _selectCurve = AnimationUtility.GetEditorCurve(_clip, binding);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return true;
        }

        private bool PrintCurveFixSettings()
        {
            if (_selectCurve == null) return false;
            if (_selectCurve.length == 0)
            {
                EditorGUILayout.HelpBox("Select curve not contains keys in keyframes", MessageType.Warning);
                var binding = _curveBindings.First(t => t.path == _allCurvesPathes[_selectCurvePathIndex] && t.propertyName == _sortCurvesPropertyNames[_selectCurvePropertyNameIndex]);
                _selectCurve = AnimationUtility.GetEditorCurve(_clip, binding);
                return false;
            }

            DrawHeader("Settings");
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);

            EditorGUILayout.LabelField("Current first value:", GUILayout.MaxWidth(150f));
            EditorGUILayout.FloatField(_selectCurve.keys[0].value, GUILayout.MaxWidth(120f));

            GUILayout.Space(GetDefaultSpace / 2f);

            EditorGUILayout.LabelField("Delta value:", GUILayout.MaxWidth(85f));
            _deltaKeyframe = EditorGUILayout.FloatField(_deltaKeyframe, GUILayout.MaxWidth(120f));

            GUILayout.Space(GetDefaultSpace / 2f);

            EditorGUILayout.LabelField("Preview result:", GUILayout.MaxWidth(95f));
            GUI.enabled = false;
            EditorGUILayout.FloatField(_selectCurve.keys[0].value + _deltaKeyframe, GUILayout.MaxWidth(120f));
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            return true;
		}

        private bool PrintPreview()
        {
            DrawHeader("Preview");
            EditorGUILayout.CurveField(_selectCurve, GUILayout.Width(position.width - 30f), GUILayout.Height(200f));
            return true;
		}

        private bool PrintCalculator()
        {
            GUILayout.Space(80f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.6f);
            DrawHeader("Calculator");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.63f);
            GUILayout.BeginHorizontal("box");
            
            GUI.color = new Color(0.65f, 0.95f, 0.95f);

            EditorGUILayout.BeginHorizontal("box");
            _firstElementCalculate = EditorGUILayout.FloatField(_firstElementCalculate, GUILayout.Width(_calcLen));
            _selectPopupCalculate = EditorGUILayout.Popup(_selectPopupCalculate, _operations, GUILayout.Width(30f));
            _secondElementCalculate = EditorGUILayout.FloatField(_secondElementCalculate, GUILayout.Width(_calcLen));
            EditorGUILayout.LabelField("=", GUILayout.Width(20f));
            _resultElementCalculate = EditorGUILayout.FloatField(Сalculate(), GUILayout.Width(_calcLen));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Default];
            GUILayout.EndHorizontal();
            return true;
		}

        private void DrawHeader(string text)
        {
            GUILayout.Space(20f);
            EditorGUILayout.LabelField(text, GUIEditorExtensions.GetHeaderLabelStyle(position));
        }

        private float Сalculate()
        {
            switch (_selectPopupCalculate)
            {
                case 0:
                    _resultElementCalculate = _firstElementCalculate + _secondElementCalculate;
                    break;
                case 1:
                    _resultElementCalculate = _firstElementCalculate - _secondElementCalculate;
                    break;
                case 2:
                    _resultElementCalculate = _firstElementCalculate * _secondElementCalculate;
                    break;
            }

            return _resultElementCalculate;
        }


        protected override void OnGUI()
        {
            base.OnGUI();
            //Отрисовка заголовка
            if (!PrintHeader()) return;
            //Отрисовка выбора кривых
            if (!PrintSelectorCurves()) return;
            //Отрисовка окна настроек изменений
            if (!PrintCurveFixSettings()) return;
            //Отрисовка превью кривой
            if (!PrintPreview()) return;
            //Отрисовка калькулятора
            if (!PrintCalculator()) return;
        }
    }
}