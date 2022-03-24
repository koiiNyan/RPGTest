using RPG.ScriptableObjects.Contexts;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace RPG.Editor
{
    public class QuestWindow : BaseCustomEditorWindow
    {
        private QuestContext _context;

        private QuestBaseContext _currentQuestContext;

        private Vector2 _scroll;
        private float GetSpaceWidth => position.width * 0.8f - 8f;

        public QuestContext SelectQuest 
        {
            get => _context; 
            set
            {
                _context = value;
                _currentQuestContext = SelectQuest.GetType().GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(SelectQuest) as QuestBaseContext;
            }
        }

        [MenuItem("Extensions/Windows/Quest Window #b", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<QuestWindow>(false, "Quest Window", true);
            window.minSize = window.maxSize = new Vector2(300f, 450f);
        }

        public override void OnEnable()
        {
            if (EditorPrefs.HasKey("Quest:select"))
                SelectQuest = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("Quest:select"), typeof(QuestContext)) as QuestContext;
        }

        protected override void OnDisable()
        {
            if (SelectQuest != null)
                EditorPrefs.SetString("Quest:select", AssetDatabase.GetAssetPath(SelectQuest));
        }

        private bool PrintHeader()
        {
            //Строка с полем для анимации
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);
            EditorGUILayout.LabelField("Quests:", GUILayout.MaxWidth(90f));

            GUILayout.Space(GetDefaultSpace / 2f);

            //Выбранный квест
            var newRef = EditorGUILayout.ObjectField(SelectQuest, typeof(QuestContext), false, GUILayout.Width(150f)) as QuestContext;
            if (newRef != SelectQuest) SelectQuest = newRef;

            GUILayout.Space(GetDefaultSpace);
            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Load", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                LoadQuestWindow.ShowLoadQuestWindow(this);
                return false;
            }

            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Compile", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                SelectQuest.GetType().GetField("_context", GUIEditorExtensions.GetPrivateReflectionFlags).SetValue(SelectQuest, _currentQuestContext);

                EditorUtility.SetDirty(SelectQuest);
                //Сохраняем файл
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return false;
            }

            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            //Конец лейбла и поля под анимацию
            EditorGUILayout.EndHorizontal();

            return true;
        }

        private void PrintBlocks()
        {
            if (_currentQuestContext == null) return;
            DrawMainParamsContext();

            //Отрисовка двух таблиц
            EditorGUILayout.BeginHorizontal();
            DrawParagraphArray();           
            DrawStringArray();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMainParamsContext()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("ContextID:", GUILayout.MaxWidth(90f));
            _currentQuestContext.ContextID = EditorGUILayout.TextField(_currentQuestContext.ContextID, GUILayout.Width(position.width * 0.2f));
            GUILayout.Space(5f);

            EditorGUILayout.LabelField("DescriptionID:", GUILayout.MaxWidth(90f));
            _currentQuestContext.DescriptionID = EditorGUILayout.TextField(_currentQuestContext.DescriptionID, GUILayout.Width(position.width * 0.5f));

            GUILayout.Space(20f);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20f);
        }

        private void DrawParagraphArray()
        {
            var width = position.width * 0.7f - 20f;
            EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            PrintParagraphs();
            if (AddButton()) _currentQuestContext.ParagraphIDs.Add(new ParagraphQuest());
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5f);
        }

        private void DrawStringArray()
        {
            var width = position.width * 0.3f - 20f;
            EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            PrintRewards();
            if (AddButton()) _currentQuestContext.RewardIDs.Add("");
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5f);
        }

        private bool AddButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var result = GUILayout.Button("Add", GUILayout.Width(100f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            return result;
        }

        private void PrintParagraphs()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Object", GUILayout.Width(150f));
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Condition", GUILayout.Width(60f));
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Paragraph", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            for(int i = 0; i < _currentQuestContext.ParagraphIDs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(5f);
                _currentQuestContext.ParagraphIDs[i].ObjectCondition = EditorGUILayout.TextField(_currentQuestContext.ParagraphIDs[i].ObjectCondition, GUILayout.Width(150f));
                EditorGUILayout.Space(5f);
                _currentQuestContext.ParagraphIDs[i].Condition = EditorGUILayout.IntField(_currentQuestContext.ParagraphIDs[i].Condition, GUILayout.Width(60f));
                EditorGUILayout.Space(5f);
                _currentQuestContext.ParagraphIDs[i].ParagraphID = EditorGUILayout.TextField(_currentQuestContext.ParagraphIDs[i].ParagraphID, GUILayout.Width(200f));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Up", GUILayout.Width(34f)))
                {
                    if (i == 0) break;
                    var pref = _currentQuestContext.ParagraphIDs[i - 1];
                    _currentQuestContext.ParagraphIDs[i - 1] = _currentQuestContext.ParagraphIDs[i];
                    _currentQuestContext.ParagraphIDs[i] = pref;
                    break;
                }
                if (GUILayout.Button("Down", GUILayout.Width(43f)))
                {
                    if (i + 1 == _currentQuestContext.ParagraphIDs.Count) break;
                    var next = _currentQuestContext.ParagraphIDs[i + 1];
                    _currentQuestContext.ParagraphIDs[i + 1] = _currentQuestContext.ParagraphIDs[i];
                    _currentQuestContext.ParagraphIDs[i] = next;
                    break;
                }
                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    _currentQuestContext.ParagraphIDs.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void PrintRewards()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Rewards", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _currentQuestContext.RewardIDs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _currentQuestContext.RewardIDs[i] = EditorGUILayout.TextField(_currentQuestContext.RewardIDs[i], GUILayout.Width(200f));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Up", GUILayout.Width(34f)))
                {
                    if (i == 0) break;
                    var pref = _currentQuestContext.RewardIDs[i - 1];
                    _currentQuestContext.RewardIDs[i - 1] = _currentQuestContext.RewardIDs[i];
                    _currentQuestContext.RewardIDs[i] = pref;
                    break;
                }
                if (GUILayout.Button("Down", GUILayout.Width(43f)))
                {
                    if (i + 1 == _currentQuestContext.RewardIDs.Count) break;
                    var next = _currentQuestContext.RewardIDs[i + 1];
                    _currentQuestContext.RewardIDs[i + 1] = _currentQuestContext.RewardIDs[i];
                    _currentQuestContext.RewardIDs[i] = next;
                    break;
                }
                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    _currentQuestContext.RewardIDs.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            //Отрисовка заголовка
            if (!PrintHeader()) return;

            //Начало отрисовки скролла таблицы
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            PrintBlocks();

            //Конец отрисовки скролла таблицы
            EditorGUILayout.EndScrollView();
        }
    }
}